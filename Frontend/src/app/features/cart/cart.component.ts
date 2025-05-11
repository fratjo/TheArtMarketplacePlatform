import { Component, computed, OnInit, signal } from '@angular/core';
import { Product } from '../../core/models/product.interface';
import { GuestService } from '../../core/services/guest.service';
import { BehaviorSubject } from 'rxjs';
import { AsyncPipe, CurrencyPipe } from '@angular/common';
import { environment } from '../../../../environment';
import { ToastService } from '../../core/services/toast.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-cart',
  imports: [CurrencyPipe],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css',
})
export class CartComponent implements OnInit {
  cartItems = signal<{ product: Product | undefined; quantity: number }[]>([]);
  totalPriceComputed = computed(() => {
    return this.cartItems().reduce((total, item) => {
      return total + (item.product?.price || 0) * item.quantity;
    }, 0);
  });

  constructor(
    private guestService: GuestService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.loadCartItems();
  }

  loadCartItems() {
    const storedCart = sessionStorage.getItem('cart');
    if (storedCart) {
      const cartData = JSON.parse(storedCart);
      const productIds = Object.values(cartData).map(
        (item: any) => item.productId
      );

      Promise.all(
        productIds.map((id) => this.guestService.getProductById(id).toPromise())
      ).then((products) => {
        const items = products.map((product, index) => ({
          product,
          quantity: Object.values(cartData).map((item: any) => item.quantity)[
            index
          ],
        }));
        this.cartItems.set(items);
      });
    } else {
      this.cartItems.set([]);
    }
  }

  increaseQuantity(productId: string) {
    const storedCart = JSON.parse(sessionStorage.getItem('cart') || '[]');
    const existingItem = storedCart.find(
      (item: { productId: string; quantity: number }) =>
        item.productId === productId
    );

    if (existingItem) {
      existingItem.quantity += 1;
    } else {
      storedCart.push({ productId, quantity: 1 });
    }

    sessionStorage.setItem('cart', JSON.stringify(storedCart));

    this.loadCartItems();

    window.dispatchEvent(new Event('cart'));
  }

  decreaseQuantity(productId: string) {
    const storedCart = JSON.parse(sessionStorage.getItem('cart') || '[]');
    const existingItem = storedCart.find(
      (item: { productId: string; quantity: number }) =>
        item.productId === productId
    );

    if (existingItem) {
      existingItem.quantity -= 1;
      if (existingItem.quantity <= 0) {
        const index = storedCart.indexOf(existingItem);
        if (index > -1) {
          storedCart.splice(index, 1);
        }
      }
    } else {
      storedCart.push({ productId, quantity: 1 });
    }

    sessionStorage.setItem('cart', JSON.stringify(storedCart));
    this.loadCartItems();

    window.dispatchEvent(new Event('cart'));
  }

  removeFromCart(productId: string) {
    const storedCart = JSON.parse(sessionStorage.getItem('cart') || '[]');
    const existingItem = storedCart.find(
      (item: { productId: string; quantity: number }) =>
        item.productId === productId
    );

    if (existingItem) {
      const index = storedCart.indexOf(existingItem);
      if (index > -1) {
        storedCart.splice(index, 1);
      }
    } else {
      storedCart.push({ productId, quantity: 1 });
    }

    sessionStorage.setItem('cart', JSON.stringify(storedCart));
    this.toastService.show({
      text: 'Product deleted from cart!',
      classname: 'bg-success text-light',
      delay: 3000,
    });
    this.loadCartItems();

    window.dispatchEvent(new Event('cart'));
  }

  clearCart() {
    sessionStorage.removeItem('cart');
    this.cartItems.set([]);
    this.toastService.show({
      text: 'Product cart cleared!',
      classname: 'bg-success text-light',
      delay: 3000,
    });

    window.dispatchEvent(new Event('cart'));
  }

  checkout() {
    // Implement checkout logic here
    console.log('Proceeding to checkout with items:', this.cartItems());
    // Clear the cart after checkout
    this.clearCart();
  }

  // Add methods to handle product catalog logic
  getImageUrl(imagePath: string): string {
    return `${environment.apiUrl}/${imagePath}`;
  }
}
