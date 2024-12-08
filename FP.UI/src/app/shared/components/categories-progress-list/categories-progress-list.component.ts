import { Component, computed, Input, signal, Signal } from '@angular/core';
import { IOperation, OperationType } from '@fp-core/models';


@Component({
    selector: 'fp-categories-progress-list',
    templateUrl: './categories-progress-list.component.html',
    styleUrls: ['./categories-progress-list.component.scss']
})
export class CategoriesProgressListComponent {
    private _operations!: Signal<IOperation[]>;

    @Input()
    public set operations(value: Signal<IOperation[]>) {
        this._operations = value;
        this.calculations = computed(() => {
            const operations = this.operations().filter(c => c.type == this.type());
            const totalAmount = this._calculateTotal(operations);
            const categoryData = operations.reduce((acc, op) => {
                const categoryName = op.category.name;
                if (!acc[categoryName]) {
                    acc[categoryName] = { name: categoryName, amount: 0, color: op.category.color, percentage: '0' };
                }
                acc[categoryName].amount += op.amount;
                acc[categoryName].percentage = `${(acc[categoryName].amount / totalAmount) * 100}`;
                return acc;
            }, {} as Record<string, { name: string; amount: number; color: string; percentage: string }>);
            return Object.values(categoryData).sort((a, b) => +b.percentage - +a.percentage);
        })
    }

    public get operations() {
        return this._operations;
    }

    public type = signal(OperationType.Expenses);

    public calculations!: any;

    private _calculateTotal(operations: IOperation[]): number {
		return operations.reduce((sum, op) => sum + op.amount, 0);
	}

    public onExpenses() {
        this.type.set(OperationType.Expenses);
    }

    public onIncomes() {
        this.type.set(OperationType.Incomes);
    }
}
