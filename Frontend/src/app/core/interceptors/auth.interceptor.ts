import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { ToastService } from '../services/toast.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const toastService = inject(ToastService);

  if (!authService.isLoggedIn() || authService.isTokenExpired()) {
    authService.logout();
    toastService.show({
      text: 'Session expired. Please log in again.',
      classname: 'bg-danger text-light',
      delay: 5000,
    });
    return next(req);
  } else {
    const token = authService.getToken();
    const clonedRequest = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
    return next(clonedRequest);
  }
};
