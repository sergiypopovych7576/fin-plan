import { Component, computed, inject, OnInit, Signal, signal, WritableSignal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { OperationModalDialogComponent } from './operation-modal';
import { IAccountBalance, ICategory, IOperation, OperationType } from '@fp-core/models';
import { AccountsService, OperationsService } from '@fp-core/services';
import { IDateChange } from './month-selector/date-change.model';
import { timer } from 'rxjs';
import moment from 'moment';

@Component({
	selector: 'fp-budget',
	templateUrl: './budget.component.html',
	styleUrls: ['./budget.component.scss'],
})
export class BudgetComponent implements OnInit {
	// Dependencies
	private readonly _dialog = inject(MatDialog);
	private readonly _operationsService = inject(OperationsService);
	private readonly _accountsService = inject(AccountsService);

	// Signals
	public accounts = this._accountsService.accounts;
	public operations!: WritableSignal<IOperation[]>;
	public operationsLoading = signal(false);
	public balance: WritableSignal<IAccountBalance | undefined> = signal(undefined);

	// Computed properties
	public accountsResults = computed(() => this.calculateAccountBalances());
	public incomeOperations!: Signal<IOperation[]>;
	public expenseOperations!: Signal<IOperation[]>;
	public incomeTotal!: Signal<number>;
	public expenseTotal!: Signal<number>;
	public categories!: Signal<any>;
	public defaultAccCurrency = computed(() => this.getDefaultCurrency());
	public selectedToday = computed(() => this.isTodaySelected());

	// Date-related signals
	public today = moment();
	public selectedYear = signal(this.today.year());
	public selectedMonthNumber = signal(this.today.month());

	// Lifecycle hooks
	public ngOnInit(): void {
		this.loadOperations();
	}

	// Methods
	private calculateAccountBalances() {
		return Object.entries(
			this.accounts().reduce((acc: any, account) => {
				const balance = account.isDefault ? this.balance()?.endMonthBalance : account.balance;
				acc[account.currency] = (acc[account.currency] || 0) + balance;
				return acc;
			}, {})
		).map(([currency, totalBalance]) => ({ currency, totalBalance: totalBalance as number }));
	}

	private filterOperationsByType(type: OperationType): IOperation[] {
		return this.operations()?.filter(op => op.type === type) || [];
	}

	private calculateTotal(operations: IOperation[]): number {
		return operations.reduce((sum, op) => sum + op.amount, 0);
	}

	private calculateCategories() {
		const filteredOperations = this.expenseOperations();
		const totalAmount = this.calculateTotal(filteredOperations);
		const categoryData = filteredOperations.reduce((acc, op) => {
			const categoryName = op.category.name;
			if (!acc[categoryName]) {
				acc[categoryName] = { name: categoryName, amount: 0, color: op.category.color, percentage: 0 };
			}
			acc[categoryName].amount += op.amount;
			acc[categoryName].percentage = (acc[categoryName].amount / totalAmount) * 100;
			return acc;
		}, {} as Record<string, { name: string; amount: number; color: string; percentage: number }>);
		return Object.values(categoryData).sort((a, b) => b.percentage - a.percentage);
	}

	private getDefaultCurrency(): string | undefined {
		return this.accounts().find(account => account.isDefault)?.currency;
	}

	private isTodaySelected(): boolean {
		return this.today.year() === this.selectedYear() && this.today.month() === this.selectedMonthNumber();
	}

	private getSelectedDate(): string {
		return moment({ year: this.selectedYear(), month: this.selectedMonthNumber() }).endOf('month').toISOString().split('T')[0];
	}

	public loadOperations(): void {
		const date = this.getSelectedDate();
		this.operations = this._operationsService.getOperationSignal(date);
		this.incomeOperations = computed(() => this.filterOperationsByType(OperationType.Incomes));
		this.expenseOperations = computed(() => this.filterOperationsByType(OperationType.Expenses));
		this.incomeTotal = computed(() => this.calculateTotal(this.incomeOperations()));
		this.expenseTotal = computed(() => this.calculateTotal(this.expenseOperations()));
		this.categories = computed(() => this.calculateCategories());
		this._accountsService.getBalance(date).subscribe(balance => this.balance.set(balance));
	}

	public refreshOperations(all = false): void {
		const date = this.getSelectedDate();
		if (all) {
			this._operationsService.refreshAllOperations();
			this.loadOperations();
		} else {
			this._operationsService.refreshOperations(date);
		}
		this._accountsService.getBalance(date).subscribe(balance => this.balance.set(balance));
		timer(500).subscribe(() => this.operationsLoading.set(false));
	}

	public onDateChange(event: IDateChange): void {
		this.selectedMonthNumber.set(event.month);
		this.selectedYear.set(event.year);
		this.loadOperations();
	}

	public onSync(): void {
		this._operationsService.sync().subscribe(() => this.refreshOperations());
	}

	public onAddOperation(): void {
		const dialogRef = this._dialog.open(OperationModalDialogComponent, {
			data: { month: this.selectedMonthNumber, year: this.selectedYear }
		});
		dialogRef.afterClosed().subscribe(result => {
			if (result) {
				this._operationsService.create(result).subscribe(() => {
					this.refreshOperations(!!result.interval);
				});
			}
		});
	}

	public onOperationDelete(operation: IOperation): void {
		if (!operation.scheduledOperationId) {
			this._operationsService.delete(operation.id).subscribe(() => this.refreshOperations());
		}
	}
}
