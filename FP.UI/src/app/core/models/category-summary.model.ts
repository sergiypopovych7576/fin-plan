import { OperationType } from './operation-type.model';

export interface ICategorySummary {
    name: string;
    amount: number;
    color: string;
    type: OperationType
}