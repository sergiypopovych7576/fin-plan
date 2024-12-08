import { Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { Observable } from 'rxjs';
import { IMonthSummary, IOperation } from '../models';
import { BaseService } from './base.service';

@Injectable({ providedIn: 'root' })
export class OperationsService extends BaseService<IOperation> {
	protected override _url = 'operations';
	public operationsDictionary = new Map<string, WritableSignal<IOperation[]>>();

	public getOperationSignal(date: string): WritableSignal<IOperation[]> {
		const key = date;
		const exists = this.operationsDictionary.get(key);
		if (exists) {
			return exists;
		}
		const keySignal = signal([]) as WritableSignal<IOperation[]>;
		this.getOperations(date).subscribe(c => keySignal.set(c));
		this.operationsDictionary.set(key, keySignal);
		return keySignal;
	}

	public refreshOperations(date: string): void {
		const key = `${date}`;
		const signal = this.operationsDictionary.get(key);
		if (!signal) {
			return;
		}
		this.getOperations(date).subscribe(c => signal.set(c));
	}

	public refreshAllOperations(): void {
		this.operationsDictionary.clear();
	}

	public override get(date: string): Signal<IOperation[]> {
		return this.getOperationSignal(date);
	}

	public override refresh(): void {
		this.operationsDictionary.forEach((value, key) => {
			this.refreshOperations(key);
		});
	}

	public getOperations(date: string): Observable<IOperation[]> {
		return this._httpClient.get<IOperation[]>(
			`operations/month/${date}`,
		);
	}

	public getSummaryByRange(startDate: string, endDate: string): Observable<IMonthSummary[]> {
		return this._httpClient.get<IMonthSummary[]>(
			`operations/summary?startDate=${startDate}&endDate=${endDate}`,
		);
	}

	public sync(): Observable<unknown> {
		return this._httpClient.post(
			'operations/sync',
			{}
		);
	}
}
