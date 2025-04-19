import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  ArtisanRegistration,
  AuthResponse,
  CheckExist,
  CustomerRegistration,
  DeliveryPartnerRegistration,
  Login,
} from '../models/auth.interface';
import { BehaviorSubject, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  public isLoggedIn$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    this.isLoggedIn()
  );

  private apiUrl = 'http://localhost:5140/api/auth'; // Replace with your API URL

  constructor(private http: HttpClient) {}

  login(login: Login) {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, login);
  }

  logout() {
    this.removeToken();
    this.isLoggedIn$.next(false);
  }

  registerArtisan(artisan: ArtisanRegistration) {
    console.log(artisan);

    return this.http.post<AuthResponse>(
      `${this.apiUrl}/register/artisan`,
      artisan
    );
  }

  registerCustomer(customer: CustomerRegistration) {
    return this.http.post<AuthResponse>(
      `${this.apiUrl}/register/customer`,
      customer
    );
  }

  registerDeliveryPartner(delivery_partner: DeliveryPartnerRegistration) {
    return this.http.post<AuthResponse>(
      `${this.apiUrl}/register/delivery-partner`,
      delivery_partner
    );
  }

  saveToken(token: string) {
    localStorage.setItem('token', token);
  }

  getToken() {
    return localStorage.getItem('token');
  }

  removeToken() {
    localStorage.removeItem('token');
  }

  isLoggedIn() {
    return !!this.getToken();
  }

  getUserRole() {
    const token = this.getToken();
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1])); // Décoder le payload du token
        return (
          (
            payload[
              'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
            ] as string
          ).toLocaleLowerCase() || null
        );
      } catch (error) {
        console.error('Error decoding token:', error);
        return null;
      }
    }
    return null;
  }

  checkIfUsernameExist(username: string) {
    return this.http
      .get<CheckExist>(`${this.apiUrl}/username`, {
        params: { username },
      })
      .pipe(
        tap((response) => {
          console.log('Username existence check response:', response);
        })
      );
  }

  checkIfEmailExist(email: string) {
    return this.http.get<CheckExist>(`${this.apiUrl}/email`, {
      params: { email },
    });
  }
}
