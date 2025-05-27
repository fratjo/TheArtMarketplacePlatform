import { Component, OnInit } from '@angular/core';
import { GuestService } from '../../../core/services/guest.service';
import { Router, RouterLink } from '@angular/router';
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
  ArtisanProducts,
} from '../../../core/models/product.interface';
import { environment } from '../../../../../environment';
import { AsyncPipe, CommonModule, CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-product-catalog',
  imports: [AsyncPipe, CurrencyPipe, CommonModule, RouterLink],
  templateUrl: './product-catalog.component.html',
  styleUrl: './product-catalog.component.css',
})
export class ProductCatalogComponent implements OnInit {
  products$!: Observable<Products>;
  categories$!: Observable<Categories>;
  artisans$!: Observable<ArtisanProducts>;
  filters$ = new BehaviorSubject<any>({
    search: '',
    artisans: [],
    categories: [],
    status: '',
    availability: '',
    rating: [],
  });
  search$ = new BehaviorSubject<string>('');
  sorting$ = new BehaviorSubject<{
    property: keyof Product;
    direction: 'asc' | 'desc';
  } | null>(null);
  view = localStorage.getItem('view') || 'grid';

  onViewChange(view: string) {
    this.view = view;
    localStorage.setItem('view', view);
  }

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

    this.guestService.getCategories().subscribe({
      next: (categories) => {
        this.categories$ = new BehaviorSubject<Categories>(categories);
      },
      error: (error) => {
        console.error('Error fetching categories:', error);
        this.toastService.show({
          text: `Error fetching categories: ${error.error.title}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });

    this.guestService.getArtisans().subscribe({
      next: (artisans) => {
        this.artisans$ = new BehaviorSubject<ArtisanProducts>(artisans);
      },
      error: (error) => {
        console.error('Error fetching artisans:', error);
        this.toastService.show({
          text: `Error fetching artisans: ${error.error.title}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });

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
    if (!imagePath) {
      return `/default_product.png`; // Fallback image
    }
    return `${environment.apiUrl}/${imagePath}`;
  }

  applyFilters(params: string = '', value: any = {}) {
    const currentFilters = this.filters$.getValue();

    this.filters$.next({
      ...currentFilters,
      [params]: value.$event as string,
    });
  }

  onArtisanSelect(event: any) {
    const currentFilters = this.filters$.getValue();
    const selectedArtisans = event.target.value;

    if (currentFilters.artisans.includes(selectedArtisans)) {
      const index = currentFilters.artisans.indexOf(selectedArtisans);
      currentFilters.artisans.splice(index, 1);
    } else {
      currentFilters.artisans.push(selectedArtisans);
    }
    this.filters$.next({
      ...currentFilters,
      artisans: currentFilters.artisans,
    });
  }

  onCategorySelect(event: any) {
    const currentFilters = this.filters$.getValue();
    const selectedCategories = event.target.value;

    if (currentFilters.categories.includes(selectedCategories)) {
      const index = currentFilters.categories.indexOf(selectedCategories);
      currentFilters.categories.splice(index, 1);
    } else {
      currentFilters.categories.push(selectedCategories);
    }
    this.filters$.next({
      ...currentFilters,
      categories: currentFilters.categories,
    });
  }

  onRatingSelect(event: any) {
    const currentFilters = this.filters$.getValue();
    const selectedRating = event.target.value;

    if (currentFilters.rating.includes(selectedRating)) {
      const index = currentFilters.rating.indexOf(selectedRating);
      currentFilters.rating.splice(index, 1);
    } else {
      currentFilters.rating.push(selectedRating);
    }
    this.filters$.next({
      ...currentFilters,
      rating: currentFilters.rating,
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

  addToCart(productId: string, spanRef: HTMLSpanElement) {
    const cart = JSON.parse(sessionStorage.getItem('cart') || '[]');
    const existingItem = cart.find(
      (item: { productId: string; quantity: number }) =>
        item.productId === productId
    );

    const quantity = parseInt(spanRef.innerText, 10);

    if (existingItem) {
      existingItem.quantity += quantity;
    } else {
      cart.push({ productId, quantity: quantity });
    }

    spanRef.innerText = '1';

    sessionStorage.setItem('cart', JSON.stringify(cart));
    this.toastService.show({
      text: 'Product added to cart!',
      classname: 'bg-success text-light',
      delay: 3000,
    });

    window.dispatchEvent(new Event('cart'));
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
