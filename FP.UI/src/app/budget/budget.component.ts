import { Component } from '@angular/core';

@Component({
  selector: 'fp-budget',
  templateUrl: './budget.component.html',
  styleUrl: './budget.component.scss'
})
export class BudgetComponent {

  displayedColumns: string[] = ['name', 'category', 'amount', 'date'];
  ELEMENT_DATA: any[] = [
    { name: 'Rent Kyiv', category: 'Needs', amount: 375, date: '01.01.2024' },
    { name: 'Utilities Kyiv', category: 'Needs', amount: 50, date: '01.01.2024' },
    { name: 'Food', category: 'Needs', amount: 400, date: '01.01.2024' },
    { name: 'Misc', category: 'Needs', amount: 100, date: '01.01.2024' },
    { name: 'Rent Avellino', category: 'Needs', amount: 450, date: '01.01.2024' },

    { name: 'Italian teacher', category: 'Wants', amount: 200, date: '01.01.2024' },
    { name: 'Psychologist', category: 'Wants', amount: 190, date: '01.01.2024' },
    { name: 'Youtube Premium', category: 'Wants', amount: 5, date: '01.01.2024' },
    { name: 'Netflix', category: 'Wants', amount: 6, date: '01.01.2024' },
    { name: 'Chat GPT', category: 'Wants', amount: 19, date: '01.01.2024' },

    { name: 'BD Dima', category: 'Wants', amount: 100, date: '11.01.2024' },
    { name: 'Ng Misha', category: 'Wants', amount: 100, date: '01.01.2024' },
    { name: 'Entertainment', category: 'Wants', amount: 100, date: '01.01.2024' }
  ];

  dataSource = this.ELEMENT_DATA;
}
