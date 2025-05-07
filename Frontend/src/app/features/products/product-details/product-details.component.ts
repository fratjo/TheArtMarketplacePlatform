import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GuestService } from '../../../core/services/guest.service';
import { Product } from '../../../core/models/product.interface';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ToastService } from '../../../core/services/toast.service';
import { environment } from '../../../../../environment';

@Component({
  selector: 'app-product-details',
  imports: [CurrencyPipe, CommonModule],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.css',
})
export class ProductDetailsComponent implements OnInit {
  product!: Product;

  constructor(
    private guestService: GuestService,
    private activatedRoute: ActivatedRoute,
    private toast: ToastService
  ) {}

  ngOnInit() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');

    if (id) {
      this.guestService.getProductById(id).subscribe({
        next: (product) => {
          this.product = product;
        },
        error: (error) => {
          console.error('Error fetching product:', error);
          this.toast.show({
            text: `Error fetching product: ${error.error.title}`,
            classname: 'bg-danger text-light',
            delay: 5000,
          });
        },
      });
    } else {
      console.error('Product ID is missing in the route.');
    }
  }

  getImageUrl(imagePath: string): string {
    return `${environment.apiUrl}/${imagePath}`;
  }
}
