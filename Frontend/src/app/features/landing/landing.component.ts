import { Component } from '@angular/core';
import { FooterComponent } from '../../layout/footer/footer.component';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-landing',
  imports: [FooterComponent, RouterLink],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.css',
})
export class LandingComponent {}
