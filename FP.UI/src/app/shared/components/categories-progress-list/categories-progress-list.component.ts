import { Component, Input, WritableSignal } from '@angular/core';

@Component({
    selector: 'fp-categories-progress-list',
    templateUrl: './categories-progress-list.component.html',
})
export class CategoriesProgressListComponent {
    @Input()
    public categories!: WritableSignal<any[]>;
}
