import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { Router, RoutesRecognized } from '@angular/router';
import { filter, Subject, takeUntil } from 'rxjs';

@Component({
	selector: 'fp-navbar',
	templateUrl: './navbar.component.html',
	styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit, OnDestroy {
	private readonly _router = inject(Router);
	private readonly _destroyed$ = new Subject<void>();

	public currentRoute = signal('');
	public menuItems = [
		{
			icon: 'assessment',
			path: '/dashboard',
			label: 'Dashboard',
			hidden: true,
		},
		{
			icon: 'credit_card',
			path: '/accounts',
			label: 'Accounts',
		},
		{
			icon: 'category',
			path: '/categories',
			label: 'Categories',
		},
		{
			icon: 'request_quote',
			path: '/budget',
			label: 'Budget',
		},
	];

	public ngOnInit(): void {
		this._router.events
			.pipe(
				takeUntil(this._destroyed$),
				filter((event) => event instanceof RoutesRecognized),
			)
			.subscribe((event: unknown) => {
				this.currentRoute.set((event as RoutesRecognized).urlAfterRedirects);
			});
	}

	public ngOnDestroy(): void {
		this._destroyed$.next();
		this._destroyed$.complete();
	}

	public matchesRoute(route: string): boolean {
		return this.currentRoute().includes(route);
	}
}
