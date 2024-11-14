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

	public selectedYear = 2024;
	public selectedMonthNumber = 11;

	public today = moment();
	public selectedToday = computed(() => this.today.year() === this.selectedYear && this.today.month() + 1 === this.selectedMonthNumber);
	public balance: WritableSignal<IAccountBalance | undefined> = signal(undefined);

	public ngOnInit(): void {
		this.loadOperations();
	}

	public loadOperations(): void {
		this.operations = this._operationsService.getOperationSignal(this.selectedYear, this.selectedMonthNumber);
		this.incomeOperations = computed(() => this.operations().filter(c => c.type === OperationType.Incomes) || []);
		this.expenseOperations = computed(() => this.operations().filter(c => c.type === OperationType.Expenses) || []);
		this._accountsService.getBalance(moment({ year: this.selectedYear, month: this.selectedMonthNumber - 1 }).endOf('month').toISOString()).subscribe(c => this.balance.set(c));
	}

	public refreshOperations(): void {
		this._operationsService.refreshOperations(this.selectedYear, this.selectedMonthNumber);
		this._accountsService.getBalance(moment({ year: this.selectedYear, month: this.selectedMonthNumber - 1 }).endOf('month').toISOString()).subscribe(c => this.balance.set(c));
		timer(500).subscribe(() => this.operationsLoading.set(false));
	}

	public onDateChange(event: IDateChange): void {
		this.selectedMonthNumber = event.month;
		this.selectedYear = event.year;
		this.loadOperations();
	}

	public onAddOperation(): void {
		const dialogRef = this._dialog.open(OperationModalDialogComponent, {});
		dialogRef.afterClosed().subscribe((result) => {
			if (result) {
				this._operationsService.create(result).subscribe(() => {
					this.refreshOperations();
				})
			}
		});
	}

	public onOperationDelete(operation: IOperation): void {
		this._operationsService.delete(operation.id).subscribe(() => this.refreshOperations());
	}
}
