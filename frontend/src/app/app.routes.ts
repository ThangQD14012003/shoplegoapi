import { Routes } from '@angular/router';
import { adminGuard, authGuard } from './core/auth.guard';
export const routes: Routes = [
 {path:'',loadComponent:()=>import('./layouts/storefront-layout').then(m=>m.StorefrontLayout),children:[
  {path:'',loadComponent:()=>import('./pages/catalog/catalog-page').then(m=>m.CatalogPage)},
  {path:'products/:id',loadComponent:()=>import('./pages/product-detail/product-detail-page').then(m=>m.ProductDetailPage)},
  {path:'cart',loadComponent:()=>import('./pages/cart/cart-page').then(m=>m.CartPage)},
  {path:'checkout',canActivate:[authGuard],loadComponent:()=>import('./pages/checkout/checkout-page').then(m=>m.CheckoutPage)},
  {path:'account',canActivate:[authGuard],loadComponent:()=>import('./pages/account/account-page').then(m=>m.AccountPage)}]},
 {path:'auth',loadComponent:()=>import('./pages/auth/auth-page').then(m=>m.AuthPage)},
 {path:'admin',canActivate:[adminGuard],loadComponent:()=>import('./layouts/admin-layout').then(m=>m.AdminLayout),children:[
  {path:'',loadComponent:()=>import('./pages/admin-dashboard/admin-dashboard-page').then(m=>m.AdminDashboardPage)},
  {path:'products',loadComponent:()=>import('./pages/admin-products/admin-products-page').then(m=>m.AdminProductsPage)},
  {path:'orders',loadComponent:()=>import('./pages/admin-orders/admin-orders-page').then(m=>m.AdminOrdersPage)},
  {path:'settings',loadComponent:()=>import('./pages/admin-settings/admin-settings-page').then(m=>m.AdminSettingsPage)}]},
 {path:'**',redirectTo:''}
];
