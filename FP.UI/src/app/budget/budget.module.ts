import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BudgetComponent } from './budget.component';
import { RouterModule, Routes } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { OperationsTableComponent } from './operations-table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MonthSelectorComponent } from './month-selector';
import { MonthSummaryComponent } from './month-summary';

const routes: Routes = [{ path: '', component: BudgetComponent }];

@NgModule({
  declarations: [BudgetComponent, OperationsTableComponent, MonthSelectorComponent, MonthSummaryComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    MatTableModule,
    MatTabsModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatButtonModule
  ],
})
export class BudgetModule {}
