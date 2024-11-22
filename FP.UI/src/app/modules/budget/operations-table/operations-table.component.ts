import { Component, computed, EventEmitter, Input, Output, ViewEncapsulation } from '@angular/core';
import { IOperation, OperationType } from '@fp-core/models';
import moment from 'moment';

@Component({
	selector: 'fp-operations-table',
	templateUrl: './operations-table.component.html',
	styleUrl: './operations-table.component.scss',
	encapsulation: ViewEncapsulation.None
})
export class OperationsTableComponent {
	private _operations!: IOperation[];

	public displayedColumns: any[] = [
		{ width: 5, title: '' }, 
		{ width: 20, title: 'Name' },
		{ width: 20, title: 'Account' }, 
		{ width: 19, title: 'Category' }, 
		{ width: 18, title: 'Amount' },
		{ width: 13, title: 'Date' }, 
		{ width: 5, title: '' }
	];
	public total = 0;
	@Input()
	public currency? = '';

	public currentDate = moment();

	public operationSplitIndex = -1;

	@Input()
	public set operations(operations: IOperation[]) {
		this._operations = operations;
		this.total = this.operations.reduce((sum, operation) => {
			let val = operation.amount;
			if (operation.type === OperationType.Expenses) {
				val = -val;
			}
			return sum + val;
		}, 0);
		this.operationSplitIndex = this.getFutureOperationsIndex();
	}

	public get operations(): IOperation[] {
		return this._operations;
	}

	@Input()
	public loading = false;

	@Output()
	public onDelete = new EventEmitter<IOperation>();

	public onDeleteClick(operation: IOperation): void {
		this.onDelete.emit(operation);
	}

	private getFutureOperationsIndex(): number {
		if (!this.operations?.length) {
			return -1;
		}
		const currentDate = this.currentDate.startOf('day');
		const splitIndex = this.operations.findIndex(op => moment(op.date).isSameOrAfter(currentDate));
		return splitIndex === -1 ? this.operations.length : splitIndex;
	}
}
