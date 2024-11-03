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
  displayedColumns: string[] = ['name', 'category', 'amount', 'date'];
  incomes: unknown[] = [
    { type: '+', name: 'Salary', category: 'Income', amount: 2000, date: '01.01.2024' },
  ];
  expenses: unknown[] = [
    { type: '-', name: 'Rent Kyiv', category: 'Needs', amount: 375, date: '01.01.2024' },
    { type: '-', name: 'Utilities Kyiv', category: 'Needs', amount: 50, date: '01.01.2024' },
    { type: '-', name: 'Food', category: 'Needs', amount: 400, date: '01.01.2024' },
    { type: '-', name: 'Misc', category: 'Needs', amount: 100, date: '01.01.2024' },
    { type: '-', name: 'Rent Avellino', category: 'Needs', amount: 450, date: '01.01.2024' },

    { type: '-', name: 'Italian teacher', category: 'Wants', amount: 200, date: '01.01.2024' },
    { type: '-', name: 'Psychologist', category: 'Wants', amount: 190, date: '01.01.2024' },
    { type: '-', name: 'Youtube Premium', category: 'Wants', amount: 5, date: '01.01.2024' },
    { type: '-', name: 'Netflix', category: 'Wants', amount: 6, date: '01.01.2024' },
    { type: '-', name: 'Chat GPT', category: 'Wants', amount: 19, date: '01.01.2024' },

    { type: '-', name: 'BD Dima', category: 'Wants', amount: 100, date: '11.01.2024' },
    { type: '-', name: 'Ng Misha', category: 'Wants', amount: 100, date: '01.01.2024' },
    { type: '-', name: 'Entertainment', category: 'Wants', amount: 100, date: '01.01.2024' }
  ]
  public chart: any;
  public chart2: any;


  createChart(){
    Chart.register(...registerables);
    this.chart = new Chart("MyChart", {
      type: 'line', //this denotes tha type of chart

      data: {// values on X-Axis
        labels: ['2022-05-10', '2022-05-11', '2022-05-12','2022-05-13',
                                 '2022-05-14', '2022-05-15', '2022-05-16','2022-05-17', ], 
           datasets: [
          {
            label: "Sales",
            data: ['467','576', '572', '79', '92',
                                 '574', '573', '576'],
            backgroundColor: 'blue'
          },
          {
            label: "Profit",
            data: ['542', '542', '536', '327', '17',
                                     '0.00', '538', '541'],
            backgroundColor: 'limegreen'
          }  
        ]
      },
      options: {
        aspectRatio:2.5
      }

    });
    this.chart2 = new Chart("MyChart2", {
      type: 'line', //this denotes tha type of chart

      data: {// values on X-Axis
        labels: ['2022-05-10', '2022-05-11', '2022-05-12','2022-05-13',
                                 '2022-05-14', '2022-05-15', '2022-05-16','2022-05-17', ], 
           datasets: [
          {
            label: "Sales",
            data: ['467','576', '572', '79', '92',
                                 '574', '573', '576'],
            backgroundColor: 'blue'
          },
          {
            label: "Profit",
            data: ['542', '542', '536', '327', '17',
                                     '0.00', '538', '541'],
            backgroundColor: 'limegreen'
          }  
        ]
      },
      options: {
        aspectRatio:2.5
      }

    });
  }
}
