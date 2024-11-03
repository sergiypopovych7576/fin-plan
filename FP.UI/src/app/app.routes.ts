import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: 'dashboard', loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule) },
    { path: 'budget', loadChildren: () => import('./budget/budget.module').then(m => m.BudgetModule) },
    { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
    { path: '**', redirectTo: '/budget' }
];
