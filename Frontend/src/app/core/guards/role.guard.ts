import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export function roleGuard(role?: string): CanActivateFn {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (!authService.isLoggedIn()) {
      router.navigate(['/login']);
      return false;
    }

    if (!role || role === '' || role === undefined) {
      return true; // No role specified, allow access
    }

    const userRole = authService.getUserRole();

    if (role !== userRole) {
      router.navigate(['/unauthorized']);
      return false;
    }

    return true;
  };
}
