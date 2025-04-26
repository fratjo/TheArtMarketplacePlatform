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

  public userRole$: BehaviorSubject<string | null> = new BehaviorSubject<
    string | null
  >(this.getUserRole());

  private apiUrl = 'http://localhost:5140/api/auth';

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

  isTokenExpired() {
    const token = this.getToken();
    if (token) {
      const payload = JSON.parse(atob(token.split('.')[1])); // Décoder le payload du token
      const expirationDate = new Date(payload.exp * 1000); // Convertir l'expiration en millisecondes
      return expirationDate < new Date(); // Vérifier si le token est expiré
    }
    return true; // Si pas de token, considéré comme expiré
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

  getUserId() {
    const token = this.getToken();
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1])); // Décoder le payload du token
        return payload[
          'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
        ] as string;
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
