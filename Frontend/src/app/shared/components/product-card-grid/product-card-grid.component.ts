import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Product } from '../../../core/models/product.interface';
import { ToastService } from '../../../core/services/toast.service';
import { StarRatingComponent } from '../star-rating/star-rating.component';
import { CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-product-card-grid',
  imports: [StarRatingComponent, CurrencyPipe, RouterLink],
  templateUrl: './product-card-grid.component.html',
  styleUrl: './product-card-grid.component.css',
})
export class ProductCardGridComponent {
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
