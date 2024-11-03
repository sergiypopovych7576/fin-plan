import { Component, OnInit, signal } from '@angular/core';
import { NavigationEnd, Router, RoutesRecognized } from '@angular/router';
import { filter } from 'rxjs';

@Component({
  selector: 'fp-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit {
  public currentRoute = signal('');

  constructor(private readonly _router: Router) {
  }

  public ngOnInit(): void {
    this._router.events
      .pipe(filter(event => event instanceof RoutesRecognized))
      .subscribe((event: any) => {
        this.currentRoute.set(event.urlAfterRedirects);
      });
  }

  public matchesRoute(route: string): boolean {
    return this.currentRoute().includes(route);
  }
}
