import { Component } from '@angular/core';
import { MainModule } from './modules';

@Component({
	selector: 'fp-root',
	imports: [MainModule],
	standalone: true,
	templateUrl: './app.component.html',
})
export class AppComponent {}
