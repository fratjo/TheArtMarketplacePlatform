import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AnyUserProfile } from '../models/user.interface';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  constructor(private http: HttpClient, private authService: AuthService) {}

  getProfile() {
    const userId = this.authService.getUserId();
    const userRole = this.authService.getUserRole();

    return this.http.get<AnyUserProfile>(
      `http://localhost:5140/api/${userRole}s/${userId}`
    );
  }

  updateProfile(profile: AnyUserProfile) {
    const userId = this.authService.getUserId();
    const userRole = this.authService.getUserRole();

    return this.http.put<AnyUserProfile>(
      `http://localhost:5140/api/${userRole}s/${userId}`,
      profile
    );
  }
}
