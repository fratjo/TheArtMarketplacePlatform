import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  imports: [RouterOutlet],
  template: '<router-outlet></router-outlet>',
})
export class DashboardComponent implements OnInit {
  constructor(private router: Router, private authService: AuthService) {}

  ngOnInit(): void {
    const role = this.authService.getUserRole();

    switch (role) {
      case 'artisan':
        this.router.navigate(['/dashboard/artisan']);
        break;
      case 'customer':
        this.router.navigate(['/dashboard/customer']);
        break;
      case 'deliverypartner':
        this.router.navigate(['/dashboard/delivery-partner']);
        break;
      default:
        this.router.navigate(['/unauthorized']);
        break;
    }
  }
}
