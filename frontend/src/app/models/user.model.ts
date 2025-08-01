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
  category?: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

export interface LoginResponse {
  data: AuthResponse;
  message: string;
  statusCode: number;
  success: boolean;
  timestamp: string;
} 