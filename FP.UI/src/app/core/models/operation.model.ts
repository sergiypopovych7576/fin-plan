import { IBaseModel } from './base.model';
import { ICategory } from './category.model';
import { OperationType } from './operation-type.model';

export interface IOperation extends IBaseModel {
	name: string;
	amount: number;
	date: string;
	type: OperationType;
	category: ICategory;
	applied: boolean;
	scheduledOperationId: string;
}
