import { Component, OnInit, signal } from '@angular/core';
import {
  ActivatedRoute,
  NavigationEnd,
  Router,
  RouterLink,
  RouterLinkActive,
} from '@angular/router';
import { BehaviorSubject, filter } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { AsyncPipe } from '@angular/common';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, AsyncPipe, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
  standalone: true,
})
export class NavbarComponent implements OnInit {
  currentUrl: string = '';
  isLoggedIn$!: BehaviorSubject<boolean>;
  userRole$!: BehaviorSubject<string | null>;
  cartQuantity = signal(0);

  constructor(
    private router: Router,
    private authService: AuthService,
    private toastService: ToastService
  ) {
    this.isLoggedIn$ = this.authService.isLoggedIn$;
    this.userRole$ = this.authService.userRole$;
  }

  ngOnInit() {
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.currentUrl = event.urlAfterRedirects;
      });

    window.addEventListener('cart', (event) => {
      this.getQuantity();
    });
    this.getQuantity();
  }

  getQuantity() {
    const storedCart = sessionStorage.getItem('cart');
    if (storedCart) {
      const cartData = JSON.parse(storedCart);
      const quantities = Object.values(cartData).map(
        (item: any) => item.quantity
      );
      this.cartQuantity.set(
        quantities.reduce((a: number, b: number) => a + b, 0)
      );
    } else {
      this.cartQuantity.set(0);
    }
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
    this.toastService.show({
      text: 'Logout successful',
      classname: 'bg-success text-light',
      delay: 2000,
    });
  }
}
