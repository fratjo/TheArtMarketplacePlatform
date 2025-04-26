import { Injectable } from '@angular/core';
import { Product, Products } from '../models/product.interface';
import { BehaviorSubject, tap } from 'rxjs';
import { AuthService } from './auth.service';
import { HttpClient } from '@angular/common/http';
import { ToastService } from './toast.service';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private apiUrl = 'http://localhost:5140/api/artisans';
  products$ = new BehaviorSubject<Products>([]);

  constructor(private authService: AuthService, private http: HttpClient) {}

  getProducts() {
    // get user role
    const userRole = this.authService.userRole$.getValue();
    // get user id
    const userId = this.authService.getUserId();

    switch (userRole) {
      case 'artisan':
        return this.http
          .get<Products>(`${this.apiUrl}/${userId}/products`)
          .pipe(
            tap((products) => {
              console.log('Products:', products);

              this.products$.next(products);
            })
          );
        break;
      case 'customer':
      default:
        throw new Error('Invalid user role');
    }
  }

  getProductById(id: string) {
    return this.http
      .get<Product>(`${this.apiUrl}/products/${id}`)
      .pipe(tap((product) => {}));
  }

  createProduct(product: Product) {
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

  updateProduct(id: string, updatedProduct: Product) {}

  deleteProduct(id: string) {}

  filterProducts(
    products: Products,
    filters: {
      name?: string;
      category?: string;
      status?: string;
      availability?: boolean;
      rating?: number;
    }
  ): Products {
    return products.filter((product) => {
      const matchesName =
        !filters.name ||
        product.name.toLowerCase().includes(filters.name.toLowerCase());
      const matchesCategory =
        !filters.category || product.category.name === filters.category;
      const matchesStatus =
        !filters.status || product.status === filters.status;
      const matchesRating =
        filters.rating == null || product.rating >= filters.rating;
      const matchesAvailability =
        !filters.availability ||
        product.availability === filters.availability.toString();

      return (
        matchesName &&
        matchesCategory &&
        matchesStatus &&
        matchesAvailability &&
        matchesRating
      );
    });
  }
}
