import { Component, inject } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IOperation } from '@fp-core/models';

@Component({
	selector: 'fp-operation-modal-dialog',
	templateUrl: 'operation-modal-dialog.component.html',
})
export class OperationModalDialogComponent {
	public readonly dialogRef = inject(MatDialogRef<OperationModalDialogComponent>);
	public readonly data = inject<IOperation>(MAT_DIALOG_DATA);
	public operation = this.data;

	public nameControl = new FormControl('name')

	public onYesClick(): void {
		this.dialogRef.close({ ...this.operation, name: this.nameControl.value});
	}

	public onNoClick(): void {
		this.dialogRef.close();
	}
}
