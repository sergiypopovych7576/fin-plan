import { NgModule } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard.component';
import { MatIconModule } from '@angular/material/icon';
import { SharedModule } from '@fp-shared/shared.module';
import { ComponentsModule } from 'app/components/components.module';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

const routes: Routes = [{ path: '', component: DashboardComponent }];

@NgModule({
	declarations: [DashboardComponent],
	imports: [CommonModule, RouterModule.forChild(routes), MatIconModule, MatProgressSpinnerModule, SharedModule, ComponentsModule],
})
export class DashboardModule { }
