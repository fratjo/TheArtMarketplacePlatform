import { AsyncPipe, CommonModule } from '@angular/common';
import {
  Component,
  effect,
  OnChanges,
  OnInit,
  SimpleChanges,
} from '@angular/core';
import {
  BehaviorSubject,
  combineLatest,
  combineLatestAll,
  combineLatestWith,
  debounce,
  debounceTime,
  distinctUntilChanged,
  map,
  Observable,
  Subject,
} from 'rxjs';
import { ProductService } from '../../../../core/services/product.service';
import { Product, Products } from '../../../../core/models/product.interface';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-my-products',
  imports: [AsyncPipe, CommonModule, FormsModule],
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
  };
  filters = {
    name: '',
    category: '',
    status: '',
    priceMin: 0,
    priceMax: 1000,
    quantityMin: 0,
    quantityMax: 100,
    availability: '',
  };

  categories!: string[];

  filteredProducts$!: Observable<Products>;

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.products$ = this.productService.getProducts$();

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
}
