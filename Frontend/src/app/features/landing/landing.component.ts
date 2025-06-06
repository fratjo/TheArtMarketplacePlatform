import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-landing',
  imports: [RouterLink],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.css',
})
export class LandingComponent implements OnInit {
  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    const role = this.authService.getUserRole();

    if (role) {
      switch (role) {
        case 'artisan':
          window.location.href = '/artisan/dashboard';
          break;
        case 'customer':
          window.location.href = '/products/catalog';
          break;
        case 'deliverypartner':
          window.location.href = '/delivery-partner/dashboard';
          break;
        default:
          break;
      }
    }
  }
}
