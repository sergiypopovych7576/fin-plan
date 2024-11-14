import { IBaseModel } from './base.model';

export interface IAccount extends IBaseModel {
	name: string;
    balance: number;
    isDefault: boolean;
    currency: string;
}
