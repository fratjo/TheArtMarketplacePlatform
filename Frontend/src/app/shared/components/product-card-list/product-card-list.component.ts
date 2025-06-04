import { CurrencyPipe } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Product } from '../../../core/models/product.interface';
import { StarRatingComponent } from '../star-rating/star-rating.component';

@Component({
  selector: 'app-product-card-list',
  imports: [StarRatingComponent, CurrencyPipe, RouterLink],
  templateUrl: './product-card-list.component.html',
  styleUrl: './product-card-list.component.css',
})
export class ProductCardListComponent {
  @Input() product!: Product;
  @Input() imageUrl: string = '';
  @Input() isWishlist: boolean = false;
  @Output() addToCart: EventEmitter<{ id: string; quantity: number }> =
    new EventEmitter<{ id: string; quantity: number }>();
  @Output() toggleToWishlist: EventEmitter<string> = new EventEmitter<string>();

  addToCartHandler(spanRef: HTMLSpanElement) {
    const quantity = parseInt(spanRef.innerText, 10);
    if (this.product.quantityLeft <= 0) {
      return;
    }

    spanRef.innerText = '1';
    this.addToCart.emit({ id: this.product.id, quantity: quantity });
  }

  toggleToWishlistHandler($event: Event) {
    $event.stopPropagation();
    $event.preventDefault();
    this.toggleToWishlist.emit(this.product.id);
  }

  increment(spanRef: HTMLSpanElement, item: Product) {
    let value = parseInt(spanRef.innerText, 10);
    if (value >= item.quantityLeft) {
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
}
