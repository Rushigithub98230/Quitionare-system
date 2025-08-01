import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { ApiService, ApiResponse } from './api.service';

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: 'User' | 'Admin';
  category?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
  lastLoginAt?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
  role: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private apiService: ApiService, private router: Router) {
    this.loadUserFromStorage();
  }

  login(credentials: LoginRequest): Observable<ApiResponse<AuthResponse>> {
    return this.apiService.post<AuthResponse>('/auth/login', credentials)
      .pipe(
        tap(response => {
          if (response.success) {
            this.setToken(response.data.token);
            this.setUser(response.data.user);
            this.navigateAfterLogin(response.data.user);
          }
        })
      );
  }

  register(userData: RegisterRequest): Observable<ApiResponse<AuthResponse>> {
    return this.apiService.post<AuthResponse>('/auth/register', userData)
      .pipe(
        tap(response => {
          if (response.success) {
            this.setToken(response.data.token);
            this.setUser(response.data.user);
            this.navigateAfterLogin(response.data.user);
          }
        })
      );
  }

  validateToken(): Observable<ApiResponse<any>> {
    return this.apiService.get<any>('/auth/validate');
  }

  logout(): void {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('user');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  isAdmin(): boolean {
    const user = this.currentUserSubject.value;
    return user?.role === 'Admin';
  }

  getToken(): string | null {
    const token = localStorage.getItem('auth_token');
    console.log('Current auth token:', token ? 'Present' : 'Missing');
    return token;
  }

  private setToken(token: string): void {
    localStorage.setItem('auth_token', token);
  }

  private setUser(user: User): void {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  private navigateAfterLogin(user: User): void {
    if (user.role === 'Admin') {
      this.router.navigate(['/admin']);
    } else {
      this.router.navigate(['/dashboard']);
    }
  }

  private loadUserFromStorage(): void {
    const userStr = localStorage.getItem('user');
    if (userStr) {
      try {
        const user = JSON.parse(userStr);
        this.currentUserSubject.next(user);
      } catch (error) {
        console.error('Error parsing user from storage:', error);
        this.logout();
      }
    }
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }
} 