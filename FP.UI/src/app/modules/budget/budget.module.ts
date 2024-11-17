import { NgModule } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
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
import {
	MatDialogActions,
	MatDialogClose,
	MatDialogContent,
	MatDialogModule,
	MatDialogTitle,
} from '@angular/material/dialog';
import { OperationModalDialogComponent } from './operation-modal';
import { CategoriesService, OperationsService } from '@fp-core/services';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ReactiveFormsModule } from '@angular/forms';
import { MatMenuModule } from '@angular/material/menu';
import {MatSelectModule} from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import {MatDatepickerModule} from '@angular/material/datepicker';
import { SharedModule } from '@fp-shared/shared.module';
import { ComponentsModule } from 'app/components/components.module';
import {MatCheckboxModule} from '@angular/material/checkbox';

const routes: Routes = [{ path: '', component: BudgetComponent }];

const MATERIAL_MODULES = []

@NgModule({
	declarations: [
		BudgetComponent,
		OperationsTableComponent,
		MonthSelectorComponent,
		MonthSummaryComponent,
		OperationModalDialogComponent,
	],
	providers: [CategoriesService, OperationsService, CurrencyPipe],
	imports: [
		CommonModule,
		RouterModule.forChild(routes),
		SharedModule,
		ComponentsModule,
		MatTableModule,
		MatTabsModule,
		MatProgressSpinnerModule,
		MatIconModule,
		MatButtonModule,
		MatDialogModule,
		MatDialogActions,
		MatDialogClose,
		MatDialogTitle,
		MatDialogContent,
		ReactiveFormsModule,
		MatFormFieldModule,
		MatMenuModule,
		MatSelectModule,
		MatInputModule,
		MatDatepickerModule,
		MatCheckboxModule
	],
})
export class BudgetModule { }
