import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ArtisanRegistration, Login } from '../models/login.interface';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'http://localhost:5140/api/auth'; // Replace with your API URL

  constructor(private http: HttpClient) {}

  login(login: Login) {
    return this.http.post(`${this.apiUrl}/login`, login);
  }

  registerArtisan(artisan: ArtisanRegistration) {
    console.log(artisan);

    return this.http.post(`${this.apiUrl}/register/artisan`, artisan);
  }

  checkIfUsernameExist(username: string) {
    return this.http.get<boolean>(`${this.apiUrl}/username`, {
      params: { username },
    });
  }

  checkIfEmailExist(email: string) {
    return this.http.get<boolean>(`${this.apiUrl}/email`, {
      params: { email },
    });
  }
}
