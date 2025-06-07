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
import {
  BehaviorSubject,
  catchError,
  finalize,
  Observable,
  shareReplay,
  tap,
  throwError,
} from 'rxjs';
import { ToastService } from './toast.service';
import { Router } from '@angular/router';

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

  constructor(
    private http: HttpClient,
    private toastService: ToastService,
    private router: Router
  ) {}

  login(login: Login) {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, login);
  }

  logout() {
    console.log('[AuthService] Logging out...');

    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      this.removeToken();
      localStorage.removeItem('refreshToken');
      this.isLoggedIn$.next(false);
      this.userRole$.next(null);
      return;
    }

    this.http
      .post(
        `${this.apiUrl}/logout`,
        {
          token: refreshToken,
        },
        {
          headers: { 'Content-Type': 'application/json' },
        }
      )
      .subscribe({
        next: () => {
          this.removeToken();
          localStorage.removeItem('refreshToken');
          this.isLoggedIn$.next(false);
          this.userRole$.next(null);
          this.router.navigate(['/']);
          this.toastService.show({
            text: 'Logout successful',
            classname: 'bg-success text-light',
            delay: 2000,
          });
        },
        error: (err) => {
          this.removeToken();
          localStorage.removeItem('refreshToken');
          this.isLoggedIn$.next(false);
          this.userRole$.next(null);
          this.router.navigate(['/']);
          this.toastService.show({
            text: 'Logout successful',
            classname: 'bg-success text-light',
            delay: 2000,
          });
        },
      });
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

  getToken() {
    return localStorage.getItem('token');
  }

  getRefreshToken() {
    return localStorage.getItem('refreshToken');
  }

  removeToken() {
    localStorage.removeItem('token');
  }

  isLoggedIn() {
    return !!this.getToken(); // Vérifie si un token est présent, si il l'est, l'utilisateur est connecté
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

  changePassword(currentPassword: string, newPassword: string) {
    return this.http.post(`${this.apiUrl}/change-password`, {
      currentPassword: currentPassword,
      newPassword: newPassword,
    });
  }

  saveTokens(tokens: AuthResponse) {
    localStorage.setItem('token', tokens.token);
    localStorage.setItem('refreshToken', tokens.refreshToken);
    this.isLoggedIn$.next(true);
    this.userRole$.next(this.getUserRole());
    console.log('[AuthService] Tokens saved successfully');
  }

  private refreshInProgress: Observable<AuthResponse> | null = null;
  refreshToken() {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      return throwError(() => new Error('No refresh token'));
    }
    if (!this.refreshInProgress) {
      this.refreshInProgress = this.http
        .post<AuthResponse>(`${this.apiUrl}/refresh-token`, {
          token: refreshToken,
        })
        .pipe(
          finalize(() => (this.refreshInProgress = null)),
          shareReplay(1)
        );
    }
    return this.refreshInProgress;
  }
}
