import { Component, inject, OnInit, signal } from '@angular/core';
import { IMonthSummary, OperationType } from '@fp-core/models';
import { OperationsService } from '@fp-core/services';
import moment from 'moment';

@Component({
	selector: 'fp-dashboard',
	templateUrl: './dashboard.component.html',
	styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
	private readonly _operationsService = inject(OperationsService);

	expensesData = {
		labels: [],
		datasets: [
			{
				label: 'Spent',
				data: [],
				borderColor: 'rgb(230, 0, 0)',
				backgroundColor: 'rgb(230, 0, 0)',
				pointStyle: 'circle',
				pointRadius: 5,
				pointHoverRadius: 8,
			},
		],
	} as any;

	public expensesConfig = signal({
		type: 'line',
		data: this.expensesData,
		options: {
			responsive: true,
			plugins: {
				legend: {
					display: false,
					position: 'right'
				}
			}
		},
	});

	public categoriesConfig = signal({
		type: 'line',
		data: {
			labels: [],
			datasets: []
		},
		options: {
			responsive: true,
			plugins: {
				legend: {
					display: false,
					position: 'top'
				}
			},
			scales: {
				x: {
					title: {
						display: true,
						text: 'Month'
					}
				},
				y: {
					title: {
						display: true,
						text: 'Expenses'
					}
				}
			}
		}
	});
	
	public ngOnInit(): void {
		this.updateChartData();
	}
	
	private updateChartData(): void {
		const endDate = moment().add(3, 'months').endOf('month').toISOString().split('T')[0];
		const startDate = moment().subtract(3, 'months').startOf('month').toISOString().split('T')[0];
	
		this._operationsService.getSummaryByRange(startDate, endDate).subscribe((summaries: IMonthSummary[]) => {
			const labels = summaries.map(s => `${s.month}/${s.year}`) as any;
			this.expensesData.labels = labels;
	
			this.expensesData.datasets[0].data = summaries.map(s => s.totalExpenses);
	
			this.expensesConfig.set({
				...this.expensesConfig(),
				data: this.expensesData,
			});
	
			const categoryDataMap: { [key: string]: { data: number[], color: string } } = {};
	
			summaries.forEach(summary => {
				summary.categories.forEach(category => {
					if (category.type === OperationType.Expenses) {
						if (!categoryDataMap[category.name]) {
							categoryDataMap[category.name] = {
								data: Array(labels.length).fill(0),
								color: category.color 
							};
						}
						const monthIndex = summaries.findIndex(
							s => s.month === summary.month && s.year === summary.year
						);
						categoryDataMap[category.name].data[monthIndex] = category.amount;
					}
				});
			});
	

			const datasets = Object.keys(categoryDataMap).map(categoryName => ({
				label: categoryName,
				data: categoryDataMap[categoryName].data,
				borderColor: categoryDataMap[categoryName].color,
				backgroundColor: categoryDataMap[categoryName].color,
				pointRadius: 5,
				pointHoverRadius: 8,
			})) as any;
	
			this.categoriesConfig.set({
				...this.categoriesConfig(),
				data: {
					labels,
					datasets
				}
			});
		});
	}
}

