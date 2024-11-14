import { IBaseModel } from './base.model';
import { OperationType } from './operation-type.model';

export interface ICategory extends IBaseModel {
	name: string;
	color: string;
	type: OperationType;
	iconName: string;
}
