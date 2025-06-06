import { AsyncPipe, CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  BehaviorSubject,
  combineLatestWith,
  map,
  Observable,
  switchMap,
  tap,
} from 'rxjs';
import {
  Categories,
  Product,
  Products,
} from '../../../../core/models/product.interface';
import { FormsModule } from '@angular/forms';
import { SingleSliderComponent } from '../../../../shared/components/single-slider/single-slider.component';
import { Router, RouterLink } from '@angular/router';
import { ToastService } from '../../../../core/services/toast.service';
import { ArtisanService } from '../../../../core/services/artisan.service';
import { environment } from '../../../../../../environment';

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
  categories$!: Observable<Categories>;
  filters$ = new BehaviorSubject<any>({
    search: '',
    category: '',
    status: '',
    availability: '',
    rating: 0,
  });
  search$ = new BehaviorSubject<string>('');
  sorting$ = new BehaviorSubject<{
    property: keyof Product;
    direction: 'asc' | 'desc';
  } | null>(null);

  constructor(
    private artisanService: ArtisanService,
    private toastService: ToastService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.artisanService.getProducts().subscribe({
      next: (products) => {
        this.products$ = new BehaviorSubject<Products>(
          products.sort((a, b) => a.name.localeCompare(b.name))
        );
      },
      error: (error) => {
        console.error('Error fetching products:', error);
        this.toastService.show({
          text: `Error fetching products: ${error.error.detail}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });

    this.products$ = this.artisanService.products$;

    this.artisanService.getCategories().subscribe({
      next: (categories) => {
        this.categories$ = new Observable<Categories>((subscriber) => {
          subscriber.next(categories);
          subscriber.complete();
        });
      },
      error: (error) => {
        console.error('Error fetching categories:', error);
        this.toastService.show({
          text: `Error fetching categories: ${error.error.detail}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });

    this.filters$
      .pipe(
        tap((f) => console.log('[DEBUG] filters$', f)),
        combineLatestWith(
          this.sorting$.pipe(tap((s) => console.log('[DEBUG] sorting$', s)))
        ),
        tap(([f, s]) => console.log('[DEBUG] combineLatest →', f, s)),
        switchMap(([filters, sorting]) => {
          filters = {
            ...filters,
            sortProperty: sorting?.property,
            sortDirection: sorting?.direction,
          };

          // ...
          return this.artisanService.filterProducts(filters);
        })
      )
      .subscribe((result) => {
        // … affichage, ou stockage en local si tu n’utilises pas async pipe
        console.log('[DEBUG] produits reçus', result);
        this.products$.next(result);
      });
  }

  getImageUrl(imagePath: string): string {
    if (!imagePath) {
      return `/default_product.png`; // Fallback image
    }
    return `${environment.apiUrl}/${imagePath}`;
  }

  onRatingSliderChange(value: number) {
    const currentFilters = this.filters$.getValue();
    this.filters$.next({ ...currentFilters, rating: value });
  }

  applyFilters(params: string = '', value: any = {}) {
    const currentFilters = this.filters$.getValue();

    this.filters$.next({
      ...currentFilters,
      [params]: value.$event as string,
    });
  }

  search(event: Event) {
    const input = event.target as HTMLInputElement;
    const currentFilters = this.filters$.getValue();
    this.filters$.next({
      ...currentFilters,
      search: input.value,
    });
  }

  sortBy(property: keyof Product) {
    const currentSorting = this.sorting$.getValue();
    const newDirection =
      currentSorting?.property === property &&
      currentSorting.direction === 'asc'
        ? 'desc'
        : 'asc';

    this.sorting$.next({ property, direction: newDirection });
  }

  onEdit(id: string) {
    this.router.navigate([`/products/${id}/edit`]);
  }

  onDelete(id: string) {
    this.artisanService.deleteProduct(id).subscribe({
      next: () => {
        this.toastService.show({
          text: 'Product deleted successfully',
          classname: 'bg-success text-light',
          delay: 3000,
        });
        this.applyFilters(); // Rafraîchir les produits après suppression
      },
      error: (error) => {
        console.error('Error deleting product:', error);
        this.toastService.show({
          text: `Error deleting product: ${error.error.detail}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
  }
}
