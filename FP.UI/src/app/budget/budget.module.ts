import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BudgetComponent } from './budget.component';
import { RouterModule, Routes } from '@angular/router';
import { MatTableModule } from '@angular/material/table';

const routes: Routes = [
  { path: '', component: BudgetComponent }
];

@NgModule({
  declarations: [BudgetComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    MatTableModule
  ]
})
export class BudgetModule { }
