import { Component, OnInit } from '@angular/core';
import { GuestService } from '../../../core/services/guest.service';
import { Router } from '@angular/router';
import { ToastService } from '../../../core/services/toast.service';
import {
  BehaviorSubject,
  combineLatestWith,
  Observable,
  switchMap,
} from 'rxjs';
import {
  Products,
  Categories,
  Product,
} from '../../../core/models/product.interface';
import { environment } from '../../../../../environment';
import { AsyncPipe, CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-product-catalog',
  imports: [AsyncPipe, CurrencyPipe],
  templateUrl: './product-catalog.component.html',
  styleUrl: './product-catalog.component.css',
})
export class ProductCatalogComponent implements OnInit {
  products$!: Observable<Products>;
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
    private guestService: GuestService,
    private toastService: ToastService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.guestService.getProducts().subscribe({
      error: (error) => {
        console.error('Error fetching products:', error);
        this.toastService.show({
          text: `Error fetching products: ${error.error.title}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
    this.products$ = this.guestService.products$;

    // this.guestService.getCategories().subscribe({
    //   next: (categories) => {
    //     this.categories$ = new BehaviorSubject<Categories>(categories);
    //   },
    //   error: (error) => {
    //     console.error('Error fetching categories:', error);
    //     this.toastService.show({
    //       text: `Error fetching categories: ${error.error.title}`,
    //       classname: 'bg-danger text-light',
    //       delay: 5000,
    //     });
    //   },
    // });

    // Combine les filtres et le tri pour envoyer au serveur
    this.products$ = this.filters$.pipe(
      combineLatestWith(this.sorting$),
      switchMap(([filters, sorting]) => {
        // Ajouter les critères de tri aux filtres
        if (sorting) {
          filters = {
            ...filters,
            sortProperty: sorting.property,
            sortDirection: sorting.direction,
          };
        }

        // Envoyer les filtres au serveur
        return this.guestService.filterProducts(filters);
      })
    );
  }

  // Add methods to handle product catalog logic
  getImageUrl(imagePath: string): string {
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

  sortBy(event: any) {
    const currentSorting = this.sorting$.getValue();

    const value = event.target.value.split(' ');
    const property = value[0] as keyof Product;
    const direction = value[1] as 'asc' | 'desc';

    this.sorting$.next({ property, direction });
  }
}
