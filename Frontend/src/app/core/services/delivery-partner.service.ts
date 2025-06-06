import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { Deliveries } from '../models/order.interface';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';

@Injectable({
  providedIn: 'root',
})
export class DeliveryPartnerService {
  private apiUrl = 'http://localhost:5140/api/deliverypartners';

  constructor(private authService: AuthService, private http: HttpClient) {}

  getDeliveries(filters?: {
    status?: string;
    year?: number;
    sortBy?: string | null;
    sortOrder?: 'asc' | 'desc';
  }) {
    // get user id
    const userId = this.authService.getUserId();

    if (!filters) {
      filters = {};
    }

    let params = new HttpParams();
    if (filters.status) {
      params = params.set('status', filters.status);
    }
    if (filters.year) {
      params = params.set('year', filters.year.toString());
    }
    if (filters.sortBy) {
      params = params.set('sortBy', filters.sortBy);
    }
    if (filters.sortOrder) {
      params = params.set('sortOrder', filters.sortOrder);
    }

    return this.http.get<Deliveries>(`${this.apiUrl}/${userId}/deliveries`, {
      params,
    });
  }

  setDeliveryAsDelivered(deliveryId: string) {
    // get user id
    const userId = this.authService.getUserId();

    return this.http.post(
      `${this.apiUrl}/${userId}/deliveries/${deliveryId}/set-as-delivered`,
      {}
    );
  }
}
