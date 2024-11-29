import { Component, computed, inject } from '@angular/core';
import { IAccount } from '@fp-core/models';
import { AccountsService } from '@fp-core/services';
import { StateService } from '@fp-core/services/state.service';

@Component({
	selector: 'fp-accounts',
	templateUrl: './accounts.component.html',
})
export class AccountsComponent {
	public accounts = inject(StateService).getService(AccountsService).get();
	public accountTotals = computed(() => {
		const accounts = this.accounts();

		const totalsByCurrency = accounts.reduce((totals, account) => {
			if (!totals[account.currency]) {
				totals[account.currency] = 0;
			}
			totals[account.currency] += account.balance;
			return totals;
		}, {} as Record<string, number>);

		return Object.entries(totalsByCurrency).map(([currency, balance]) => ({
			name: `Total (${currency})`,
			balance,
			currency,
		})) as IAccount[];
	});
}

