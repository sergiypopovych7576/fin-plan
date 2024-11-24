import { Component, inject } from '@angular/core';
import { CategoriesService } from '@fp-core/services';

@Component({
	selector: 'fp-categories',
	templateUrl: './categories.component.html',
})
export class CategoriesComponent {
    private readonly _categoriesService = inject(CategoriesService);
    public categories = this._categoriesService.categories;

    public columns = [
        {
            name: 'indicator',
            title: '',
            width: 8
        },
        {
            name: 'icon',
            title: 'Icon',
            width: 16
        },
        {
            name: 'name',
            title: 'Name',
            width: 40
        },
        {
            name: 'type',
            title: 'Type',
            width: 31
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
}

