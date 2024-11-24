import { NgModule } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { SharedModule } from '@fp-shared/shared.module';
import { ComponentsModule } from 'app/components/components.module';
import { CategoriesComponent } from './categories.component';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';

const routes: Routes = [{ path: '', component: CategoriesComponent }];

@NgModule({
	declarations: [CategoriesComponent],
	providers: [CurrencyPipe],
	imports: [CommonModule, MatMenuModule, MatButtonModule, RouterModule.forChild(routes), MatIconModule, SharedModule, ComponentsModule],
})
export class CategoriesModule { }
