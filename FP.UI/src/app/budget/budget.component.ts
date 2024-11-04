import { Component, OnInit } from '@angular/core';
import { Chart, registerables } from 'chart.js';

@Component({
  selector: 'fp-budget',
  templateUrl: './budget.component.html',
  styleUrl: './budget.component.scss'
})
export class BudgetComponent implements OnInit{

  ngOnInit(): void {
    this.createChart();
  }
  public incomes: any[] = [
    { type: '+', name: 'Work salary', category: 'Salary', amount: 2000, date: '01.01.2024' },
    { type: '+', name: 'Investments', category: 'Investments', amount: 200, date: '01.01.2024' },
    { type: '+', name: 'Loan return', category: 'Loans', amount: 50, date: '01.01.2024' },
  ];
  public expenses: any[] = [
    { type: '-', name: 'Rent', category: 'Needs', amount: 375, date: '01.01.2024' },
    { type: '-', name: 'Utilities', category: 'Needs', amount: 150, date: '01.01.2024' },
    { type: '-', name: 'Food', category: 'Needs', amount: 400, date: '01.01.2024' },
    { type: '-', name: 'Misc', category: 'Needs', amount: 100, date: '01.01.2024' },
    { type: '-', name: 'English teacher', category: 'Wants', amount: 200, date: '01.01.2024' },
    { type: '-', name: 'Youtube Premium', category: 'Subscriptions', amount: 5, date: '01.01.2024' },
    { type: '-', name: 'Netflix', category: 'Subscriptions', amount: 6, date: '01.01.2024' },
    { type: '-', name: 'Chat GPT', category: 'Subscriptions', amount: 19, date: '01.01.2024' },
    { type: '-', name: 'Birthday Alex', category: 'Presents', amount: 100, date: '11.01.2024' },
    { type: '-', name: 'Entertainment', category: 'Wants', amount: 100, date: '01.01.2024' }
  ];
  public chart: any;
  public chart2: any;
  

  createChart(){
    Chart.register(...registerables);
    const data = {
      labels: [
        'Needs',
        'Wants',
        'Subscriptions',
        'Presents',
      ],
      datasets: [{
        label: 'Expenses',
        data: [375 + 150 + 400 + 100, 200 + 100, 6 + 19 + 5, 100],
        backgroundColor: [
          'rgb(255, 99, 132)',
          'rgb(54, 162, 235)',
          'rgb(255, 205, 86)',
          'rgb(0, 92, 187)',
        ],
        hoverOffset: 4
      }]
    };
    const incomeData = {
      labels: [
        'Salary',
        'Investments',
        'Loans'
      ],
      datasets: [{
        label: 'Incomes',
        data: [2000, 200,50],
        backgroundColor: [
          'rgb(0, 92, 187)',
          'rgb(255, 205, 86)',
          'rgb(54, 162, 235)'
        ],
        hoverOffset: 4
      }]
    };
    this.chart = new Chart("MyChart", {
      type: 'doughnut',
      data: incomeData,
    });
    this.chart2 = new Chart("MyChart2", {
      type: 'doughnut',
      data: data,
    });
  }
}
