import { Component, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { OperationModalDialogComponent } from './operation-modal';
import { ICategory, IOperation, OperationType } from '@fp-core/models';
import { CategoriesService, OperationsService } from '@fp-core/services';
import { IDateChange } from './month-selector/date-change.model';
import { timer } from 'rxjs';

@Component({
	selector: 'fp-budget',
	templateUrl: './budget.component.html',
	styleUrl: './budget.component.scss',
})
export class BudgetComponent implements OnInit {
	public incomes: WritableSignal<IOperation[]> = signal([]);
	public expenses: WritableSignal<IOperation[]> = signal([]);
	public chart: any;
	public categories: ICategory[] = [];
	public operationsLoading = signal(false);
	public expensesChartData = {
		type: 'doughnut',
		data: {
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
		}
	};
	public incomesChartData =
		{
			type: 'doughnut',
			data: {
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
			}
		};
	private readonly _dialog = inject(MatDialog);
	private readonly _operationsService = inject(OperationsService);

	private _selectedYear = 2024;
	private _selectedMonthNumber = 11;

	public ngOnInit(): void {
		this.refreshOperations();
	}

	public onAddOperation(): void {
		const dialogRef = this._dialog.open(OperationModalDialogComponent, {
			data: { name: 'Rent', amount: 375, date: "2024-11-08T11:49:46Z", type: 0, categoryId: "18bdd321-c65c-4bc4-96f8-15883d95fb4a" },
		});
		dialogRef.afterClosed().subscribe((result) => {
			if (result) {
				this._operationsService.create(result).subscribe(() => {
					this.refreshOperations();
				})
			}
		});
	}

	public refreshOperations(): void {
		this._operationsService.get(this._selectedYear, this._selectedMonthNumber).subscribe((operations) => {
			this.incomes.set(operations.filter(c => c.type === OperationType.Incomes));
			this.expenses.set(operations.filter(c => c.type === OperationType.Expenses));
			timer(600).subscribe(() => this.operationsLoading.set(false));
		});
	}

	public onDateChange(event: IDateChange): void {
		this._selectedMonthNumber = event.month;
		this._selectedYear = event.year;
		this.refreshOperations();
	}

	public onOperationDelete(operation: IOperation): void {
		this.operationsLoading.set(true);
		this._operationsService.delete(operation.id).subscribe(() => this.refreshOperations());
	}
}
