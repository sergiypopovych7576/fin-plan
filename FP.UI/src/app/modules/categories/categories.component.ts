import { ChangeDetectionStrategy, Component, computed, inject, Signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CategoriesService } from '@fp-core/services';
import { CategoryModalDialogComponent } from './category-modal';
import { ICategory } from '@fp-core/models';
import { ConfirmationModalDialogComponent } from '@fp-shared/components';
import { StateService } from '@fp-core/services/state.service';

@Component({
    selector: 'fp-categories',
    templateUrl: './categories.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class CategoriesComponent {
    private readonly _categoriesService = inject(StateService).getService(CategoriesService);
    private readonly _dialog = inject(MatDialog);
    public categories: Signal<ICategory[]> = this._categoriesService.get();

    public columns = [
        {
            name: 'indicator',
            title: '',
            width: 8
        },
        {
            name: 'icon',
            title: 'Icon',
            width: 8
        },
        {
            name: 'name',
            title: 'Name',
            width: 40
        },
        {
            name: 'type',
            title: 'Type',
            width: 39
        },
        {
            name: 'actions',
            title: '',
            width: 5
        }
    ]

    public data = [
        {
            color: 'red',
            iconName: 'account',
            name: 'First item'
        }
    ]

    public onAddCategory(): void {
        const dialogRef = this._dialog.open(CategoryModalDialogComponent, {
            data: {}
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this._categoriesService.create(result).subscribe();
            }
        });
    }

    public onEditCategory(category: ICategory): void {
        const dialogRef = this._dialog.open(CategoryModalDialogComponent, {
            data: { category }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this._categoriesService.update(result).subscribe();
            }
        });
    }

    public onDeleteCategory(category: ICategory): void {
        const dialogRef = this._dialog.open(ConfirmationModalDialogComponent, { data: 'Are you sure you want to delete this category and linked operations?' });
        dialogRef.afterClosed().subscribe((result) => {
            if (result) {
                this._categoriesService.delete(category.id).subscribe();
            }
        });
    }
}

