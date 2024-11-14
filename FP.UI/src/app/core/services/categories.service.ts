import { Injectable, signal, WritableSignal } from '@angular/core';
import { ICategory } from '../models';
import { BaseService } from './base.service';

@Injectable({ providedIn: 'root' })
export class CategoriesService extends BaseService {
	private readonly _url = 'categories';
	public categories: WritableSignal<ICategory[]> = signal([]);

	constructor() {
		super();
		this.loadCategories();
	}

	public loadCategories(): void {
		this._httpClient.get<ICategory[]>(this._url).subscribe(c => this.categories.set(c));
	}
}
