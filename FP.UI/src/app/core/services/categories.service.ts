import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ICategory } from '../models';
import { BaseService } from './base.service';

@Injectable({ providedIn: 'root' })
export class CategoriesService extends BaseService {
	public get(): Observable<ICategory[]> {
		const categories = this._httpClient.get<ICategory[]>('categories');
		return this.getOrReceiveFromCache(categories) as Observable<ICategory[]>;
	}
}
