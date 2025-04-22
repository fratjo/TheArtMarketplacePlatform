import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { LoggedInGuard } from './core/guards/logged-in.guard';

export const routes: Routes = [
  {
    path: '',
    canActivate: [LoggedInGuard],
    loadComponent: () =>
      import('./features/landing/landing.component').then(
        (c) => c.LandingComponent
      ),
  },
  {
    path: 'login',
    canActivate: [LoggedInGuard],
    loadComponent: () =>
      import('./features/login/login.component').then((c) => c.LoginComponent),
  },
  {
    path: 'register',
    canActivate: [LoggedInGuard],
    loadComponent: () =>
      import('./features/register/register.component').then(
        (c) => c.RegisterComponent
      ),
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./features/dashboard/dashboard.component').then(
        (c) => c.DashboardComponent
      ),
    canActivate: [AuthGuard],
    children: [
      {
        path: 'artisan',
        loadComponent: () =>
          import(
            './features/artisan/artisan-dashboard/artisan-dashboard.component'
          ).then((c) => c.ArtisanDashboardComponent),
      },
      {
        path: 'customer',
        loadComponent: () =>
          import(
            './features/customer/customer-dashboard/customer-dashboard.component'
          ).then((c) => c.CustomerDashboardComponent),
      },
      {
        path: 'delivery-partner',
        loadComponent: () =>
          import(
            './features/delivery-partner/delivery-partner-dashboard/delivery-partner-dashboard.component'
          ).then((c) => c.DeliveryPartnerDashboardComponent),
      },
    ],
  },
  {
    path: 'products',
    loadComponent: () =>
      import('./features/products/products.component').then(
        (c) => c.ProductsComponent
      ),
    children: [
      {
        path: 'my-products',
        loadComponent: () =>
          import(
            './features/artisan/products/my-products/my-products.component'
          ).then((c) => c.MyProductsComponent),
      },
      {
        path: 'product-catalog',
        loadComponent: () =>
          import(
            './features/products/product-catalog/product-catalog.component'
          ).then((c) => c.ProductCatalogComponent),
      },
    ],
  },
  {
    path: 'unauthorized',
    loadComponent: () =>
      import('./features/unauthorized/unauthorized.component').then(
        (c) => c.UnauthorizedComponent
      ),
  },
  {
    path: '**',
    redirectTo: '',
  },
];
