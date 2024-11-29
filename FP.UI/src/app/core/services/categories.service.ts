import { Injectable } from '@angular/core';
import { ICategory } from '../models';
import { BaseService } from './base.service';

@Injectable({ providedIn: 'root' })
export class CategoriesService extends BaseService<ICategory> {
	protected override _url = 'categories';

	constructor() {
		super();
		this.refresh();
	}
}
