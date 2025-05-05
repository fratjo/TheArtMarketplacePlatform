import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';
import { Product, Products } from '../models/product.interface';

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

  filterProducts(filters: {
    search?: string;
    category?: string;
    status?: string;
    availability?: boolean;
    rating?: number;
    sortProperty?: string;
    sortDirection?: 'asc' | 'desc';
  }) {
    // Construire les paramètres de requête
    let params = new HttpParams();
    if (filters.search) {
      params = params.set('search', filters.search);
    }
    if (filters.category) {
      params = params.set('category', filters.category.toString());
    }
    if (filters.status) {
      params = params.set('status', filters.status.toString());
    }
    if (filters.availability !== undefined) {
      params = params.set('availability', filters.availability.toString());
    }
    if (filters.rating !== undefined) {
      params = params.set('rating', filters.rating.toString());
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
