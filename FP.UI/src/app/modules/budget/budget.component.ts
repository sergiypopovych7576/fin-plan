import { ChangeDetectorRef, Component, computed, inject, OnInit, Signal, signal, WritableSignal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { OperationModalDialogComponent } from './operation-modal';
import { IAccount, IAccountBalance, IOperation, OperationType } from '@fp-core/models';
import { AccountsService, OperationsService } from '@fp-core/services';
import { IDateChange } from './month-selector/date-change.model';
import { timer } from 'rxjs';
import moment from 'moment';
import { StateService } from '@fp-core/services/state.service';

@Component({
	selector: 'fp-budget',
	templateUrl: './budget.component.html',
	styleUrls: ['./budget.component.scss'],
})
export class BudgetComponent implements OnInit {
	// Dependencies
	private readonly _operationsService = inject(StateService).getService(OperationsService);
	private readonly _accountsService = inject(StateService).getService(AccountsService);

	// Signals
	public accounts = this._accountsService.get();
	public operations!: Signal<IOperation[]>;
	public operationsLoading = signal(false);
	public balance: WritableSignal<IAccountBalance | undefined> = signal(undefined);

	// Computed properties
	public accountsResults = computed(() => this.calculateAccountBalances());
	public incomeOperations!: Signal<IOperation[]>;
	public expenseOperations!: Signal<IOperation[]>;
	public incomeTotal!: Signal<number>;
	public expenseTotal!: Signal<number>;
	public defaultAccCurrency = computed(() => this.getDefaultCurrency());
	public selectedToday = computed(() => this.isTodaySelected());
	public defaultAcc = computed(() => this.accounts().find(account => account.isDefault) as IAccount);

	// Date-related signals
	public today = moment();
	public selectedYear = signal(this.today.year());
	public selectedMonthNumber = signal(this.today.month());
	public selectedDate = computed(() => this.getSelectedDate());

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

	private getDefaultCurrency(): string | undefined {
		return this.accounts().find(account => account.isDefault)?.currency;
	}

	private isTodaySelected(): boolean {
		return this.today.year() === this.selectedYear() && this.today.month() === this.selectedMonthNumber();
	}

	private getSelectedDate(): moment.Moment {
		return moment({ year: this.selectedYear(), month: this.selectedMonthNumber() });
	}

	private getSelectedDateString(): string {
		return moment({ year: this.selectedYear(), month: this.selectedMonthNumber() }).endOf('month').toISOString().split('T')[0];
	}

	public loadOperations(): void {
		this.operationsLoading.set(true);
		const date = this.getSelectedDateString();
		this.operations = this._operationsService.get(date);
		this.incomeOperations = computed(() => this.filterOperationsByType(OperationType.Incomes));
		this.expenseOperations = computed(() => this.filterOperationsByType(OperationType.Expenses));
		this.incomeTotal = computed(() => this.calculateTotal(this.incomeOperations()));
		this.expenseTotal = computed(() => this.calculateTotal(this.expenseOperations()));		
		timer(500).subscribe(() => this.operationsLoading.set(false));
		// this._cdkRef.detectChanges();
	}

	public refreshOperations(all = false): void {
		const date = this.getSelectedDateString();
		if (all) {
			this._operationsService.refreshAllOperations();
			this.loadOperations();
		} else {
			this._operationsService.refreshOperations(date);
		}
	}

	public onDateChange(event: IDateChange): void {
		this.selectedMonthNumber.set(event.month);
		this.selectedYear.set(event.year);
		this.loadOperations();
	}

	public onSync(): void {
		this._operationsService.sync().subscribe(() => this.refreshOperations());
	}
}
