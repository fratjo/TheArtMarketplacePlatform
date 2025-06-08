import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Product, Products } from '../models/product.interface';
import { UserFullProfiles } from '../models/user.interface';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private apiUrl = 'http://localhost:5140/api/admin';

  constructor(private http: HttpClient) {}

  getAllUsers(filters?: {
    search?: string;
    status?: string;
    isDeleted?: boolean;
    role?: string;
    sortBy?: string;
    sortOrder?: 'asc' | 'desc';
  }) {
    let params = new HttpParams();

    if (filters) {
      if (filters.search) {
        params = params.set('search', filters.search);
      }
      if (filters.status) {
        params = params.set('status', filters.status);
      }
      if (filters.isDeleted !== undefined) {
        params = params.set('isDeleted', filters.isDeleted.toString());
      }
      if (filters.role) {
        params = params.set('role', filters.role);
      }
      if (filters.sortBy) {
        params = params.set('sortBy', filters.sortBy);
      }
      if (filters.sortOrder) {
        params = params.set('sortOrder', filters.sortOrder);
      }
    }

    return this.http.get<UserFullProfiles>(`${this.apiUrl}/users`, { params });
  }

  getAllProducts(filters?: {
    search?: string;
    artisan?: string;
    category?: string;
    sortBy?: string;
    sortOrder?: 'asc' | 'desc';
  }) {
    let params = new HttpParams();

    if (filters) {
      if (filters.search) {
        params = params.set('search', filters.search);
      }
      if (filters.artisan) {
        params = params.set('artisan', filters.artisan);
      }
      if (filters.category) {
        params = params.set('category', filters.category);
      }
      if (filters.sortBy) {
        params = params.set('sortBy', filters.sortBy);
      }
      if (filters.sortOrder) {
        params = params.set('sortOrder', filters.sortOrder);
      }
    }

    console.log('Fetching products with filters:', filters);

    return this.http.get<Products>(`${this.apiUrl}/products`, { params });
  }

  setUserDeleted(userId: string) {
    return this.http.delete(`${this.apiUrl}/users/${userId}`);
  }

  setUserStatus(userId: string) {
    return this.http.delete(`${this.apiUrl}/users/${userId}/deactivate`);
  }

  setDeleteProduct(productId: string) {
    return this.http.delete(`${this.apiUrl}/products/${productId}`);
  }
}
