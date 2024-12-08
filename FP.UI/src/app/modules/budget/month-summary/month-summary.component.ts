import { Component, computed, inject, Input, Signal, signal, WritableSignal } from '@angular/core';
import { IAccountBalance } from '@fp-core/models';
import { AccountsService } from '@fp-core/services';
import { StateService } from '@fp-core/services/state.service';
import moment from 'moment';

@Component({
	selector: 'fp-month-summary',
	templateUrl: './month-summary.component.html',
	styleUrl: './month-summary.component.scss',
})
export class MonthSummaryComponent {
	private readonly _accountsService: AccountsService = inject(StateService).getService(AccountsService);
	private _selectedDate = moment();
	public accounts = this._accountsService.get();

	@Input()
	public set selectedDate(value: moment.Moment) {
		this._selectedDate = value;
		this.formAccountBalances();
	}

	public get selectedDate(): moment.Moment {
		return this._selectedDate;
	}

	public displayedColumns: any[] = [
		{ width: 25, name: 'name', title: 'Name' },
		{ width: 25, name: 'current', title: 'Current' }, 
		{ width: 25, name: 'difference', title: 'Difference' }, 
		{ width: 25, name: 'end', title: 'End balance' },
	];

	public today = moment();

	public selectedToday = computed(() => {
		return this.today.year() === this.selectedDate.year() && this.today.month() === this.selectedDate.month();
	});

	public accountsResults: WritableSignal<any[]> = signal([]);
	public accountBalances: Signal<IAccountBalance>[] = [];
	public currenciesResults: Signal<{ currency: string, amount: number }[]> = signal([]);
	public filterResults = computed(() => {
		const accounts = this.accounts();
		return this.accounts().map((c: any, ind) => {
			c.ind = ind;
			return c;
		})
		// .filter((c: any, ind) => { 
		// 	const exisitingBalance = this.accountBalances[ind];
		// 	if(!exisitingBalance) {
		// 		return false;
		// 	}
		// 	return exisitingBalance().difference > 0;
		// });
	});
	

	private formAccountBalances(): void {
		const selectedDateString = this.selectedDate.endOf('month').toISOString().split('T')[0];
		this.accountBalances = this.accounts().map(c => 
			this._accountsService.getBalance(c.id, selectedDateString)
		);

		const balances = this.accounts().map((account, ind) =>
			({
				name: account.name,
				currency: account.currency,
				ind
			}));
		this.accountsResults.set(balances);

		this.currenciesResults = computed(() => {
			const accounts = this.accounts();
		
			const totalsByCurrency = accounts.reduce((totals, account, index) => {
				if (!totals[account.currency]) {
					totals[account.currency] = 0;
				}
				if (!this.accountBalances[index]) {
					return totals;
				}
				const balance = this.accountBalances[index]()?.endMonthBalance;
				if (!balance) {
					return totals;
				}
				totals[account.currency] += balance;
		
				return totals;
			}, {} as Record<string, number>);
		
			return Object.entries(totalsByCurrency).map(([currency, amount]) => ({ currency, amount }));
		});
	}
}
