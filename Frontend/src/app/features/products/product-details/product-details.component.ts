import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GuestService } from '../../../core/services/guest.service';
import { Product } from '../../../core/models/product.interface';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ToastService } from '../../../core/services/toast.service';
import { environment } from '../../../../../environment';
import { AuthService } from '../../../core/services/auth.service';
import { CustomerService } from '../../../core/services/customer.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ArtisanService } from '../../../core/services/artisan.service';
import { StarRatingComponent } from '../../../shared/components/star-rating/star-rating.component';
import { CartService } from '../../../core/services/cart.service';

@Component({
  selector: 'app-product-details',
  imports: [
    CurrencyPipe,
    CommonModule,
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    StarRatingComponent,
  ],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.css',
})
export class ProductDetailsComponent implements OnInit {
  product!: Product;
  productReview = { id: '', score: 0, comment: '', responseText: '' };
  alreadyBoughtAndNotReviewed: boolean = false;
  isOwnedByArtisan: boolean = false;

  constructor(
    private authService: AuthService,
    private guestService: GuestService,
    private customerService: CustomerService,
    private artisanService: ArtisanService,
    private activatedRoute: ActivatedRoute,
    private toastService: ToastService,
    private cartService: CartService
  ) {}

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');

    if (id) {
      this.guestService.getProductById(id).subscribe({
        next: (product) => {
          this.product = product;
          console.log('Product fetched successfully:', this.product);

          if (
            this.authService.isLoggedIn() &&
            this.authService.getUserRole() === 'customer'
          ) {
            this.customerService
              .checkIfAlreadyBoughtProductAndNotReviewed(this.product.id)
              .subscribe({
                next: (alreadyBoughtAndNotReviewed) => {
                  this.alreadyBoughtAndNotReviewed =
                    alreadyBoughtAndNotReviewed;
                },
              });
          } else if (
            this.authService.isLoggedIn() &&
            this.authService.getUserRole() === 'artisan' &&
            this.artisanService.checkIfProductIsOwnedByArtisan(this.product)
          ) {
            this.isOwnedByArtisan = true;
          }
        },
        error: (error) => {
          console.error('Error fetching product:', error);
          this.toastService.show({
            text: `Error fetching product: ${error.error.detail}`,
            classname: 'bg-danger text-light',
            delay: 5000,
          });
        },
      });

      this.productReview.id = id; // Initialize review ID with product ID
    } else {
      console.error('Product ID is missing in the route.');
    }
  }

  getImageUrl(imagePath: string): string {
    if (!imagePath) {
      return `/default_product.png`; // Fallback image
    }
    return `${environment.apiUrl}/${imagePath}`;
  }

  submitReview() {
    this.customerService
      .reviewProduct(this.product.id, this.productReview)
      .subscribe({
        next: () => {
          this.toastService.show({
            text: 'Review submitted successfully!',
            classname: 'bg-success text-light',
            delay: 5000,
          });
          this.alreadyBoughtAndNotReviewed = false;
        },
        error: (error) => {
          console.error('Error submitting review:', error);
          this.toastService.show({
            text: `Error submitting review: ${error.error.detail}`,
            classname: 'bg-danger text-light',
            delay: 5000,
          });
        },
      });
  }

  respondToReview(reviewId: string) {
    const review = this.product.productReviews.find((r) => r.id === reviewId);
    if (!this.productReview.responseText) return;

    // Appelle ton service pour envoyer la réponse au backend
    this.artisanService
      .respondToReview(reviewId, this.productReview.responseText)
      .subscribe({
        next: () => {
          this.toastService.show({
            text: 'Response submitted successfully!',
            classname: 'bg-success text-light',
            delay: 5000,
          });
          window.location.reload(); // Reload the page to see the updated review
        },
        error: (err) => {
          console.error('Error responding to review:', err);
          this.toastService.show({
            text: `Error responding to review: ${err.error.detail}`,
            classname: 'bg-danger text-light',
            delay: 5000,
          });
        },
      });
  }

  addToCart(productId: string, spanRef: HTMLSpanElement) {
    const quantity = parseInt(spanRef.innerText, 10);

    spanRef.innerText = '1';

    this.cartService.addToCart(productId, quantity);
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
}
