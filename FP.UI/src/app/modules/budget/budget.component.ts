import { Component, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { OperationModalDialogComponent } from './operation-modal';
import { ICategory, IOperation, OperationType } from '@fp-core/models';
import { AccountsService, CategoriesService, OperationsService } from '@fp-core/services';
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
	public operationsLoading = signal(false);
	private readonly _categoriesService = inject(CategoriesService);
	public expensesChartData: unknown;
	public incomesChartData: unknown;
	private readonly _dialog = inject(MatDialog);
	private readonly _operationsService = inject(OperationsService);
	public incomeCategories = signal([] as any[]);
	public expenseCategories= signal([] as any[])
	public categories: WritableSignal<ICategory[]> = this._categoriesService.categories;

	public selectedYear = 2024;
	public selectedMonthNumber = 11;

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
		//this.operations = this._operationsService.getOperationSignal(this.selectedYear, this.selectedMonthNumber);

		this._operationsService.get(this.selectedYear, this.selectedMonthNumber).subscribe((operations) => {
			this.operations.set(operations);
			this.incomesChartData = this.generateChartData(operations, OperationType.Incomes, 'Incomes');
			this.expensesChartData = this.generateChartData(operations, OperationType.Expenses, 'Expenses');
			timer(500).subscribe(() => this.operationsLoading.set(false));
		});
	}

	public onDateChange(event: IDateChange): void {
		this.selectedMonthNumber = event.month;
		this.selectedYear = event.year;
		this.refreshOperations();
	}

	public onOperationDelete(operation: IOperation): void {
		this.operationsLoading.set(true);
		this._operationsService.delete(operation.id).subscribe(() => this.refreshOperations());
	}

	private generateChartData(operations: IOperation[], type: OperationType, label: string): any {
		// Filter operations based on the specified type
		const filteredOperations = operations.filter(op => op.type === type);
	  
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
	  
		if (type === OperationType.Expenses) {
		  this.expenseCategories.set(arr as any);
		} else {
		  this.incomeCategories.set(arr as any);
		}
	  
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
				display: false,
				position: 'right'
			  }
			}
		  }
		};
	  }
	  
	  
}
