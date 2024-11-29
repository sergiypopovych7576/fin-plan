import { HttpClient } from '@angular/common/http';
import { inject, Signal, signal, WritableSignal } from '@angular/core';
import { Observable } from 'rxjs';
import { IEntityService } from './entity-service';

export class BaseService<T> implements IEntityService<T> {
	protected readonly _httpClient = inject(HttpClient);
	protected readonly _entities: WritableSignal<T[]> = signal([]);
	protected _url = '';

	public get(args?: any): Signal<T[]> {
		return this._entities.asReadonly();
	}

	public refresh(): void {
		this._httpClient.get<T[]>(this._url).subscribe(c => this._entities.set(c));
	}

	public update(entity: unknown): Observable<void> {
		return this._httpClient.put<void>(this._url, entity);
	}

	public create(entity: unknown): Observable<void> {
		return this._httpClient.post<void>(this._url, entity);
	}

	public delete(id: string): Observable<void> {
		return this._httpClient.delete<void>(`${this._url}/${id}`);
	}
}
