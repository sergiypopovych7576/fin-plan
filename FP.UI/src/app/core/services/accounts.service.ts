import { Injectable, signal, WritableSignal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { IAccount, IAccountBalance } from '../models';
import { BaseService } from './base.service';

@Injectable({ providedIn: 'root' })
export class AccountsService extends BaseService {
	private readonly _url = 'accounts';
	public accounts: WritableSignal<IAccount[]> = signal([]);

	constructor() {
		super();
		this.loadAccounts();
	}

	public loadAccounts(): void {
		this._httpClient.get<IAccount[]>(this._url).subscribe(c => this.accounts.set(c));
	}

	public getBalance(date: unknown): Observable<IAccountBalance> {
		return this._httpClient.get<IAccountBalance>(`${this._url}/balance?targetDate=${date}`);
	}

	public update(account: IAccount): Observable<void> {
		return this._httpClient.put<void>(this._url, account).pipe(tap(c => this.loadAccounts()));
	}

	public create(account: IAccount): Observable<void> {
		return this._httpClient.post<void>(this._url, account).pipe(tap(c => this.loadAccounts()));
	}

	public delete(id: string): Observable<void> {
		return this._httpClient.delete<void>(`${this._url}/${id}`).pipe(tap(c => this.loadAccounts()));
	}
}
