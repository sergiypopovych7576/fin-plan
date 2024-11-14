import { Component, inject, Input, OnInit, signal, WritableSignal } from '@angular/core';
import { IAccount, IAccountBalance, IOperation, OperationType } from '@fp-core/models';
import { AccountsService } from '@fp-core/services';
import moment from 'moment';

@Component({
	selector: 'fp-month-summary',
	templateUrl: './month-summary.component.html',
	styleUrl: './month-summary.component.scss',
})
export class MonthSummaryComponent implements OnInit {
	private readonly _accountsService = inject(AccountsService);
	private _operations?: IOperation[];
	public expenses = 0;
	public incomes = 0;
	public startingBalance = 0;
	public endBalance = 0;
	public today = moment();

	public selectedToday = signal(true);

	public accounts: IAccount[] = [];
	public balance: WritableSignal<null | IAccountBalance> = signal(null);

	@Input()
	public year: any;

	@Input()
	public set month(month: number) {
		this.selectedToday.set(this.today.year() === this.year && this.today.month() + 1 === month);

		this._accountsService.getBalance(moment({ year: this.year, month: month - 1 }).endOf('month').toISOString()).subscribe(c => this.balance.set(c));
	}

	public ngOnInit(): void {
	}
}
