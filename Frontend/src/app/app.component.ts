import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './layout/navbar/navbar.component';
import { ToastsComponent } from './features/toasts/toasts.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavbarComponent, ToastsComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'The Art Marketplace';
}
