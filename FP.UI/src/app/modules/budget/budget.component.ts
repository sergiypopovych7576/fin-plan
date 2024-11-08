import { Component, inject, OnInit } from '@angular/core';
import { Chart, registerables } from 'chart.js';
import { MatDialog } from '@angular/material/dialog';
import { OperationModalDialogComponent } from './operation-modal';
import { ICategory, IOperation, OperationType } from '@fp-core/models';
import { CategoriesService, OperationsService } from '@fp-core/services';

@Component({
	selector: 'fp-budget',
	templateUrl: './budget.component.html',
	styleUrl: './budget.component.scss',
})
export class BudgetComponent implements OnInit {
	public incomes: IOperation[] = []
	public expenses: IOperation[] = []
	public chart: any;
	public chart2: any;
	public categories: ICategory[] = [];
	private readonly _dialog = inject(MatDialog);
	private readonly _categoriesService = inject(CategoriesService);
	private readonly _operationsService = inject(OperationsService);

	public ngOnInit(): void {
		this.createChart();
		this._operationsService.get(2024, 11).subscribe((operations) => {
			this.incomes = operations.filter(c => c.type === OperationType.Incomes);
			this.expenses = operations.filter(c => c.type === OperationType.Expenses);
		});
	}

	public onAddOperation(): void {
		const dialogRef = this._dialog.open(OperationModalDialogComponent, {
			data: { name: 'Rent', amount: 375, date: "2024-11-08T11:49:46Z", type: 0, categoryId: "18bdd321-c65c-4bc4-96f8-15883d95fb4a" },
		});
		dialogRef.afterClosed().subscribe((result) => {
			if (result) {
				this._operationsService.create(result).subscribe()
			}
		});
	}

	public createChart() {
		Chart.register(...registerables);
		const data = {
			labels: ['Needs', 'Wants', 'Subscriptions', 'Presents'],
			datasets: [
				{
					label: 'Expenses',
					data: [375 + 150 + 400 + 100, 200 + 100, 6 + 19 + 5, 100],
					backgroundColor: [
						'rgb(255, 99, 132)',
						'rgb(54, 162, 235)',
						'rgb(255, 205, 86)',
						'rgb(0, 92, 187)',
					],
					hoverOffset: 4,
				},
			],
		};
		const incomeData = {
			labels: ['Salary', 'Investments', 'Loans'],
			datasets: [
				{
					label: 'Incomes',
					data: [2000, 200, 50],
					backgroundColor: [
						'rgb(0, 92, 187)',
						'rgb(255, 205, 86)',
						'rgb(54, 162, 235)',
					],
					hoverOffset: 4,
				},
			],
		};
		this.chart = new Chart('MyChart', {
			type: 'doughnut',
			data: incomeData,
		});
		this.chart2 = new Chart('MyChart2', {
			type: 'doughnut',
			data: data,
		});
	}
}
