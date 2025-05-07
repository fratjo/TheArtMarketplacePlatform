import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';
import {
  ArtisanProducts,
  Categories,
  Product,
  Products,
} from '../models/product.interface';

@Injectable({
  providedIn: 'root',
})
export class GuestService {
  private apiUrl = 'http://localhost:5140/api';
  products$ = new BehaviorSubject<Products>([]);

  constructor(private http: HttpClient) {}

  getProducts() {
    return this.http.get<Products>(`${this.apiUrl}/products`).pipe(
      tap((products) => {
        this.products$.next(products);
      })
    );
  }

  getProductById(id: string) {
    return this.http
      .get<Product>(`${this.apiUrl}/products/${id}`)
      .pipe(tap((product) => {}));
  }

  getCategories() {
    return this.http.get<Categories>(`${this.apiUrl}/categories`);
  }

  getArtisans() {
    return this.http.get<ArtisanProducts>(`${this.apiUrl}/artisans`);
  }

  filterProducts(filters: {
    search?: string;
    artisans?: string[];
    categories?: string[];
    status?: string;
    availability?: boolean;
    rating?: number[];
    sortProperty?: string;
    sortDirection?: 'asc' | 'desc';
  }) {
    // Construire les paramètres de requête
    let params = new HttpParams();
    if (filters.search) {
      params = params.set('search', filters.search);
    }
    if (filters.artisans) {
      params = params.set('artisans', filters.artisans.join(','));
    }
    if (filters.categories) {
      params = params.set('categories', filters.categories.join(','));
    }
    if (filters.status) {
      params = params.set('status', filters.status.toString());
    }
    if (filters.availability !== undefined) {
      params = params.set('availability', filters.availability.toString());
    }
    if (filters.rating !== undefined) {
      params = params.set('rating', filters.rating.join(','));
    }
    if (filters.sortProperty) {
      params = params.set('sortBy', filters.sortProperty);
    }
    if (filters.sortDirection) {
      params = params.set('sortOrder', filters.sortDirection);
    }

    // Envoyer la requête HTTP avec les paramètres
    return this.http.get<Products>(`${this.apiUrl}/products`, {
      params,
    });
  }
}
