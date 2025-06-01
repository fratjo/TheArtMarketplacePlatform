import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { Deliveries } from '../models/order.interface';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';

@Injectable({
  providedIn: 'root',
})
export class DeliveryPartnerService {
  private apiUrl = 'http://localhost:5140/api/delivery-partners';

  constructor(private authService: AuthService, private http: HttpClient) {}

  getDeliveries() {
    // get user id
    const userId = this.authService.getUserId();

    return this.http.get<Deliveries>(`${this.apiUrl}/${userId}/deliveries`);
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
