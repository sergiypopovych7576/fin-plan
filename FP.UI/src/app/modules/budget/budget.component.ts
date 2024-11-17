import { Component, computed, inject, OnInit, Signal, signal, WritableSignal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { OperationModalDialogComponent } from './operation-modal';
import { IAccountBalance, IOperation, OperationType } from '@fp-core/models';
import { AccountsService, OperationsService } from '@fp-core/services';
import { IDateChange } from './month-selector/date-change.model';
import { timer } from 'rxjs';
import moment from 'moment';

@Component({
	selector: 'fp-budget',
	templateUrl: './budget.component.html',
	styleUrl: './budget.component.scss',
})
export class BudgetComponent implements OnInit {
	private readonly _dialog = inject(MatDialog);
	private readonly _operationsService = inject(OperationsService);
	private readonly _accountsService = inject(AccountsService);
	public operations!: WritableSignal<IOperation[]>;
	public operationsLoading = signal(false);
	public incomeOperations = computed(() => this.operations().filter(c => c.type === OperationType.Incomes) || []);
	public expenseOperations = computed(() => this.operations().filter(c => c.type === OperationType.Expenses) || []);

	public today = moment();
	public selectedYear = signal(this.today.year());
	public selectedMonthNumber = signal(this.today.month());
	public defaultAccCurrency = computed(() => this._accountsService.accounts().find(c => c.isDefault)?.currency);

	public selectedToday = computed(() => this.today.year() === this.selectedYear() && this.today.month() === this.selectedMonthNumber());
	public balance: WritableSignal<IAccountBalance | undefined> = signal(undefined);

	public ngOnInit(): void {
		this.loadOperations();

	}

	public loadOperations(): void {
		const date = moment({ year: this.selectedYear(), month: this.selectedMonthNumber() }).endOf('month').toISOString().split('T')[0];
		this.operations = this._operationsService.getOperationSignal(date);
		this.incomeOperations = computed(() => this.operations().filter(c => c.type === OperationType.Incomes) || []);
		this.expenseOperations = computed(() => this.operations().filter(c => c.type === OperationType.Expenses) || []);
		this._accountsService.getBalance(date).subscribe(c => this.balance.set(c));
	}

	public refreshOperations(all = false): void {
		const date = moment({ year: this.selectedYear(), month: this.selectedMonthNumber() }).endOf('month').toISOString().split('T')[0];
		if(all) {
			this._operationsService.refreshAllOperations();
			this.loadOperations();
		}else {
			this._operationsService.refreshOperations(date);
		}
	
		this._accountsService.getBalance(date).subscribe(c => this.balance.set(c));
		timer(500).subscribe(() => this.operationsLoading.set(false));
	}

	public onDateChange(event: IDateChange): void {
		this.selectedMonthNumber.set(event.month);
		this.selectedYear.set(event.year);
		this.loadOperations();
	}

	public onSync() : void {
		this._operationsService.sync().subscribe(() => {
			this.refreshOperations();
		})
	}

	public onAddOperation(): void {
		const dialogRef = this._dialog.open(OperationModalDialogComponent, { data: { month: this.selectedMonthNumber, year: this.selectedYear } });
		dialogRef.afterClosed().subscribe((result) => {
			if (result) {
				const isCreatedScheduled = !!result.interval;
				this._operationsService.create(result).subscribe(() => {
					this.refreshOperations(isCreatedScheduled);
				})
			}
		});
	}

	public onOperationDelete(operation: IOperation): void {
		this._operationsService.delete(operation.id).subscribe(() => this.refreshOperations());
	}
}
