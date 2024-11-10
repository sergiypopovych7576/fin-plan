import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { IDateChange } from './date-change.model';
import moment from 'moment';

@Component({
	selector: 'fp-month-selector',
	templateUrl: './month-selector.component.html',
	styleUrls: ['./month-selector.component.scss'],
})
export class MonthSelectorComponent implements OnInit {
	public selectedDate = moment();
	public currentMonth = '';
	public monthIsCurrent = true;
	public currentMonthNumber = moment().month();
	public currentYear = moment().year();
	public selectedMonthNumber!: number;
	public selectedYear!: number;

	@Output()
	public dateChanged = new EventEmitter<IDateChange>();

	private _formMonth(): void {
		this.monthIsCurrent =
			this.selectedDate.month() === this.currentMonthNumber &&
			this.selectedDate.year() === this.currentYear;
		this.currentMonth = this.selectedDate.format('MMMM YYYY');
		this.selectedMonthNumber = this.selectedDate.month();
		this.selectedYear = this.selectedDate.year();
		this.dateChanged.emit({ year: this.selectedYear, month: this.selectedMonthNumber + 1 });
	}

	public ngOnInit(): void {
		this._formMonth();
	}

	public onNextMonth(): void {
		this.monthIsCurrent = false;
		this.selectedDate = this.selectedDate.clone().add(1, 'month');
		this._formMonth();
	}

	public onPreviousMonth(): void {
		this.monthIsCurrent = false;
		this.selectedDate = this.selectedDate.clone().subtract(1, 'month');
		this._formMonth();
	}

	public onCurrentMonth(): void {
		this.selectedDate = moment();
		this.monthIsCurrent = true;
		this._formMonth();
	}
}
