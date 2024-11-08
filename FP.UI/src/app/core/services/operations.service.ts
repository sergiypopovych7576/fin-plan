import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IOperation } from '../models';
import { BaseService } from './base.service';

@Injectable({ providedIn: 'root' })
export class OperationsService extends BaseService {
	public get(year: number, month: number): Observable<IOperation[]> {
		return this._httpClient.get<IOperation[]>(
			`operations?year=${year}&month=${month}`,
		);
	}

	public create(operation: IOperation): Observable<unknown> {
		return this._httpClient.post(
			'operations',
			operation
		);
	}
}
