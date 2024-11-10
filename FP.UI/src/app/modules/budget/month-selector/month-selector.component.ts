import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { IDateChange } from './date-change.model';

@Component({
	selector: 'fp-month-selector',
	templateUrl: './month-selector.component.html',
	styleUrl: './month-selector.component.scss',
})
export class MonthSelectorComponent implements OnInit {
	public selectedDate = new Date();
	public currentMonth = '';
	public monthIsCurrent = true;
	public currentMonthNumber = this.selectedDate.getMonth();
	public currentYear = this.selectedDate.getFullYear();
	public selectedMonthNumber!: number;
	public selectedYear!: number;

	@Output()
	public dateChanged = new EventEmitter<IDateChange>();

	private _formMonth(): void {
		if (
			this.selectedDate.getMonth() === this.currentMonthNumber &&
			this.selectedDate.getFullYear() === this.currentYear
		) {
			this.monthIsCurrent = true;
		}
		this.currentMonth = this.selectedDate.toLocaleString('en-US', {
			month: 'long',
		});
		this.currentMonth =
			this.currentMonth.charAt(0).toUpperCase() +
			this.currentMonth.slice(1) +
			' ' +
			this.selectedDate.getFullYear();
		this.selectedMonthNumber = this.selectedDate.getMonth();
		this.selectedYear = this.selectedDate.getFullYear();
		this.dateChanged.emit({ year: this.selectedYear, month: this.selectedMonthNumber });
	}

	public ngOnInit(): void {
		this._formMonth();
	}

	public onNextMonth(): void {
		this.monthIsCurrent = false;
		this.selectedDate = new Date(
			this.selectedDate.getFullYear(),
			this.selectedDate.getMonth() + 1,
		);
		this._formMonth();
	}

	public onPreviousMonth(): void {
		this.monthIsCurrent = false;
		this.selectedDate = new Date(
			this.selectedDate.getFullYear(),
			this.selectedDate.getMonth() - 1,
		);
		this._formMonth();
	}

	public onCurrentMonth(): void {
		this.selectedDate = new Date();
		this.monthIsCurrent = true;
		this._formMonth();
	}
}
