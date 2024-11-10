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
	public operations: WritableSignal<IOperation[]> = signal([]);
	public chart: any;
	public categories: ICategory[] = [];
	public operationsLoading = signal(false);
	private readonly _categoriesService = inject(CategoriesService);
	public expensesChartData: unknown;
	public incomesChartData: unknown;
	private readonly _dialog = inject(MatDialog);
	private readonly _operationsService = inject(OperationsService);

	private _selectedYear = 2024;
	private _selectedMonthNumber = 11;

	public ngOnInit(): void {
		this._categoriesService.get().subscribe(c => this.categories = c);
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
			this.operations.set(operations);
			this.incomesChartData = this.generateChartData(operations, OperationType.Incomes, 'Incomes');
			this.expensesChartData = this.generateChartData(operations, OperationType.Expenses, 'Expenses');
			timer(500).subscribe(() => this.operationsLoading.set(false));
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

	private generateChartData(operations: IOperation[], type: OperationType, label: string): any {
		// Filter operations based on the specified type
		const filteredOperations = operations.filter(op => op.type === type);
	
		// Group operations by category and sum amounts
		const categoryData = filteredOperations.reduce((acc, op) => {
			const categoryName = op.category.name;
			if (!acc[categoryName]) {
				acc[categoryName] = { amount: 0, color: op.category.color };
			}
			acc[categoryName].amount += op.amount;
			return acc;
		}, {} as Record<string, { amount: number; color: string }>);
	
		// Generate labels, data, and colors arrays for the chart
		const labels = Object.keys(categoryData);
		const data = Object.values(categoryData).map(item => item.amount);
		const backgroundColors = Object.values(categoryData).map(item => item.color);
	
		// Return chart configuration
		return {
			type: 'doughnut',
			data: {
				labels,
				datasets: [
					{
						label,
						data,
						backgroundColor: backgroundColors,
						hoverOffset: 4,
					},
				],
			},
			options: {   
				plugins: {
				  legend: {
					display: true,
					position: 'right'
				  }
				}
			  }
		};
	}
}
