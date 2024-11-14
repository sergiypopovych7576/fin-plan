import { Component, Input, signal, WritableSignal } from '@angular/core';
import { IAccountBalance } from '@fp-core/models';

@Component({
	selector: 'fp-month-summary',
	templateUrl: './month-summary.component.html',
	styleUrl: './month-summary.component.scss',
})
export class MonthSummaryComponent {
	@Input()
	public selectedToday = true;
	
	@Input()
	public balance?: IAccountBalance;
}
