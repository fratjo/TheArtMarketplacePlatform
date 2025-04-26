import { AsyncPipe, CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, combineLatestWith, map, Observable } from 'rxjs';
import { ProductService } from '../../../../core/services/product.service';
import { Product, Products } from '../../../../core/models/product.interface';
import { FormsModule } from '@angular/forms';
import { SingleSliderComponent } from '../../../../shared/components/single-slider/single-slider.component';
import { Router, RouterLink } from '@angular/router';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-my-products',
  imports: [
    AsyncPipe,
    CommonModule,
    FormsModule,
    SingleSliderComponent,
    RouterLink,
  ],
  templateUrl: './my-products.component.html',
  styleUrl: './my-products.component.css',
})
export class MyProductsComponent implements OnInit {
  products$!: BehaviorSubject<Products>;
  filters$ = new BehaviorSubject<any>({});
  search$ = new BehaviorSubject<string>('');
  sorting: { [key: string]: 'asc' | 'desc' } = {
    name: 'asc',
    price: 'asc',
    category: 'asc',
    status: 'asc',
    quantity: 'asc',
    availability: 'asc',
    rating: 'asc',
  };
  filters = {
    name: '',
    category: '',
    status: '',
    availability: '',
    rating: 0,
  };

  categories!: string[];

  filteredProducts$!: Observable<Products>;

  constructor(
    private productService: ProductService,
    private toastService: ToastService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.productService.getProducts().subscribe({
      error: (error) => {
        console.error('Error fetching products:', error);
        this.toastService.show({
          text: `Error fetching products: ${error.error.title}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
    this.products$ = this.productService.products$;
    // this.productService.categories$.getValue();

    this.filteredProducts$ = this.products$.pipe(
      combineLatestWith(this.filters$, this.search$),
      map(([products, filters, searchTerm]) => {
        let filtered = this.productService.filterProducts(products, filters);

        if (searchTerm) {
          filtered = filtered.filter((p) =>
            [p.name, p.category, p.status, p.availability]
              .join(' ')
              .toLowerCase()
              .includes(searchTerm.toLowerCase())
          );
        }

        return filtered;
      })
    );
  }

  onRatingSliderChange(event: number) {
    this.filters.rating = event;
    this.applyFilters();
  }

  applyFilters() {
    this.filters$.next({ ...this.filters });
  }

  sortBy(property: keyof Product) {
    const direction = this.sorting[property] === 'asc' ? 1 : -1;
    this.sorting[property] = direction === 1 ? 'desc' : 'asc';

    this.filteredProducts$ = this.filteredProducts$.pipe(
      map((products) =>
        [...products].sort((a, b) =>
          a[property]! < b[property]! ? -1 * direction : 1 * direction
        )
      )
    );
  }

  search(event: Event) {
    const input = event.target as HTMLInputElement;
    this.search$.next(input.value);
  }

  onEdit(id: string) {
    this.router.navigate([`/products/${id}/edit`]);
  }

  onDelete(id: string) {
    //   console.log('Delete product with id:', id);
    //   this.productService.deleteProduct(id).subscribe({
    //     next: () => {
    //       console.log('Product deleted successfully');
    //       this.toastService.show({
    //         text: 'Product deleted successfully',
    //         classname: 'bg-success text-light',
    //         delay: 3000,
    //       });
    //     },
    //     error: (error) => {
    //       console.error('Error deleting product:', error);
    //       this.toastService.show({
    //         text: `Error deleting product: ${error.error.title}`,
    //         classname: 'bg-danger text-light',
    //         delay: 5000,
    //       });
    //     },
    //     complete: () => {
    //       this.products$ = this.productService.getProducts$();
    //     },
    //   });
  }
}
