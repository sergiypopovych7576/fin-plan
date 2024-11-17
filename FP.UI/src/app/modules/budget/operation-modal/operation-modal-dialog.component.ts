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

	public categoryForm = new FormGroup({
		name: new FormControl(),
		type: new FormControl(1),
		amount: new FormControl(),
		categoryId: new FormControl(),
		date: new FormControl(moment()),
		startDate: new FormControl(moment()),
		endDate: new FormControl(),
		frequency: new FormControl(),
		interval: new FormControl()
	});

	public ngOnInit(): void {
		this.categoryForm.controls.type.valueChanges.subscribe(c => {
			if (c || c=== 0) {
				this.selectedOperationType.set(c);
			}
		});
		let date = moment().year(this.data.year).month(this.data.month).startOf('month');
		const today = moment();
		if(date < today) {
			date = today;
		}
		this.categoryForm.controls.date.setValue(date);
	}

	trackByCategory(index: number, category: any): number {
		return category.id;
	}

	public onYesClick(): void {
		this.dialogRef.close({
			...this.categoryForm.value,
			date: this.categoryForm.value.date?.toISOString(true).split('T')[0],
			startDate: this.categoryForm.value.startDate?.toISOString(true).split('T')[0],
			endDate: this.categoryForm.value.endDate?.toISOString(true).split('T')[0]
		});
	}

	public onNoClick(): void {
		this.dialogRef.close();
	}
}
