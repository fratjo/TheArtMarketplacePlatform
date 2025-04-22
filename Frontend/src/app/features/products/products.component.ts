import { Component, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-products',
  imports: [RouterOutlet],
  template: '<router-outlet></router-outlet>',
})
export class ProductsComponent implements OnInit {
  constructor(private router: Router, private authService: AuthService) {}

  ngOnInit(): void {
    const role = this.authService.getUserRole();

    console.log('User role:', role);

    switch (role) {
      case 'artisan':
        this.router.navigate(['/products/my-products']);
        break;
      case 'deliverypartner':
        this.router.navigate(['/unauthorized']);
        break;
      default:
        this.router.navigate(['/products/product-catalog']);
        break;
    }
  }
}
