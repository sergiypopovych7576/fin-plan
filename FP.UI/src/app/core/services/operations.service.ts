import { Injectable, signal, WritableSignal } from '@angular/core';
import { Observable } from 'rxjs';
import { IOperation } from '../models';
import { BaseService } from './base.service';

@Injectable({ providedIn: 'root' })
export class OperationsService extends BaseService {
	public operationsDictionary = new Map<string, WritableSignal<IOperation[]>>();

	public getOperationSignal(date: string): WritableSignal<IOperation[]> {
		const key = date;
		const exists = this.operationsDictionary.get(key);
		if(exists) {
			return exists;
		}
		const keySignal = signal([]) as WritableSignal<IOperation[]>;
		this.get(date).subscribe(c => keySignal.set(c));
		this.operationsDictionary.set(key, keySignal);
		return keySignal;
	}

	public refreshOperations(date: string): void {
		const key = `${date}`;
		const signal = this.operationsDictionary.get(key);
		if(!signal) {
			return;
		}
		this.get(date).subscribe(c => signal.set(c));
	}

	public refreshAllOperations(): void {
		this.operationsDictionary.clear();
	}

	public get(date: string): Observable<IOperation[]> {
		return this._httpClient.get<IOperation[]>(
			`operations?date=${date}`,
		);
	}

	public sync(): Observable<unknown> {
		return this._httpClient.post(
			'operations/sync',
			{}
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
