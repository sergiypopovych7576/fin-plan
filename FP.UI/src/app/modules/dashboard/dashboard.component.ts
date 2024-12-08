import { Component, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { IMonthSummary, OperationType } from '@fp-core/models';
import { OperationsService } from '@fp-core/services';
import { StateService } from '@fp-core/services/state.service';
import moment from 'moment';

@Component({
	selector: 'fp-dashboard',
	templateUrl: './dashboard.component.html',
	styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
	private readonly _operationsService = inject(StateService).getService(OperationsService);

	public expensesConfig: WritableSignal<any> = signal(null);
	public expenseCategoriesConfig: WritableSignal<any> = signal(null);
	public incomesConfig: WritableSignal<any> = signal(null);
	public incomesCategoriesConfig: WritableSignal<any> = signal(null);

	public ngOnInit(): void {
		this.updateChartData();
	}

	private updateChartData(): void {
		const startDate = moment().subtract(3, 'months').startOf('month').format('YYYY-MM-DD');;
		const endDate = moment().add(3, 'months').endOf('month').toISOString(false).split('T')[0];

		this._operationsService.getSummaryByRange(startDate, endDate).subscribe((summaries: IMonthSummary[]) => {
			const labels = summaries.map(s => `${s.month}/${s.year}`) as any;
			const expenseTemplate = {
				...{
					labels: [],
					datasets: [
						{
							label: 'Expenses',
							data: [],
							borderColor: 'rgb(230, 0, 0)',
							backgroundColor: 'rgb(230, 0, 0)',
							pointStyle: 'circle',
							pointRadius: 5,
							pointHoverRadius: 8,
						},
					],
				}
			} as any;
			expenseTemplate.labels = labels;
			expenseTemplate.datasets[0].data = summaries.map(s => s.totalExpenses);

			const base = {
				type: 'line',
				options: {
					responsive: true,
					plugins: {
						legend: {
							display: false,
							position: 'right'
						}
					}
				},
			};
			this.expensesConfig.set({
				...base,
				data: expenseTemplate,
			});

			const incomeTemplate = {
				...{
					labels: [],
					datasets: [
						{
							label: 'Incomes',
							data: [],
							borderColor: 'rgb(0, 169, 0)',
							backgroundColor: 'rgb(0, 169, 0)',
							pointStyle: 'circle',
							pointRadius: 5,
							pointHoverRadius: 8,
						},
					],
				}
			} as any;
			incomeTemplate.labels = labels;
			incomeTemplate.datasets[0].data = summaries.map(s => s.totalIncomes);

			this.incomesConfig.set({
				...base,
				data: incomeTemplate,
			});


			const expenseCategoryDataMap: { [key: string]: { data: number[], color: string } } = {};
			const incomeCategoryDataMap: { [key: string]: { data: number[], color: string } } = {};

			summaries.forEach(summary => {
				summary.categories.forEach(category => {
					if (category.type === OperationType.Expenses) {
						if (!expenseCategoryDataMap[category.name]) {
							expenseCategoryDataMap[category.name] = {
								data: Array(labels.length).fill(0),
								color: category.color
							};
						}
						const monthIndex = summaries.findIndex(
							s => s.month === summary.month && s.year === summary.year
						);
						expenseCategoryDataMap[category.name].data[monthIndex] = category.amount;
					}
				});
				summary.categories.forEach(category => {
					if (category.type === OperationType.Incomes) {
						if (!incomeCategoryDataMap[category.name]) {
							incomeCategoryDataMap[category.name] = {
								data: Array(labels.length).fill(0),
								color: category.color
							};
						}
						const monthIndex = summaries.findIndex(
							s => s.month === summary.month && s.year === summary.year
						);
						incomeCategoryDataMap[category.name].data[monthIndex] = category.amount;
					}
				});
			});


			const expensesDatasets = Object.keys(expenseCategoryDataMap).map(categoryName => ({
				label: categoryName,
				data: expenseCategoryDataMap[categoryName].data,
				borderColor: expenseCategoryDataMap[categoryName].color,
				backgroundColor: expenseCategoryDataMap[categoryName].color,
				pointRadius: 5,
				pointHoverRadius: 8,
			})).sort((a, b) => {
				const sumA = a.data.reduce((sum, value) => sum + value, 0);
				const sumB = b.data.reduce((sum, value) => sum + value, 0);
				return sumB - sumA;
			}) as any;
			const incomesDatasets = Object.keys(incomeCategoryDataMap).map(categoryName => ({
				label: categoryName,
				data: incomeCategoryDataMap[categoryName].data,
				borderColor: incomeCategoryDataMap[categoryName].color,
				backgroundColor: incomeCategoryDataMap[categoryName].color,
				pointRadius: 5,
				pointHoverRadius: 8,
			})).sort((a, b) => {
				const sumA = a.data.reduce((sum, value) => sum + value, 0);
				const sumB = b.data.reduce((sum, value) => sum + value, 0);
				return sumB - sumA;
			}) as any;
			const baseCat = {
				type: 'bar',
				data: {
					labels: [],
					datasets: []
				},
				options: {
					responsive: true,
					plugins: {
						legend: {
							display: false,
							position: 'right'
						}
					},
					scales: {
						x: {
							stacked: true,
							title: {
								display: false,
								text: 'Month'
							}
						},
						y: {
							stacked: true,
							title: {
								display: false,
								text: 'Expenses'
							}
						}
					}
				}
			};
			this.expenseCategoriesConfig.set({
				...baseCat,
				data: {
					labels,
					datasets: expensesDatasets
				}
			});
			this.incomesCategoriesConfig.set({
				...baseCat,
				data: {
					labels,
					datasets: incomesDatasets
				}
			});
		});
	}
}

