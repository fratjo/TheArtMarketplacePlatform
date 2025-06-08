import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../../core/services/admin.service';
import { ToastService } from '../../../core/services/toast.service';
import { BehaviorSubject } from 'rxjs';
import { AsyncPipe, CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GuestService } from '../../../core/services/guest.service';

@Component({
  selector: 'app-product-admin-panel',
  imports: [AsyncPipe, DatePipe, CommonModule, FormsModule],
  templateUrl: './product-admin-panel.component.html',
  styleUrl: './product-admin-panel.component.css',
})
export class ProductAdminPanelComponent implements OnInit {
  products$: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);
  artisans: string[] = [];
  categories: string[] = [];

  filters: any = {
    search: '',
    artisan: '',
    category: '',
    sortBy: '',
    sortOrder: '',
  };

  constructor(
    private adminService: AdminService,
    private guestService: GuestService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    const urlParams = new URLSearchParams(window.location.search);
    const artisanParam = urlParams.get('artisan');
    if (artisanParam) {
      this.filters.artisan = artisanParam;
    }

    this.applyFilters();
    this.guestService.getArtisans().subscribe({
      next: (artisans) => {
        this.artisans = artisans.map((artisan) => artisan.username);
        console.log('Loaded artisans:', this.artisans);
      },
      error: (error) => {
        console.error('Error loading artisans:', error);
        this.toastService.show({
          text: `Error loading artisans: ${error.error.detail}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
    this.guestService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories.map((category) => category.name);
        console.log('Loaded categories:', this.categories);
      },
      error: (error) => {
        console.error('Error loading categories:', error);
        this.toastService.show({
          text: `Error loading categories: ${error.error.detail}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
  }

  applyFilters() {
    console.log('Applying filters:', this.filters);

    this.adminService.getAllProducts(this.filters).subscribe({
      next: (products) => {
        this.products$.next(products);
      },
      error: (error) => {
        this.toastService.show({
          text: `Erreur lors du chargement des produits : ${
            error.error?.detail || error.message
          }`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
  }

  toggleDelete(product: any) {
    const newValue = !product.isDeleted;
    this.adminService.setDeleteProduct(product.id).subscribe({
      next: () => {
        product.isDeleted = newValue;
        this.toastService.show({
          text: 'Produit supprimé avec succès',
          classname: 'bg-success text-light',
          delay: 3000,
        });
      },
      error: (error) => {
        this.toastService.show({
          text: `Erreur lors de la suppression : ${
            error.error?.detail || error.message
          }`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
  }
}
