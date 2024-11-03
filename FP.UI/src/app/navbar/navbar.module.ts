import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { NavbarComponent } from './navbar.component';

const COMPONENTS = [
    NavbarComponent
];

@NgModule({
    imports: [
        RouterModule,
        MatButtonModule,
        MatIconModule,
    ],
    declarations: [
        COMPONENTS
    ],
    exports: [
        COMPONENTS
    ],
})
export class NavbarModule { }
