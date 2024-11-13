import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IAccount } from '../models';
import { BaseService } from './base.service';

@Injectable({ providedIn: 'root' })
export class AccountsService extends BaseService {
	public get(): Observable<IAccount[]> {
		const categories = this._httpClient.get<IAccount[]>('accounts');
		return this.getOrReceiveFromCache(categories) as Observable<IAccount[]>;
	}
}
