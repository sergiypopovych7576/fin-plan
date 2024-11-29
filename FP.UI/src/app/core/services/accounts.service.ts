import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IAccount, IAccountBalance } from '../models';
import { BaseService } from './base.service';

@Injectable({ providedIn: 'root' })
export class AccountsService extends BaseService<IAccount> {
	protected override _url = 'accounts';

	constructor() {
		super();
		this.refresh();
	}

	public getBalance(accountId: string, date: unknown): Observable<IAccountBalance> {
		return this._httpClient.get<IAccountBalance>(`${this._url}/${accountId}/balance?targetDate=${date}`);
	}
}
