import { HttpRequest, HttpHandlerFn } from '@angular/common/http';
import { inject } from '@angular/core';

export function AuthInterceptor(request: HttpRequest<unknown>, next: HttpHandlerFn) {
  const token = localStorage.getItem('token');
  
  if (token) {
    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }
  
  return next(request);
} 