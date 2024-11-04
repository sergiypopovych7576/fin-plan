import { Component, Input } from '@angular/core';

@Component({
  selector: 'fp-operations-table',
  templateUrl: './operations-table.component.html',
  styleUrl: './operations-table.component.scss',
})
export class OperationsTableComponent {
  @Input()
  public operations?: any[];

  public displayedColumns: string[] = ['name', 'category', 'amount', 'date'];
}
