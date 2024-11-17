import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
    selector: 'fp-confirmation-modal-dialog',
    templateUrl: 'confirmation-modal.component.html',
})
export class ConfirmationModalDialogComponent {
    public readonly dialogRef = inject(MatDialogRef<ConfirmationModalDialogComponent>);
    public readonly title = inject<string>(MAT_DIALOG_DATA);

    public onYesClick(): void {
        this.dialogRef.close(true);
    }

    public onDiscard(): void {
        this.dialogRef.close();
    }
}
