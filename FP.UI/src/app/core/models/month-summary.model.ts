import { ICategorySummary } from './category-summary.model';

export interface IMonthSummary {
    year: number,
    month: number,
    totalExpenses: number;
    totalIncomes: number;
    monthBalance: number;
    categories: ICategorySummary[]
};
