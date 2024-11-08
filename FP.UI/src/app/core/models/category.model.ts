import { IBaseModel } from './base.model';

export interface ICategory extends IBaseModel {
	name: string;
	color: string;
}
