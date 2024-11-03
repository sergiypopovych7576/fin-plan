import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { Router, RouterEvent, RoutesRecognized } from '@angular/router';
import { filter, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'fp-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit, OnDestroy {
  private _destroyed$ = new Subject<void>();
  public currentRoute = signal('');

  constructor(private readonly _router: Router) {
  }

  public ngOnInit(): void {
    this._router.events
      .pipe(
        takeUntil(this._destroyed$),
        filter(event => event instanceof RoutesRecognized)
      )
      .subscribe((event: unknown) => {
        this.currentRoute.set((event as RoutesRecognized).urlAfterRedirects);
      });
  }

  ngOnDestroy(): void {
    this._destroyed$.next();
    this._destroyed$.complete();
  }

  public matchesRoute(route: string): boolean {
    return this.currentRoute().includes(route);
  }
}
