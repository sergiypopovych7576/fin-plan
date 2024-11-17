import { Component, inject, OnInit, signal } from '@angular/core';
import { IMonthSummary } from '@fp-core/models';
import { OperationsService } from '@fp-core/services';
import moment from 'moment';

@Component({
	selector: 'fp-dashboard',
	templateUrl: './dashboard.component.html',
	styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
	private readonly _operationsService = inject(OperationsService);

	// Default chart data for expenses
	expensesData = {
		labels: [],
		datasets: [
			{
				label: 'Expenses by month',
				data: [],
				borderColor: 'red',
				backgroundColor: 'rgba(255, 0, 0, 0.5)', // Semi-transparent red
				pointStyle: 'circle',
				pointRadius: 5,
				pointHoverRadius: 8,
			},
		],
	} as any;

	// Default chart data for incomes
	incomesData = {
		labels: [],
		datasets: [
			{
				label: 'Incomes by month',
				data: [],
				borderColor: 'green',
				backgroundColor: 'rgba(0, 255, 0, 0.5)', // Semi-transparent green
				pointStyle: 'circle',
				pointRadius: 5,
				pointHoverRadius: 8,
			},
		],
	} as any;

	// Chart configuration for expenses
	public expensesConfig = signal({
		type: 'line',
		data: this.expensesData,
		options: {
			responsive: true,
			scales: {
				x: {
					title: {
						display: true,
						text: 'Months',
					},
				},
				y: {
					title: {
						display: true,
						text: 'Amount (€)',
					},
				},
			},
		},
	});

	// Chart configuration for incomes
	public incomesConfig = signal({
		type: 'line',
		data: this.incomesData,
		options: {
			responsive: true,
			scales: {
				x: {
					title: {
						display: true,
						text: 'Months',
					},
				},
				y: {
					title: {
						display: true,
						text: 'Amount (€)',
					},
				},
			},
		},
	});

	public ngOnInit(): void {
		this.updateChartData();
	}

	// Fetch and update chart data for the last 6 months
	private updateChartData(): void {
		const endDate = moment().add(3, 'months').endOf('month').toISOString().split('T')[0]; // End of the current month
		const startDate = moment().subtract(3, 'months').startOf('month').toISOString().split('T')[0]; // Start of 6 months ago

		this._operationsService.getSummaryByRange(startDate, endDate).subscribe((summaries: IMonthSummary[]) => {
			// Update labels with the months
			const labels = summaries.map(s => `${s.month}/${s.year}`);
			this.expensesData.labels = labels;
			this.incomesData.labels = labels;

			// Update dataset with total expenses and incomes for each month
			this.expensesData.datasets[0].data = summaries.map(s => s.totalExpenses);
			this.incomesData.datasets[0].data = summaries.map(s => s.totalIncomes);

			// Refresh chart configurations
			this.expensesConfig.set({
				...this.expensesConfig(),
				data: this.expensesData,
			});
			this.incomesConfig.set({
				...this.incomesConfig(),
				data: this.incomesData,
			});
		});
	}
}

