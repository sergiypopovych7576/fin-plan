import { Component, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { IAccount } from '@fp-core/models';
import { AccountsService } from '@fp-core/services';

@Component({
	selector: 'fp-dashboard',
	templateUrl: './dashboard.component.html',
	styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
	public accounts: WritableSignal<IAccount[]> = signal([]);
	private readonly _accService = inject(AccountsService);
	
	public ngOnInit(): void {
		this._accService.get().subscribe(c => this.accounts.set(c));
	}
}
