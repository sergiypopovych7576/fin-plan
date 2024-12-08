import { Component, computed, EventEmitter, OnInit, Output, signal } from '@angular/core';
import { IDateChange } from './date-change.model';
import moment from 'moment';
import { debounceTime, Subject } from 'rxjs';

@Component({
	selector: 'fp-month-selector',
	templateUrl: './month-selector.component.html',
	styleUrls: ['./month-selector.component.scss'],
})
export class MonthSelectorComponent implements OnInit {
	public currentMonthNumber = moment().month();
	public currentYear = moment().year();
	public selectedDate = signal(moment());
	public selectedDateTitle = computed(() => this.selectedDate().format('MMMM YYYY'));
	public selectedMonthNumber = computed(() => this.selectedDate().month());
	public selectedYear = computed(() => this.selectedDate().year());
	public monthIsCurrent = computed(() => {
		return this.selectedDate().month() === this.currentMonthNumber &&
			this.selectedDate().year() === this.currentYear
	});

	@Output()
	public dateChanged = new EventEmitter<IDateChange>();
	
	private _dateChangeSubject = new Subject<IDateChange>();

	private _emitDateChange(): void {
		this._dateChangeSubject.next({ year: this.selectedYear(), month: this.selectedMonthNumber() });
	}

	public 	ngOnInit(): void {
		this._dateChangeSubject
		.pipe(debounceTime(200))
		.subscribe((dateChange) => this.dateChanged.emit(dateChange));
	}

	public onNextMonth(): void {
		this.selectedDate.set(this.selectedDate().clone().add(1, 'month'));
		this._emitDateChange();
	}

	public onPreviousMonth(): void {
		this.selectedDate.set(this.selectedDate().clone().subtract(1, 'month'));
		this._emitDateChange();
	}

	public onCurrentMonth(): void {
		this.selectedDate.set(moment());
		this._emitDateChange();
	}
}
