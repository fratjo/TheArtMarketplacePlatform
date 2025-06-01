import { Injectable } from '@angular/core';
import { ToastService } from './toast.service';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  constructor(private toastService: ToastService) {}

  addToCart(productId: string, quantity: number): void {
    const cart = JSON.parse(sessionStorage.getItem('cart') || '[]');
    const existingItem = cart.find(
      (item: { productId: string; quantity: number }) =>
        item.productId === productId
    );

    if (existingItem) {
      existingItem.quantity += quantity;
    } else {
      cart.push({ productId, quantity });
    }

    sessionStorage.setItem('cart', JSON.stringify(cart));
    this.toastService.show({
      text: 'Product added to cart!',
      classname: 'bg-success text-light',
      delay: 3000,
    });

    window.dispatchEvent(new Event('cart'));
  }
}
