import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
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
      id: 1,
      firstName: 'Admin',
      lastName: 'User',
      email: 'admin@questionnaire.com',
      role: 'Admin',
      category: 'System Administrator',
      createdDate: new Date().toISOString()
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
      id: 1,
      firstName: 'Admin',
      lastName: 'User',
      email: 'admin@questionnaire.com',
      role: 'Admin',
      category: 'System Administrator',
      createdDate: new Date().toISOString()
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

  refreshToken(): Observable<ApiResponse<AuthResponseDto>> {
    const refreshToken = localStorage.getItem('refreshToken');
    if (!refreshToken) {
      throw new Error('No refresh token available');
    }

    return this.apiService.post<AuthResponseDto>('/auth/refresh', { refreshToken });
  }

  validateToken(): Observable<ApiResponse<any>> {
    return this.apiService.post<any>('/auth/validate', {});
  }
} 