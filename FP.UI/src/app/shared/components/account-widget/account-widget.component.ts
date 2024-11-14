import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IAccount } from '@fp-core/models';

@Component({
    selector: 'fp-account-widget',
    templateUrl: './account-widget.component.html',
    styleUrl: './account-widget.component.scss'
})
export class AccountWidgetComponent {
    @Input()
    public account!: IAccount;

    @Output()
    public onEdit = new EventEmitter<void>();

    @Output()
    public onDelete = new EventEmitter<void>();
}
