import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  Product,
  ProductForm,
} from '../../../../core/models/product.interface';
import { ProductFormComponent } from '../product-form/product-form.component';
import { ToastService } from '../../../../core/services/toast.service';
import { ArtisanService } from '../../../../core/services/artisan.service';
import { NgModelGroup } from '@angular/forms';

@Component({
  selector: 'app-product-form-page',
  imports: [ProductFormComponent],
  templateUrl: './product-form-page.component.html',
})
export class ProductFormPageComponent implements OnInit {
  product: Product | null = null;
  productId: string | null = null;
  isEditMode = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private artisanService: ArtisanService,
    private toastService: ToastService
  ) {}

  ngOnInit() {
    this.route.paramMap.subscribe((params) => {
      this.productId = params.get('id');
      this.isEditMode = !!this.productId;
      if (this.isEditMode && this.productId) {
        this.artisanService.getProductById(this.productId).subscribe({
          next: (product: Product) => {
            this.product = product;
            console.log('Product fetched:', product);
          },
          error: () => {
            // Gérer erreur (ex: id pas trouvé)
            this.router.navigate(['/products']);
          },
        });
      }
    });
  }

  onSave(product: FormData) {
    if (this.isEditMode && this.productId) {
      this.artisanService.updateProduct(this.productId, product).subscribe({
        next: (response: any) => {
          this.toastService.show({
            text: 'Product updated successfully',
            classname: 'bg-success text-light',
            delay: 3000,
          });
          this.router.navigate(['/artisan/products']);
        },
        error: (error: any) => {
          console.error('Error updating product:', error);
          this.toastService.show({
            text: `Error updating product: ${error.error.title}`,
            classname: 'bg-danger text-light',
            delay: 5000,
          });
        },
      });
    } else {
      this.artisanService.createProduct(product).subscribe({
        next: (response: any) => {
          this.toastService.show({
            text: 'Product created successfully',
            classname: 'bg-success text-light',
            delay: 3000,
          });
          this.router.navigate(['/artisan/products']);
        },
        error: (error: any) => {
          console.error('Error creating product:', error);
          this.toastService.show({
            text: `Error creating product: ${error.error.title}`,
            classname: 'bg-danger text-light',
            delay: 5000,
          });
        },
      });
    }
  }
}
