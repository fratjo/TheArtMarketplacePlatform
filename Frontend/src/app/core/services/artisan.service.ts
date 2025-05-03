import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';
import {
  Categories,
  Product,
  ProductForm,
  Products,
} from '../models/product.interface';
import { AuthService } from './auth.service';
import { Form } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class ArtisanService {
  private apiUrl = 'http://localhost:5140/api/artisans';
  products$ = new BehaviorSubject<Products>([]);

  constructor(private authService: AuthService, private http: HttpClient) {}

  getProducts() {
    // get user role
    const userRole = this.authService.userRole$.getValue();

    // get user id
    const userId = this.authService.getUserId();

    return this.http.get<Products>(`${this.apiUrl}/${userId}/products`).pipe(
      tap((products) => {
        this.products$.next(products);
      })
    );
  }

  getProductById(id: string) {
    // get user id
    const userId = this.authService.getUserId();

    return this.http
      .get<Product>(`${this.apiUrl}/${userId}/products/${id}`)
      .pipe(tap((product) => {}));
  }

  getCategories() {
    return this.http.get<Categories>(`${this.apiUrl}/categories`);
  }

  createProduct(data: FormData) {
    const product: ProductForm = {
      name: data.get('name') as string,
      description: data.get('description') as string,
      price: parseFloat(data.get('price') as string),
      category: data.get('category') as string,
      status: data.get('status') as string,
      image: data.get('image') as File,
      imageExtension: data.get('extension') as string,
      quantityLeft: parseInt(data.get('quantityLeft') as string),
    };

    const userId = this.authService.getUserId();
    return this.http
      .post<Product>(`${this.apiUrl}/${userId}/products`, product)
      .pipe(
        tap((newProduct) => {
          const currentProducts = this.products$.getValue();
          this.products$.next([...currentProducts, newProduct]);
        })
      );
  }

  updateProduct(id: string, data: FormData) {
    const updatedProduct: ProductForm = {
      // id: data.get('id') as string,
      name: data.get('name') as string,
      description: data.get('description') as string,
      price: parseFloat(data.get('price') as string),
      category: data.get('category') as string,
      status: data.get('status') as string,
      image: data.get('image') as File,
      imageExtension: data.get('extension') as string,
      quantityLeft: parseInt(data.get('quantityLeft') as string),
    };

    const userId = this.authService.getUserId();
    return this.http
      .put<Product>(`${this.apiUrl}/${userId}/products/${id}`, updatedProduct)
      .pipe(
        tap((updatedProduct) => {
          const currentProducts = this.products$.getValue();
          const index = currentProducts.findIndex((p) => p.id === id);
          if (index !== -1) {
            currentProducts[index] = updatedProduct;
            this.products$.next([...currentProducts]);
          }
        })
      );
  }

  deleteProduct(id: string) {
    const userId = this.authService.getUserId();
    return this.http
      .delete<Product>(`${this.apiUrl}/${userId}/products/${id}`)
      .pipe(
        tap(() => {
          const currentProducts = this.products$.getValue();
          const updatedProducts = currentProducts.filter((p) => p.id !== id);
          this.products$.next(updatedProducts);
        })
      );
  }

  filterProducts(filters: {
    search?: string;
    category?: string;
    status?: string;
    availability?: boolean;
    rating?: number;
  }) {
    const userId = this.authService.getUserId();

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

    // Envoyer la requête HTTP avec les paramètres
    return this.http.get<Products>(`${this.apiUrl}/${userId}/products`, {
      params,
    });
  }
}
