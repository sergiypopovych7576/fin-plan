import { Component, computed, Input, Signal } from '@angular/core';
import { IOperation } from '@fp-core/models';

@Component({
    selector: 'fp-operations-widget',
    templateUrl: './operations-widget.component.html',
})
export class OperationsWidgetComponent {
    private _operations!: Signal<IOperation[]>;

    @Input()
    public set operations(operations: Signal<IOperation[]>) {
        this._operations = operations;
        this.chartData = computed(() => {
            return this.generateChartData(this.operations());
        })
    };

    public get operations(): Signal<IOperation[]> {
        return this._operations;
    }

    public chartData!: Signal<any>;

    private generateChartData(operations: IOperation[]): any {
        // Filter operations based on the specified type
        const filteredOperations = operations;

        // Calculate the total amount for the filtered operations
        const totalAmount = filteredOperations.reduce((sum, op) => sum + op.amount, 0);

        // Array to hold category data for easy access
        const arr = [] as any[];

        // Group operations by category, sum amounts, and calculate percentage
        const categoryData = filteredOperations.reduce((acc, op) => {
            const categoryName = op.category.name;

            if (!acc[categoryName]) {
                acc[categoryName] = { name: categoryName, amount: 0, color: op.category.color, percentage: 0 };
            }

            acc[categoryName].amount += op.amount;
            acc[categoryName].percentage = (acc[categoryName].amount / totalAmount) * 100;

            // Push only unique categories to `arr`
            if (!arr.includes(acc[categoryName])) {
                arr.push(acc[categoryName]);
            }

            return acc;
        }, {} as Record<string, { name: string, amount: number; color: string; percentage: number }>);

        // Sort by percentage in descending order
        arr.sort((a, b) => b.percentage - a.percentage);

        // if (type === OperationType.Expenses) {
        // 	this.expenseCategories.set(arr as any);
        // } else {
        // 	this.incomeCategories.set(arr as any);
        // }

        // Generate labels, data, and colors arrays for the chart
        const labels = arr.map(item => item.name);
        const data = arr.map(item => item.amount);
        const backgroundColors = arr.map(item => item.color);

        // Return chart configuration
        return {
            type: 'doughnut',
            data: {
                labels,
                datasets: [
                    {
                        data,
                        backgroundColor: backgroundColors,
                        hoverOffset: 4,
                    },
                ],
            },
            options: {
                plugins: {
                    legend: {
                        display: false,
                        position: 'right'
                    }
                }
            }
        };
    }
}
