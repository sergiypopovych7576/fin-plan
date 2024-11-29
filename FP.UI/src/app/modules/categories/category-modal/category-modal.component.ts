import { Component, inject, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ICategory } from '@fp-core/models';

@Component({
    selector: 'fp-category-modal-dialog',
    templateUrl: 'category-modal.component.html',
})
export class CategoryModalDialogComponent implements OnInit {
    public readonly dialogRef = inject(MatDialogRef<CategoryModalDialogComponent>);
    public readonly data = inject<{ category: ICategory }>(MAT_DIALOG_DATA);

    public categoryForm = new FormGroup({
        id: new FormControl(),
        name: new FormControl(),
        type: new FormControl(1),
        color: new FormControl('#00ff00'),
        iconName: new FormControl('category'),
    });

    public ngOnInit(): void {
        if (this.data.category) {
            this.categoryForm.setValue(this.data.category);
        }
    }

    public onYesClick(): void {
        this.dialogRef.close(this.categoryForm.value);
    }

    public onNoClick(): void {
        this.dialogRef.close();
    }
}
