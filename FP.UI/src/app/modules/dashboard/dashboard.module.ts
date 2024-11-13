import { NgModule } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard.component';
import { MetricComponent } from '@fp-shared/metric';
import { ToMoneyPipe } from '@fp-shared/to-money';
import { MatIconModule } from '@angular/material/icon';
import { SharedModule } from '@fp-shared/shared.module';

const routes: Routes = [{ path: '', component: DashboardComponent }];

@NgModule({
	declarations: [DashboardComponent, MetricComponent],
	providers: [CurrencyPipe],
	imports: [CommonModule, RouterModule.forChild(routes), MatIconModule, SharedModule],
})
export class DashboardModule {}
