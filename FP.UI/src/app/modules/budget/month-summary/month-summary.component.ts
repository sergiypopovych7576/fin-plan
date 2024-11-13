import { Component, inject, Input, OnInit } from '@angular/core';
import { IAccount, IOperation, OperationType } from '@fp-core/models';
import { AccountsService } from '@fp-core/services';

@Component({
	selector: 'fp-month-summary',
	templateUrl: './month-summary.component.html',
	styleUrl: './month-summary.component.scss',
})
export class MonthSummaryComponent implements OnInit {
	private _operations?: IOperation[];
	public expenses = 0;
	public incomes = 0;
	public startingBalance = 0;

	public accounts: IAccount[] = [];

	@Input()
	public set operations(value: IOperation[]) {
		this._operations = value;
		this.expenses = this.operations.filter(c => c.type === OperationType.Expenses).reduce((sum, operation) => sum + operation.amount, 0);
		this.incomes = this.operations.filter(c => c.type === OperationType.Incomes).reduce((sum, operation) => sum + operation.amount, 0);
	}

	public get operations(): IOperation[] {
		return this._operations!;
	}

	private readonly _accountsService = inject(AccountsService);

	public ngOnInit(): void {
		this._accountsService.get().subscribe(c => { 
			this.accounts = c;
			const defaultAccount = c.find(a => a.isDefault) as IAccount;
			this.startingBalance = defaultAccount.balance;
		});
	}
}
