import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IAccount, IOperation } from '@fp-core/models';

@Component({
    selector: 'fp-account-modal-dialog',
    templateUrl: 'account-modal.component.html',
})
export class AccountModalDialogComponent implements OnInit {
    private readonly _cdk = inject(ChangeDetectorRef);
    public readonly dialogRef = inject(MatDialogRef<AccountModalDialogComponent>);
    public readonly data = inject<IAccount>(MAT_DIALOG_DATA);
    public accountForm = new FormGroup({
        id: new FormControl('', [Validators.required]),
        name: new FormControl('', [Validators.required]),
        balance: new FormControl(100, [Validators.required]),
        isDefault: new FormControl(false),
        currency: new FormControl('$', [Validators.required])
    });

    public ngOnInit(): void {
        if(this.data) {
            this.accountForm.setValue(this.data);
        } else {
            this.accountForm.controls.id.setValue(crypto.randomUUID());
        }
    }

    public onYesClick(): void {
        this.accountForm.markAllAsTouched();
        if(this.accountForm.valid) {
            this.dialogRef.close({
                ...this.accountForm.value,
            });
            return;
        }
    }

    public onNoClick(): void {
        this.dialogRef.close();
    }
}
