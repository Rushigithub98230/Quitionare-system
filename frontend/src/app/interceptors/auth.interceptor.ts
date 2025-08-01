import { HttpRequest, HttpHandlerFn } from '@angular/common/http';
import { inject } from '@angular/core';

export function AuthInterceptor(request: HttpRequest<unknown>, next: HttpHandlerFn) {
  const token = localStorage.getItem('auth_token');
  
  console.log('AuthInterceptor - URL:', request.url);
  console.log('AuthInterceptor - Token present:', !!token);
  
  if (token) {
    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }
  
  return next(request);
} 