import { Component, OnInit } from '@angular/core';

@Component({
	selector: 'fp-month-selector',
	templateUrl: './month-selector.component.html',
	styleUrl: './month-selector.component.scss',
})
export class MonthSelectorComponent implements OnInit {
	public currentDate = new Date();
	public currentMonth = '';
	public monthIsCurrent = true;
	public currentMonthNumber = this.currentDate.getMonth();
	public currentYear = this.currentDate.getFullYear();

	private _formMonth(): void {
		if (
			this.currentDate.getMonth() === this.currentMonthNumber &&
			this.currentDate.getFullYear() === this.currentYear
		) {
			this.monthIsCurrent = true;
		}
		this.currentMonth = this.currentDate.toLocaleString('en-US', {
			month: 'long',
		});
		this.currentMonth =
			this.currentMonth.charAt(0).toUpperCase() +
			this.currentMonth.slice(1) +
			' ' +
			this.currentDate.getFullYear();
	}

	public ngOnInit(): void {
		this._formMonth();
	}

	public onNextMonth(): void {
		this.monthIsCurrent = false;
		this.currentDate = new Date(
			this.currentDate.getFullYear(),
			this.currentDate.getMonth() + 1,
		);
		this._formMonth();
	}

	public onPreviousMonth(): void {
		this.monthIsCurrent = false;
		this.currentDate = new Date(
			this.currentDate.getFullYear(),
			this.currentDate.getMonth() - 1,
		);
		this._formMonth();
	}

	public onCurrentMonth(): void {
		this.currentDate = new Date();
		this.monthIsCurrent = true;
		this._formMonth();
	}
}
