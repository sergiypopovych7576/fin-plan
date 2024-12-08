import { Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { IAccount, IAccountBalance } from '../models';
import { BaseService } from './base.service';

@Injectable({ providedIn: 'root' })
export class AccountsService extends BaseService<IAccount> {
	protected override _url = 'accounts';
	private _balances = new Map<string, WritableSignal<IAccountBalance>>();

	constructor() {
		super();
		this.refresh();
	}

	public override refresh(): void {
		super.refresh();
		this._balances.forEach((value, key) => {
			const accountId = key.split('*')[0];
			const date = key.split('*')[1];
			this._httpClient.get<IAccountBalance>(`${this._url}/${accountId}/balance?targetDate=${date}`).subscribe(c => {
				value.set(c);
			});
		});
	}

	public getBalance(accountId: string, date: unknown): Signal<IAccountBalance> {
		const key = `${accountId}*${date}`;
		let balance = this._balances.get(key);
		if (balance) {
			return balance;
		}
		balance = signal({}) as WritableSignal<IAccountBalance>;
		this._balances.set(key, balance);
		this._httpClient.get<IAccountBalance>(`${this._url}/${accountId}/balance?targetDate=${date}`).subscribe(c => {
			balance.set(c);
		});
		return balance.asReadonly();
	}
}
