import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CustomerService {
  constructor(private http: HttpClient) {}

  createOrder(order: any) {
    return this.http.post('http://localhost:5140/api/customers/orders', order);
  }
}
