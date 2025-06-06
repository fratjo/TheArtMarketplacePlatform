import { Component, computed, OnInit, signal } from '@angular/core';
import { Product } from '../../core/models/product.interface';
import { GuestService } from '../../core/services/guest.service';
import { BehaviorSubject } from 'rxjs';
import { AsyncPipe, CommonModule, CurrencyPipe } from '@angular/common';
import { environment } from '../../../../environment';
import { ToastService } from '../../core/services/toast.service';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CreateOrder } from '../../core/models/order.interface';
import { CustomerService } from '../../core/services/customer.service';

@Component({
  selector: 'app-cart',
  imports: [CurrencyPipe, AsyncPipe, ReactiveFormsModule, CommonModule],
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
  totalItemsComputed = computed(() => {
    return this.cartItems().reduce((total, item) => {
      return total + item.quantity;
    }, 0);
  });

  isLoggedIn$ = new BehaviorSubject<boolean>(false);

  loginForm!: FormGroup;
  showPassword: boolean = false;

  constructor(
    private customerService: CustomerService,
    private guestService: GuestService,
    private toastService: ToastService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadCartItems();
    this.isLoggedIn$.next(this.authService.isLoggedIn());
    this.loginForm = new FormGroup({
      email: new FormControl('', {
        validators: [Validators.required, Validators.email],
        updateOn: 'change',
      }),
      password: new FormControl('', {
        validators: [Validators.required],
        updateOn: 'change',
      }),
    });
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

  increaseQuantity(productId: string, quantity: number) {
    const storedCart = JSON.parse(sessionStorage.getItem('cart') || '[]');
    const existingItem = storedCart.find(
      (item: { productId: string; quantity: number }) =>
        item.productId === productId
    );

    if (existingItem && existingItem.quantity + 1 <= quantity) {
      existingItem.quantity += 1;
    } else {
      this.toastService.show({
        text: 'Cannot add more than available quantity',
        classname: 'bg-warning text-light',
        delay: 3000,
      });
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
    // redirect to checkout page if logged in
    if (this.authService.isLoggedIn()) {
      window.location.href = '/checkout';
      return;
    }
  }

  // Add methods to handle product catalog logic
  getImageUrl(imagePath: string): string {
    return `${environment.apiUrl}/${imagePath}`;
  }

  onLogin() {
    if (this.loginForm.invalid) {
      this.toastService.show({
        text: 'Please fill in all required fields.',
        classname: 'bg-danger text-light',
        delay: 3000,
      });
      return;
    }

    const loginData = this.loginForm.value;
    this.authService.login(loginData).subscribe({
      next: (response) => {
        console.log('Login successful', response);
        // save token
        this.authService.saveToken(response.token);
        window.location.href = '/cart';
      },
      error: (error) => {
        console.error('Login failed', error);
        // Handle login error, e.g., show error message
        this.toastService.show({
          text: `Login failed:  ${error.error.detail}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
  }
}
