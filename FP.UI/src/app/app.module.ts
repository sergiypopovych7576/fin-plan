import { NgModule } from '@angular/core';
import { NavbarComponent } from './navbar';
import {MatButtonModule} from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';

@NgModule({
  imports: [
    MatButtonModule,
    MatIconModule,
    RouterModule
  ],
  declarations: [
    NavbarComponent
  ],
  exports: [
    NavbarComponent
  ],
})
export class AppModule {}
