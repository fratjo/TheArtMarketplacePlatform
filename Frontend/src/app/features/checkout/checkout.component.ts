import { Component, computed, OnInit, signal } from '@angular/core';
import { Product } from '../../core/models/product.interface';
import { AuthService } from '../../core/services/auth.service';
import { CustomerService } from '../../core/services/customer.service';
import { GuestService } from '../../core/services/guest.service';
import { ToastService } from '../../core/services/toast.service';
import { AsyncPipe, CommonModule, CurrencyPipe } from '@angular/common';
import { BehaviorSubject, Observable } from 'rxjs';
import { DeliveryPartner } from '../../core/models/user.interface';
import { CreateOrder } from '../../core/models/order.interface';

type CheckoutResume = {
  [artisanId: string]: {
    artisan: { id: string; username: string };
    products: { product: Product | undefined; quantity: number }[];
  };
};

@Component({
  selector: 'app-checkout',
  imports: [CurrencyPipe, AsyncPipe, CommonModule],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css',
})
export class CheckoutComponent implements OnInit {
  checkoutResume: CheckoutResume = {};
  deliveryPartners$: BehaviorSubject<DeliveryPartner[]> = new BehaviorSubject<
    DeliveryPartner[]
  >([]);
  selectedDeliveryPartner: DeliveryPartner | null = null;
  cartItems = signal<{ product: Product; quantity: number }[]>([]);
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

  constructor(
    private customerService: CustomerService,
    private guestService: GuestService,
    private toastService: ToastService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadCartItems();
    this.guestService.getDeliveryPartners().subscribe({
      next: (deliveryPartners) => {
        this.deliveryPartners$.next(deliveryPartners);
      },
      error: (error) => {
        console.error('Error loading delivery partners:', error);
        this.toastService.show({
          text: 'Failed to load delivery partners. Please try again later.',
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
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
          product: product as Product,
          quantity: Object.values(cartData).map((item: any) => item.quantity)[
            index
          ],
        }));
        this.cartItems.set(items);
        this.loadCheckoutResume();
      });
    } else {
      this.cartItems.set([]);
      this.loadCheckoutResume();
    }
  }

  loadCheckoutResume() {
    this.checkoutResume = {};

    this.cartItems().forEach((item) => {
      const artisanId = item.product.artisanId;
      if (!artisanId) return;

      if (!this.checkoutResume[artisanId]) {
        console.log(item);

        this.checkoutResume[artisanId] = {
          artisan: {
            id: artisanId,
            username: item.product?.artisan.user.username || '',
          },
          products: [],
        };
      }

      this.checkoutResume[artisanId].products.push({
        product: item.product,
        quantity: item.quantity,
      });
    });
  }

  getCheckoutResumeArray() {
    const arr = Object.values(this.checkoutResume).map((entry) => ({
      artisan: entry.artisan,
      products: entry.products,
    }));
    return arr;
  }

  placeOrder() {
    const order: CreateOrder = {
      customerId: this.authService.getUserId() || '',
      deliveryPartnerId: this.selectedDeliveryPartner?.id || '',
      orderProducts: this.cartItems().map((item) => ({
        productId: item.product?.id || '',
        artisanId: item.product?.artisan.id || '',
        quantity: item.quantity,
      })),
    };
    console.log('Order to be placed:', order);
    this.customerService.createOrder(order).subscribe({
      next: (response) => {
        console.log('Order placed successfully', response);
        this.toastService.show({
          text: 'Order placed successfully!',
          classname: 'bg-success text-light',
          delay: 3000,
        });
        // Clear the cart after checkout
        this.clearCart();
        window.location.href = '/customer/orders';
      },
      error: (error) => {
        console.error('Error placing order', error);
        this.toastService.show({
          text: `Error placing order: ${error.error.title}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
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
}
