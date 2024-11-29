import { NgModule } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { SharedModule } from '@fp-shared/shared.module';
import { ComponentsModule } from 'app/components/components.module';
import { CategoriesComponent } from './categories.component';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogActions, MatDialogClose, MatDialogContent, MatDialogModule, MatDialogTitle } from '@angular/material/dialog';
import { CategoryModalDialogComponent } from './category-modal';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

const routes: Routes = [{ path: '', component: CategoriesComponent }];

@NgModule({
	declarations: [CategoriesComponent, CategoryModalDialogComponent],
	providers: [CurrencyPipe],
	imports: [CommonModule, MatMenuModule, MatButtonModule, 
		MatDialogModule,
		MatDialogActions,
		MatDialogClose,
		MatDialogTitle,
		MatDialogContent,
		ReactiveFormsModule,
		MatFormFieldModule,
		MatSelectModule,
		MatInputModule,
		RouterModule.forChild(routes), MatIconModule, SharedModule, ComponentsModule],
})
export class CategoriesModule { }
