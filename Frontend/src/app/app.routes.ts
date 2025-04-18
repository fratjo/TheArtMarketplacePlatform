import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/landing/landing.component').then(
        (c) => c.LandingComponent
      ),
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/login/login.component').then((c) => c.LoginComponent),
  },
  {
    path: 'register-artisan',
    loadComponent: () =>
      import(
        './features/register/register-artisan/register-artisan.component'
      ).then((c) => c.RegisterArtisanComponent),
  },
  {
    path: 'register-customer',
    loadComponent: () =>
      import(
        './features/register/register-customer/register-customer.component'
      ).then((c) => c.RegisterCustomerComponent),
  },
  {
    path: 'register-delivery-partner',
    loadComponent: () =>
      import(
        './features/register/register-delivery-partner/register-delivery-partner.component'
      ).then((c) => c.RegisterDeliveryPartnerComponent),
  },
  {
    path: '**',
    redirectTo: '',
  },
];
