import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { ToastService } from '../services/toast.service';
import { catchError, switchMap, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const toastService = inject(ToastService);

  if (req.url.includes('/refresh-token')) {
    return next(req);
  }

  let accessToken = authService.getToken();

  // Si pas connecté, on continue sans rien
  if (!authService.isLoggedIn()) return next(req);

  // Token non expiré, on ajoute simplement le header
  if (!authService.isTokenExpired()) {
    console.log(
      '[AuthInterceptor] Token non expiré, ajout du header Authorization'
    );

    const cloned = req.clone({
      setHeaders: { Authorization: `Bearer ${accessToken}` },
    });
    return next(cloned);
  }

  console.log('[AuthInterceptor] Token expiré, tentative de rafraîchissement');

  // Token expiré, on refresh le token
  return authService.refreshToken().pipe(
    switchMap((tokens) => {
      authService.saveTokens(tokens);
      const retryReq = req.clone({
        setHeaders: { Authorization: `Bearer ${tokens.token}` },
      });
      console.log('[AuthInterceptor] Rejoue la requête avec le nouveau token');

      return next(retryReq);
    }),
    catchError((refreshError) => {
      console.error('[AuthInterceptor] Retry a échoué aussi : logout forcé');
      console.log(
        '[AuthInterceptor] Erreur lors du rafraîchissement du token',
        refreshError
      );

      alert();
      // authService.logout();
      toastService.show({
        text: 'Session expirée. Veuillez vous reconnecter.',
        classname: 'bg-danger text-light',
        delay: 5000,
      });
      return throwError(() => refreshError);
    })
  );
};
