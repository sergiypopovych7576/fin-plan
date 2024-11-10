import { Component, EventEmitter, Input, Output, ViewEncapsulation } from '@angular/core';
import { IOperation, OperationType } from '@fp-core/models';

@Component({
	selector: 'fp-operations-table',
	templateUrl: './operations-table.component.html',
	styleUrl: './operations-table.component.scss',
	encapsulation: ViewEncapsulation.None
})
export class OperationsTableComponent {
	private _operations!: IOperation[];

	public displayedColumns: string[] = ['color', 'name', 'category', 'amount', 'date', 'actions'];
	public total = 0;

	@Input()
	public set operations(operations: IOperation[]) {
		this._operations = operations;
		this.total = this.operations.reduce((sum, operation) => { 
			let val = operation.amount;
			if(operation.type === OperationType.Expenses) {
				val = -val;
			}
			return sum + val; 
		}, 0);
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
}
