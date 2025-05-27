import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { Order, Orders } from '../models/order.interface';

@Injectable({
  providedIn: 'root',
})
export class CustomerService {
  constructor(private http: HttpClient, private authService: AuthService) {}

  createOrder(order: any) {
    return this.http.post('http://localhost:5140/api/customers/orders', order);
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
}
