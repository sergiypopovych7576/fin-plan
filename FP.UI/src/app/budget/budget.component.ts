import { Component } from '@angular/core';

@Component({
  selector: 'fp-budget',
  templateUrl: './budget.component.html',
  styleUrl: './budget.component.scss'
})
export class BudgetComponent {
  displayedColumns: string[] = ['type', 'name', 'category', 'amount', 'date'];
  ELEMENT_DATA: unknown[] = [
    { type: '+', name: 'Salary', category: 'Income', amount: 2000, date: '01.01.2024' },
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
  ];

  dataSource = this.ELEMENT_DATA;
}
