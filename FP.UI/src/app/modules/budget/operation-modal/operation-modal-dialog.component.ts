import { Component, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ICategory, IOperation } from '@fp-core/models';
import { CategoriesService } from '@fp-core/services';

@Component({
	selector: 'fp-operation-modal-dialog',
	templateUrl: 'operation-modal-dialog.component.html',
})
export class OperationModalDialogComponent implements OnInit {
	private readonly _categoriesService = inject(CategoriesService);
	public readonly dialogRef = inject(MatDialogRef<OperationModalDialogComponent>);
	public readonly data = inject<IOperation>(MAT_DIALOG_DATA);
	public operation = this.data;
	public categories: WritableSignal<ICategory[]> = signal([])

	public categoryForm = new FormGroup({
		name: new FormControl(),
		type: new FormControl(1),
		amount: new FormControl(),
		categoryId: new FormControl(),
		date: new FormControl(),
	});

	public ngOnInit(): void {
		this._categoriesService.get().subscribe((c) => {
			this.categories.set(c);
		});
	}

	trackByCategory(index: number, category: any): number {
		return category.id;
	}

	public onYesClick(): void {
		this.dialogRef.close({
			...this.categoryForm.value
		});
	}

	public onNoClick(): void {
		this.dialogRef.close();
	}
}
