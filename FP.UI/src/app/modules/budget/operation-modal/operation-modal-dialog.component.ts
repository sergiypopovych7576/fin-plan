import { Component, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ICategory, IOperation } from '@fp-core/models';
import { CategoriesService } from '@fp-core/services';
import moment from 'moment';

@Component({
	selector: 'fp-operation-modal-dialog',
	templateUrl: 'operation-modal-dialog.component.html',
})
export class OperationModalDialogComponent implements OnInit {
	private readonly _categoriesService = inject(CategoriesService);
	public readonly dialogRef = inject(MatDialogRef<OperationModalDialogComponent>);
	public readonly data = inject<IOperation>(MAT_DIALOG_DATA);
	public operation = this.data;
	private _categories: ICategory[] = [];
	public categories: WritableSignal<ICategory[]> = signal([]);

	public categoryForm = new FormGroup({
		name: new FormControl(),
		type: new FormControl(1),
		amount: new FormControl(),
		categoryId: new FormControl(),
		date: new FormControl(moment()),
	});

	public ngOnInit(): void {
		this._categoriesService.get().subscribe((c) => {
			this._categories = c;
			this.categories.set(c.filter(f => f.type === 1));
		});
		this.categoryForm.controls.type.valueChanges.subscribe(c => {
			if (c || c=== 0) {
				this.categories.set(this._categories.filter(f => f.type === c));
			}

		});
	}

	trackByCategory(index: number, category: any): number {
		return category.id;
	}

	public onYesClick(): void {
		this.dialogRef.close({
			...this.categoryForm.value,
			date: this.categoryForm.value.date?.hours(12).minutes(0).second(0)
		});
	}

	public onNoClick(): void {
		this.dialogRef.close();
	}
}
