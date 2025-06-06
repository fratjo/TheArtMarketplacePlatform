import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { LoggedInGuard } from './core/guards/logged-in.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  {
    path: '',
    canActivate: [],
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
    path: 'profile',
    canActivate: [roleGuard()],
    loadComponent: () =>
      import('./features/profile/profile.component').then(
        (c) => c.ProfileComponent
      ),
  },
  {
    path: 'artisan',
    canActivate: [roleGuard('artisan')],
    children: [
      {
        path: 'products',
        loadComponent: () =>
          import(
            './features/artisan/products/my-products/my-products.component'
          ).then((c) => c.MyProductsComponent),
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import(
            './features/artisan/artisan-dashboard/artisan-dashboard.component'
          ).then((c) => c.ArtisanDashboardComponent),
      },
      {
        path: 'orders',
        loadComponent: () =>
          import('./features/artisan/orders/orders/orders.component').then(
            (c) => c.OrdersComponent
          ),
      },
      {
        path: 'orders/:id',
        loadComponent: () =>
          import(
            './features/artisan/orders/order-details/order-details.component'
          ).then((c) => c.OrderDetailsComponent),
      },
    ],
  },
  {
    path: 'customer',
    canActivate: [roleGuard('customer')],
    children: [
      {
        path: 'orders',
        loadComponent: () =>
          import(
            './features/customer/orders/my-orders/my-orders.component'
          ).then((c) => c.MyOrdersComponent),
      },
      {
        path: 'orders/:id',
        loadComponent: () =>
          import(
            './features/customer/orders/order-details/order-details.component'
          ).then((c) => c.OrderDetailsComponent),
      },
      {
        path: 'favorites',
        loadComponent: () =>
          import('./features/customer/favorites/favorites.component').then(
            (c) => c.FavoritesComponent
          ),
      },
    ],
  },
  {
    path: 'delivery-partner',
    canActivate: [roleGuard('deliverypartner')],
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import(
            './features/delivery-partner/delivery-partner-dashboard/delivery-partner-dashboard.component'
          ).then((c) => c.DeliveryPartnerDashboardComponent),
      },
      {
        path: 'deliveries',
        loadComponent: () =>
          import(
            './features/delivery-partner/deliveries/deliveries/deliveries.component'
          ).then((c) => c.DeliveriesComponent),
      },
    ],
  },
  {
    path: 'cart',
    loadComponent: () =>
      import('./features/cart/cart.component').then((c) => c.CartComponent),
  },
  {
    path: 'checkout',
    canActivate: [AuthGuard],
    loadComponent: () =>
      import('./features/checkout/checkout.component').then(
        (c) => c.CheckoutComponent
      ),
  },
  {
    path: 'order-confirmation',
    canActivate: [AuthGuard],
    loadComponent: () =>
      import('./features/valid-purchase/valid-purchase.component').then(
        (c) => c.ValidPurchaseComponent
      ),
  },
  {
    path: 'products',
    children: [
      {
        path: 'catalog',
        loadComponent: () =>
          import(
            './features/products/product-catalog/product-catalog.component'
          ).then((c) => c.ProductCatalogComponent),
      },
      {
        path: 'new',
        canActivate: [roleGuard('artisan')],
        loadComponent: () =>
          import(
            './features/artisan/products/product-form-page/product-form-page.component'
          ).then((c) => c.ProductFormPageComponent),
      },
      {
        path: ':id',
        children: [
          {
            path: 'details',
            loadComponent: () =>
              import(
                './features/products/product-details/product-details.component'
              ).then((c) => c.ProductDetailsComponent),
          },
          {
            path: 'edit',
            canActivate: [roleGuard('artisan')],
            loadComponent: () =>
              import(
                './features/artisan/products/product-form-page/product-form-page.component'
              ).then((c) => c.ProductFormPageComponent),
          },
        ],
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
