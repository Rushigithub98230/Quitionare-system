import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from './api.service';
import { User, LoginDto, RegisterDto, AuthResponseDto } from '../models/user.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private apiService: ApiService) {
    this.loadUserFromStorage();
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

  login(loginDto: LoginDto): Observable<User> {
    // Return seeded admin user since authentication is disabled
    const adminUser: User = {
      id: '00000000-0000-0000-0000-000000000001',
      firstName: 'Admin',
      lastName: 'User',
      email: 'admin@questionnaire.com',
      role: 'Admin',
      category: 'System Administrator',
      createdAt: new Date().toISOString()
    };

    localStorage.setItem('token', 'hardcoded-admin-token');
    localStorage.setItem('refreshToken', 'hardcoded-refresh-token');
    localStorage.setItem('user', JSON.stringify(adminUser));
    this.currentUserSubject.next(adminUser);

    return new Observable(observer => {
      observer.next(adminUser);
      observer.complete();
    });
  }

  register(registerDto: RegisterDto): Observable<User> {
    // Return seeded admin user since authentication is disabled
    const adminUser: User = {
      id: '00000000-0000-0000-0000-000000000001',
      firstName: 'Admin',
      lastName: 'User',
      email: 'admin@questionnaire.com',
      role: 'Admin',
      category: 'System Administrator',
      createdAt: new Date().toISOString()
    };

    localStorage.setItem('token', 'hardcoded-admin-token');
    localStorage.setItem('refreshToken', 'hardcoded-refresh-token');
    localStorage.setItem('user', JSON.stringify(adminUser));
    this.currentUserSubject.next(adminUser);

    return new Observable(observer => {
      observer.next(adminUser);
      observer.complete();
    });
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    this.currentUserSubject.next(null);
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    // TODO: Re-enable authentication for production
    // For now, always return true for admin access
    return true;
  }

  isAdmin(): boolean {
    // TODO: Re-enable authentication for production
    // For now, always return true for admin access
    return true;
  }

  refreshToken(): Observable<AuthResponseDto> {
    const refreshToken = localStorage.getItem('refreshToken');
    if (!refreshToken) {
      throw new Error('No refresh token available');
    }

    return this.apiService.post<AuthResponseDto>('/auth/refresh', { refreshToken }).pipe(
      map(response => {
        if (response.success && response.data) {
          localStorage.setItem('token', response.data.token);
          localStorage.setItem('refreshToken', response.data.refreshToken);
          localStorage.setItem('user', JSON.stringify(response.data.user));
          this.currentUserSubject.next(response.data.user);
          return response.data;
        }
        throw new Error(response.message || 'Token refresh failed');
      })
    );
  }

  validateToken(): Observable<boolean> {
    return this.apiService.post<ApiResponse>('/auth/validate', {}).pipe(
      map(response => response.success)
    );
  }
} 