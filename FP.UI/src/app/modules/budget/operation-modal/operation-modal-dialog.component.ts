import { Component, computed, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ICategory, IOperation, OperationType } from '@fp-core/models';
import { AccountsService, CategoriesService } from '@fp-core/services';
import moment from 'moment';

@Component({
	selector: 'fp-operation-modal-dialog',
	templateUrl: 'operation-modal-dialog.component.html',
})
export class OperationModalDialogComponent implements OnInit {
	private readonly _categoriesService = inject(CategoriesService);
	private readonly _accountsService = inject(AccountsService);
	public readonly dialogRef = inject(MatDialogRef<OperationModalDialogComponent>);
	public readonly data = inject< { month: number, year: number}>(MAT_DIALOG_DATA);
	public operation = this.data;
	public accounts = this._accountsService.accounts;
	public categories: WritableSignal<ICategory[]> = this._categoriesService.categories;
	public selectedOperationType = signal(OperationType.Expenses);
	public filteredCategories = computed(() => {
		if(this.selectedOperationType() === OperationType.Transfer) {
			return this.categories();
		}
		return this.categories().filter(c => c.type === this.selectedOperationType())
	});

	public operationForm = new FormGroup({
		name: new FormControl(),
		type: new FormControl(1),
		amount: new FormControl(),
		categoryId: new FormControl(),
		date: new FormControl(moment()),
		isScheduled: new FormControl(false),
		startDate: new FormControl(moment()),
		endDate: new FormControl(),
		frequency: new FormControl(2),
		interval: new FormControl(1),
		sourceAccountId: new FormControl(),
		targetAccountId: new FormControl(),
	});

	public ngOnInit(): void {
		this.operationForm.controls.type.valueChanges.subscribe(c => {
			if (c || c=== 0) {
				this.selectedOperationType.set(c);
			}
		});
		let date = moment().year(this.data.year).month(this.data.month).startOf('month');
		this.operationForm.controls.date.setValue(date);
		this.operationForm.controls.startDate.setValue(date);
		const defaultAcc = this.accounts().find(a => a.isDefault);
		this.operationForm.controls.sourceAccountId.setValue(defaultAcc?.id);
	}

	trackByCategory(index: number, category: any): number {
		return category?.id;
	}

	public onYesClick(): void {
		const isScheduled = this.operationForm.value.isScheduled;
		const operation = {
			...this.operationForm.value,
			date: this.operationForm.value.date?.toISOString(true)?.split('T')[0],
			startDate: this.operationForm.value.startDate?.toISOString(true)?.split('T')[0],
			endDate: this.operationForm.value.endDate?.toISOString(true)?.split('T')[0]
		}
		if(isScheduled) {
			operation.date = undefined;
			operation.isScheduled = undefined;
		} else {
			operation.startDate = undefined;
			operation.interval = undefined;
			operation.frequency = undefined;
			operation.isScheduled = undefined;
		}
		if(operation.type === OperationType.Incomes) {
			operation.targetAccountId = this.operationForm.value.sourceAccountId;
			operation.sourceAccountId = undefined; 
		}
		if(operation.type === OperationType.Expenses) {
			operation.sourceAccountId = this.operationForm.value.sourceAccountId;
			operation.targetAccountId = undefined; 
		}
		this.dialogRef.close(operation);
	}

	public onNoClick(): void {
		this.dialogRef.close();
	}
}
