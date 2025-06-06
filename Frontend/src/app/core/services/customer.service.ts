import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { Order, Orders } from '../models/order.interface';
import { ProductFavorite, ProductFavorites } from '../models/product.interface';

@Injectable({
  providedIn: 'root',
})
export class CustomerService {
  constructor(private http: HttpClient, private authService: AuthService) {}

  createOrder(order: any) {
    // get user id
    const userId = this.authService.getUserId();

    return this.http.post(
      `http://localhost:5140/api/customers/${userId}/orders`,
      order
    );
  }

  getOrders() {
    // get user id
    const userId = this.authService.getUserId();

    return this.http.get<Orders>(
      `http://localhost:5140/api/customers/${userId}/orders`
    );
  }

  getOrderById(orderId: string) {
    // get user id
    const userId = this.authService.getUserId();

    return this.http.get<Order>(
      `http://localhost:5140/api/customers/${userId}/orders/${orderId}`
    );
  }

  checkIfAlreadyBoughtProductAndNotReviewed(productId: string) {
    // get user id
    const userId = this.authService.getUserId();

    return this.http.get<boolean>(
      `http://localhost:5140/api/customers/${userId}/already-bought-reviewed/${productId}`
    );
  }

  reviewProduct(productId: string, review: any) {
    // get user id
    const userId = this.authService.getUserId();

    return this.http.post(
      `http://localhost:5140/api/customers/${userId}/review-product`,
      {
        productId: productId,
        rating: review.score,
        review: review.comment,
      }
    );
  }

  getFavoriteProducts() {
    // get user id
    const userId = this.authService.getUserId();

    return this.http.get<ProductFavorites>(
      `http://localhost:5140/api/customers/${userId}/products/favorites`
    );
  }

  addProductToFavorites(productId: string) {
    // get user id
    const userId = this.authService.getUserId();

    return this.http.post<ProductFavorite>(
      `http://localhost:5140/api/customers/${userId}/products/favorites/${productId}`,
      {}
    );
  }

  removeProductFromFavorites(productId: string) {
    // get user id
    const userId = this.authService.getUserId();

    return this.http.delete<ProductFavorite>(
      `http://localhost:5140/api/customers/${userId}/products/favorites/${productId}`
    );
  }
}
