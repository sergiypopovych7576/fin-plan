import { Component, computed, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ICategory, IOperation, OperationType } from '@fp-core/models';
import { CategoriesService } from '@fp-core/services';
import moment from 'moment';

@Component({
	selector: 'fp-operation-modal-dialog',
	templateUrl: 'operation-modal-dialog.component.html',
})
export class OperationModalDialogComponent implements OnInit {
	private readonly _categoriesService = inject(CategoriesService);
	public readonly dialogRef = inject(MatDialogRef<OperationModalDialogComponent>);
	public readonly data = inject< { month: number, year: number}>(MAT_DIALOG_DATA);
	public operation = this.data;
	public categories: WritableSignal<ICategory[]> = this._categoriesService.categories;
	public selectedOperationType = signal(OperationType.Expenses);
	public filteredCategories = computed(() => {
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
		interval: new FormControl(1)
	});

	public ngOnInit(): void {
		this.operationForm.controls.type.valueChanges.subscribe(c => {
			if (c || c=== 0) {
				this.selectedOperationType.set(c);
			}
		});
		let date = moment().year(this.data.year).month(this.data.month).startOf('month');
		const today = moment();
		if(date < today) {
			date = today;
		}
		this.operationForm.controls.date.setValue(date);
	}

	trackByCategory(index: number, category: any): number {
		return category.id;
	}

	public onYesClick(): void {
		const isScheduled = this.operationForm.value.isScheduled;
		let operation = {
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

		this.dialogRef.close(operation);
	}

	public onNoClick(): void {
		this.dialogRef.close();
	}
}
