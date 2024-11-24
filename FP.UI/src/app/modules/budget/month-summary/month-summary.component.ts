import { Component, computed, inject, Input, Signal, signal, WritableSignal } from '@angular/core';
import { IAccountBalance } from '@fp-core/models';
import { AccountsService } from '@fp-core/services';
import moment from 'moment';
import { forkJoin } from 'rxjs';

@Component({
	selector: 'fp-month-summary',
	templateUrl: './month-summary.component.html',
	styleUrl: './month-summary.component.scss',
})
export class MonthSummaryComponent {
	private readonly _accountsService = inject(AccountsService);
	private _selectedDate = moment();
	public accounts = this._accountsService.accounts;

	@Input()
	public set selectedDate(value: moment.Moment) {
		this._selectedDate = value;
		this.formAccountBalances();
	}

	public get selectedDate(): moment.Moment {
		return this._selectedDate;
	}

	public today = moment();

	public selectedToday = computed(() => {
		return this.today.year() === this.selectedDate.year() && this.today.month() === this.selectedDate.month();
	});

	public accountsResults: WritableSignal<IAccountBalance[]> = signal([]);
	public currenciesResults: Signal<{ currency: string, amount: number }[]> = computed(() => {
		const accounts = this.accounts();
		const balances = this.accountsResults();
	
		const totalsByCurrency = accounts.reduce((totals, account, index) => {
			const balance = balances[index]?.endMonthBalance ?? account.balance; // Fallback to account balance if endMonthBalance is missing
			if (!totals[account.currency]) {
				totals[account.currency] = 0;
			}
			totals[account.currency] += balance;
	
			return totals;
		}, {} as Record<string, number>);
	
		return Object.entries(totalsByCurrency).map(([currency, amount]) => ({ currency, amount }));
	});
	

	private formAccountBalances(): void {
		const selectedDateString = this.selectedDate.endOf('month').toISOString().split('T')[0];

		const balanceRequests = this.accounts().map((account) =>
			this._accountsService.getBalance(account.id, selectedDateString)
		);

		forkJoin(balanceRequests).subscribe((balances) => {
			this.accountsResults.set(balances);
		});
	}
}
