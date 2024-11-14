import { Injectable, signal, WritableSignal } from '@angular/core';
import { Observable } from 'rxjs';
import { IOperation } from '../models';
import { BaseService } from './base.service';

@Injectable({ providedIn: 'root' })
export class OperationsService extends BaseService {
	public operationsDictionary = new Map<string, WritableSignal<IOperation[]>>();

	public getOperationSignal(year: number, month: number): WritableSignal<IOperation[]> {
		const key = `${year}-${month}`;
		const exists = this.operationsDictionary.get(key);
		if(exists) {
			return exists;
		}
		const keySignal = signal([]) as WritableSignal<IOperation[]>;
		this.get(year, month).subscribe(c => keySignal.set(c));
		this.operationsDictionary.set(key, keySignal);
		return keySignal;
	}

	public refreshOperations(year: number, month: number): void {
		const key = `${year}-${month}`;
		const signal = this.operationsDictionary.get(key);
		if(!signal) {
			return;
		}
		this.get(year, month).subscribe(c => signal.set(c));
	}

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

	public delete(operationId: string): Observable<unknown> {
		return this._httpClient.delete(
			`operations/${operationId}`,
		);
	}
}
