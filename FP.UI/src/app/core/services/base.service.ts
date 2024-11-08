import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, of, tap } from 'rxjs';

export class BaseService {
	protected readonly _httpClient = inject(HttpClient);
	protected _cachedData: unknown;

	public getOrReceiveFromCache(
		request: Observable<unknown>,
	): Observable<unknown> {
		if (this._cachedData) {
			return of(this._cachedData);
		}
		return request.pipe(tap((data) => (this._cachedData = data)));
	}
}
