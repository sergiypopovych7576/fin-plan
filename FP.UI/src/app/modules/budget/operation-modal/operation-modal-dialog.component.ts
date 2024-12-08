import { Component, computed, inject, OnInit, Signal, signal, WritableSignal } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ICategory, IOperation, OperationType } from '@fp-core/models';
import { AccountsService, CategoriesService } from '@fp-core/services';
import { StateService } from '@fp-core/services/state.service';
import moment from 'moment';

@Component({
	selector: 'fp-operation-modal-dialog',
	templateUrl: 'operation-modal-dialog.component.html',
})
export class OperationModalDialogComponent implements OnInit {
	private readonly _categoriesService = inject(StateService).getService(CategoriesService);
	private readonly _accountsService = inject(StateService).getService(AccountsService);
	public readonly dialogRef = inject(MatDialogRef<OperationModalDialogComponent>);
	public readonly data = inject<{ month: number, year: number, operation: IOperation }>(MAT_DIALOG_DATA);
	public operation = this.data;
	public accounts = this._accountsService.get();
	public categories: Signal<ICategory[]> = this._categoriesService.get();
	public selectedOperationType = signal(OperationType.Expenses);
	public filteredCategories = computed(() => {
		return this.categories().filter(c => c.type === this.selectedOperationType())
	});
	public isEdit = false;

	public operationForm = new FormGroup({
		name: new FormControl(),
		amount: new FormControl(),
		date: new FormControl(moment()),
		type: new FormControl(1),
		categoryId: new FormControl(),
		endDate: new FormControl(),
		frequency: new FormControl(-1),
		interval: new FormControl(1),
		sourceAccountId: new FormControl(),
		targetAccountId: new FormControl(),
	});

	public ngOnInit(): void {
		this.operationForm.controls.type.valueChanges.subscribe(c => {
			if (c || c === 0) {
				this.selectedOperationType.set(c);
			}
		});
		if (this.data.operation) {
			this.isEdit = true;
			this.operationForm.controls.frequency.disable();
			this.operationForm.setValue({
				name: this.data.operation.name,
				amount: this.data.operation.amount,
				date: moment(this.data.operation.date),
				type: this.data.operation.type,
				categoryId: this.data.operation.category?.id,
				sourceAccountId: this.data.operation.sourceAccountId,
				targetAccountId: this.data.operation.targetAccountId,
				endDate: null,
				frequency: -1,
				interval: 1
			});
		}
		else {
			let date = moment().year(this.data.year).month(this.data.month).startOf('month');
			this.operationForm.controls.date.setValue(date);
			const defaultAcc = this.accounts().find(a => a.isDefault);
			this.operationForm.controls.sourceAccountId.setValue(defaultAcc?.id);
		}
	}

	trackByCategory(index: number, category: any): number {
		return category?.id;
	}

	public onYesClick(): void {
		const isScheduled = this.operationForm.value.frequency !== -1;
		const operation = {
			...this.data.operation,
			...this.operationForm.value,
			date: this.operationForm.value.date?.toISOString(true)?.split('T')[0],
			startDate: this.operationForm.value.date?.toISOString(true)?.split('T')[0],
			endDate: this.operationForm.value.endDate?.toISOString(true)?.split('T')[0]
		}
		if (isScheduled) {
			operation.startDate = operation.date;
			operation.date = undefined;
		} else {
			operation.startDate = undefined;
			operation.endDate = undefined;
			operation.interval = undefined;
			operation.frequency = undefined;
		}
		if (operation.type === OperationType.Incomes) {
			operation.targetAccountId = this.operationForm.value.sourceAccountId;
			operation.sourceAccountId = undefined;
		}
		if (operation.type === OperationType.Expenses) {
			operation.sourceAccountId = this.operationForm.value.sourceAccountId;
			operation.targetAccountId = undefined;
		}
		this.dialogRef.close(operation);
	}

	public onNoClick(): void {
		this.dialogRef.close();
	}
}
