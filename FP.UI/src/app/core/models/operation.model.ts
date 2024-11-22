import { IAccount } from './account.model';
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
	scheduledOperationId?: string;
	sourceAccount: IAccount;
	targetAccount: IAccount;
	sourceAccountId?: string;
	targetAccountId?: string;
}
