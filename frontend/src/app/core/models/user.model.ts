export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  category?: string;
  createdAt: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
  role: string;
  category?: string;
}

export interface AuthResponseDto {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

export interface RefreshTokenDto {
  refreshToken: string;
} 