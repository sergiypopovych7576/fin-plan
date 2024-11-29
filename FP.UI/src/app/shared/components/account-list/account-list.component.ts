import { Component, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { IAccount } from '@fp-core/models';
import { AccountsService } from '@fp-core/services';
import { StateService } from '@fp-core/services/state.service';
import { AccountModalDialogComponent, ConfirmationModalDialogComponent } from '@fp-shared/components';

@Component({
	selector: 'fp-account-list',
	templateUrl: './account-list.component.html',
	styleUrls: ['./account-list.component.scss']
})
export class AccountListComponent {
	private readonly _dialog = inject(MatDialog);
	private readonly _accountsService = inject(StateService).getService(AccountsService);

	public accounts = this._accountsService.get();

	public onAddAccount(): void {
		const dialogRef = this._dialog.open(AccountModalDialogComponent, {});
		dialogRef.afterClosed().subscribe((result) => {
			if (result) {
				result.id = crypto.randomUUID();
				this._accountsService.create(result).subscribe();
			}
		});
	}

	public onEditAccount(account?: IAccount): void {
		const dialogRef = this._dialog.open(AccountModalDialogComponent, {
			data: account
		});
		dialogRef.afterClosed().subscribe((result) => {
			if (result) {
				this._accountsService.update(result).subscribe();
			}
		});
	}

	public onDeleteAccount(account: IAccount): void {
		const dialogRef = this._dialog.open(ConfirmationModalDialogComponent, { data: 'Are you sure you want to delete this account?'});
		dialogRef.afterClosed().subscribe((result) => {
			if (result) {
				this._accountsService.delete(account.id).subscribe();
			}
		});
	}
}
