import { NgModule } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { SharedModule } from '@fp-shared/shared.module';
import { ComponentsModule } from 'app/components/components.module';
import { AccountsComponent } from './accounts.component';

const routes: Routes = [{ path: '', component: AccountsComponent }];

@NgModule({
    declarations: [AccountsComponent],
    providers: [CurrencyPipe],
    imports: [CommonModule, RouterModule.forChild(routes), MatIconModule, SharedModule, ComponentsModule],
})
export class AccountsModule { }
