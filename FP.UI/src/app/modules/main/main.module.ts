import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, RouterOutlet } from '@angular/router';
import { NavbarComponent } from './navbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MainComponent } from './main.component';
import {MatSidenavModule} from '@angular/material/sidenav'

const COMPONENTS = [NavbarComponent, MainComponent];

@NgModule({
	declarations: COMPONENTS,
	imports: [
		CommonModule,
		RouterModule,
		RouterOutlet,
		MatSidenavModule,
		MatButtonModule,
		MatIconModule,
	],
	exports: [MainComponent],
})
export class MainModule {}
