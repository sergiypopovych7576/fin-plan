import { Component, computed, EventEmitter, inject, Input, Output, Signal, ViewEncapsulation } from '@angular/core';
import { IOperation, OperationType } from '@fp-core/models';
import { OperationsService } from '@fp-core/services';
import { StateService } from '@fp-core/services/state.service';
import moment from 'moment';
import { OperationModalDialogComponent } from '../operation-modal';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationModalDialogComponent } from '@fp-shared/components';

@Component({
	selector: 'fp-operations-table',
	templateUrl: './operations-table.component.html',
	styleUrl: './operations-table.component.scss',
	encapsulation: ViewEncapsulation.None
})
export class OperationsTableComponent {
	private readonly _dialog = inject(MatDialog);
	private readonly _operationsService = inject(StateService).getService(OperationsService);
	private _operations!: Signal<IOperation[]>;

	public displayedColumns: any[] = [
		{ width: 5, name: 'indicator', title: '' }, 
		{ width: 20, name: 'name', title: 'Name' },
		{ width: 20, name: 'account', title: 'Account' }, 
		{ width: 19, name: 'category', title: 'Category' }, 
		{ width: 18, name: 'amount', title: 'Amount' },
		{ width: 13, name: 'date', title: 'Date' }, 
		{ width: 5, name: 'actions', title: '' }
	];
	public total = 0;
	@Input()
	public currency? = '';

	public currentDate = moment();

	public operationSplitIndex = -1;

	@Input()
	public set operations(operations: Signal<IOperation[]>) {
		this._operations = operations;
		this.total = this._operations().reduce((sum, operation) => {
			let val = operation.amount;
			if (operation.type === OperationType.Expenses) {
				val = -val;
			}
			return sum + val;
		}, 0);
		this.operationSplitIndex = this.getFutureOperationsIndex();
	}

	public get operations(): Signal<IOperation[]> {
		return this._operations;
	}

	@Input()
	public loading = false;

	public onEditClick(operation: IOperation): void {
		const dialogRef = this._dialog.open(OperationModalDialogComponent, {
			data: { month: moment().month(), year: moment().year(), operation }
			//data: { month: this.selectedMonthNumber(), year: this.selectedYear() }
		});
		dialogRef.afterClosed().subscribe(result => {
			if (result) {
				this._operationsService.update(result).subscribe();
			}
		});
	}

	public onAddOperation(): void {
		const dialogRef = this._dialog.open(OperationModalDialogComponent, {
			data: { month: moment().month(), year: moment().year() }
			//data: { month: this.selectedMonthNumber(), year: this.selectedYear() }
		});
		dialogRef.afterClosed().subscribe(result => {
			if (result) {
				this._operationsService.create(result).subscribe();
			}
		});
	}
	
	public onOperationDelete(operation: IOperation): void {
		let message = 'Are you sure you want to delete operation?';
		if(operation.scheduledOperationId) {
			message += '\n Scheduled operations will remove all associated operations';
		}
		const dialogRef = this._dialog.open(ConfirmationModalDialogComponent, { data: message });
        dialogRef.afterClosed().subscribe((result) => {
            if (result) {
                this._operationsService.delete(operation.scheduledOperationId ? operation.scheduledOperationId : operation.id).subscribe();
            }
        });
	}

	private getFutureOperationsIndex(): number {
		if (!this.operations?.length) {
			return -1;
		}
		const currentDate = this.currentDate.startOf('day');
		const splitIndex = this.operations().findIndex(op => moment(op.date).isSameOrAfter(currentDate));
		return splitIndex === -1 ? this.operations.length : splitIndex;
	}
}
