import { Injectable } from '@angular/core';
import { ToastService } from './toast.service';
import { GuestService } from './guest.service';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  constructor(
    private toastService: ToastService,
    private guestService: GuestService
  ) {}

  addToCart(productId: string, quantity: number): void {
    this.guestService.getProductById(productId).subscribe((product) => {
      const cart = JSON.parse(sessionStorage.getItem('cart') || '[]');
      const existingItem = cart.find(
        (item: { productId: string; quantity: number }) =>
          item.productId === productId
      );
      const maxQuantity = product.quantityLeft;

      let newQuantity = quantity;
      if (existingItem) {
        newQuantity = existingItem.quantity + quantity;
      }

      if (newQuantity > maxQuantity) {
        this.toastService.show({
          text: `You can't add more than ${maxQuantity} items for this product.`,
          classname: 'bg-warning text-light',
          delay: 3000,
        });
        return;
      }

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
    });
  }
}
