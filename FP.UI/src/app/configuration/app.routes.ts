import { Routes } from '@angular/router';

export const routes: Routes = [
	{
		path: 'dashboard',
		loadChildren: () =>
			import('../modules/dashboard/dashboard.module').then(
				(m) => m.DashboardModule,
			),
	},
	{
		path: 'budget',
		loadChildren: () =>
			import('../modules/budget/budget.module').then((m) => m.BudgetModule),
	},
	{
		path: 'accounts',
		loadChildren: () =>
			import('../modules/accounts/accounts.module').then((m) => m.AccountsModule),
	},
	{
		path: 'categories',
		loadChildren: () =>
			import('../modules/categories/categories.module').then((m) => m.CategoriesModule),
	},
	{
		path: 'demo',
		loadChildren: () =>
			import('../modules/demo/demo.module').then((m) => m.DemoModule),
	},
	{ path: '', redirectTo: '/budget', pathMatch: 'full' },
	{ path: '**', redirectTo: '/budget' },
];
