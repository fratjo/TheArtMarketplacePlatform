import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import {
  Observable,
  BehaviorSubject,
  combineLatestWith,
  switchMap,
  map,
} from 'rxjs';
import { environment } from '../../../../../environment';
import {
  Products,
  Categories,
  ArtisanProducts,
  Product,
} from '../../../core/models/product.interface';
import { AuthService } from '../../../core/services/auth.service';
import { CartService } from '../../../core/services/cart.service';
import { CustomerService } from '../../../core/services/customer.service';
import { ToastService } from '../../../core/services/toast.service';
import { AsyncPipe, CurrencyPipe, CommonModule } from '@angular/common';
import { ProductCardGridComponent } from '../../../shared/components/product-card-grid/product-card-grid.component';
import { ProductCardListComponent } from '../../../shared/components/product-card-list/product-card-list.component';
import { StarRatingComponent } from '../../../shared/components/star-rating/star-rating.component';
import { GuestService } from '../../../core/services/guest.service';

@Component({
  selector: 'app-favorites',
  imports: [
    AsyncPipe,
    CommonModule,
    ProductCardGridComponent,
    ProductCardListComponent,
  ],
  templateUrl: './favorites.component.html',
  styleUrl: './favorites.component.css',
})
export class FavoritesComponent implements OnInit {
  products$!: Observable<Products>;
  view = localStorage.getItem('view') || 'grid';
  favorites: string[] = [];

  onViewChange(view: string) {
    this.view = view;
    localStorage.setItem('view', view);
  }

  constructor(
    private guestService: GuestService,
    private customerService: CustomerService,
    private authService: AuthService,
    private toastService: ToastService,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    if (
      this.authService.isLoggedIn() &&
      this.authService.getUserRole() === 'customer'
    ) {
      this.customerService.getFavoriteProducts().subscribe({
        next: (favorites) => {
          this.favorites = favorites.map((fav) => fav.id);
          this.products$ = this.guestService
            .getProducts()
            .pipe(
              map((products) =>
                products.filter((product) =>
                  this.favorites.includes(product.id)
                )
              )
            );
        },
        error: (error) => {
          console.error('Error fetching favorites:', error);
          this.toastService.show({
            text: `Error fetching favorites: ${error.error.detail}`,
            classname: 'bg-danger text-light',
            delay: 5000,
          });
        },
      });
    }
  }

  // Add methods to handle product catalog logic
  getImageUrl(imagePath: string): string {
    if (!imagePath) {
      return `/default_product.png`; // Fallback image
    }
    return `${environment.apiUrl}/${imagePath}`;
  }

  addToCart($event: { id: string; quantity: number }) {
    this.cartService.addToCart($event.id, $event.quantity);
  }

  increment(spanRef: HTMLSpanElement, item: Product) {
    let value = parseInt(spanRef.innerText, 10);
    if (value >= item.quantityLeft) {
      this.toastService.show({
        text: 'Cannot add more than available quantity',
        classname: 'bg-warning text-light',
        delay: 3000,
      });
      return;
    }
    spanRef.innerText = (value + 1).toString();
  }

  decrement(spanRef: HTMLSpanElement) {
    let value = parseInt(spanRef.innerText, 10);
    if (value > 1) {
      spanRef.innerText = (value - 1).toString();
    }
  }

  toggleFavorite(productId: string) {
    if (
      !this.authService.isLoggedIn() ||
      this.authService.getUserRole() !== 'customer'
    ) {
      this.toastService.show({
        text: 'You must be logged in as customer to add products to favorites.',
        classname: 'bg-warning text-light',
        delay: 3000,
      });
      return;
    }

    if (this.favorites.includes(productId)) {
      this.customerService.removeProductFromFavorites(productId).subscribe({
        next: () => {
          this.favorites = this.favorites.filter((id) => id !== productId);
          this.toastService.show({
            text: 'Product removed from favorites successfully.',
            classname: 'bg-success text-light',
            delay: 3000,
          });
        },
        error: (error) => {
          console.error('Error removing product from favorites:', error);
          this.toastService.show({
            text: `Error removing product from favorites: ${error.error.detail}`,
            classname: 'bg-danger text-light',
            delay: 5000,
          });
        },
      });
    } else {
      this.customerService.addProductToFavorites(productId).subscribe({
        next: () => {
          this.favorites.push(productId);
          this.toastService.show({
            text: 'Product added to favorites successfully.',
            classname: 'bg-success text-light',
            delay: 3000,
          });
        },
        error: (error) => {
          console.error('Error adding product to favorites:', error);
          this.toastService.show({
            text: `Error adding product to favorites: ${error.error.detail}`,
            classname: 'bg-danger text-light',
            delay: 5000,
          });
        },
      });
    }
  }
}
