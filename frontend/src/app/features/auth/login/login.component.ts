import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="login-container">
      <mat-card class="login-card">
        <mat-card-header>
          <mat-card-title>Login</mat-card-title>
          <mat-card-subtitle>Enter your credentials to access the questionnaire system</mat-card-subtitle>
        </mat-card-header>
        
        <mat-card-content>
          <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email</mat-label>
              <input matInput type="email" formControlName="email" placeholder="Enter your email">
              <mat-icon matSuffix>email</mat-icon>
              <mat-error *ngIf="loginForm.get('email')?.hasError('required')">
                Email is required
              </mat-error>
              <mat-error *ngIf="loginForm.get('email')?.hasError('email')">
                Please enter a valid email
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Password</mat-label>
              <input matInput [type]="hidePassword ? 'password' : 'text'" formControlName="password" placeholder="Enter your password">
              <button mat-icon-button matSuffix (click)="hidePassword = !hidePassword" type="button">
                <mat-icon>{{hidePassword ? 'visibility_off' : 'visibility'}}</mat-icon>
              </button>
              <mat-error *ngIf="loginForm.get('password')?.hasError('required')">
                Password is required
              </mat-error>
            </mat-form-field>

            <div class="button-container">
              <button mat-raised-button color="primary" type="submit" [disabled]="loginForm.invalid || isLoading" class="full-width">
                <mat-icon *ngIf="!isLoading">login</mat-icon>
                <mat-spinner diameter="20" *ngIf="isLoading"></mat-spinner>
                {{ isLoading ? 'Logging in...' : 'Login' }}
              </button>
            </div>
          </form>
        </mat-card-content>
        
        <mat-card-actions>
          <button mat-button routerLink="/register">Don't have an account? Register</button>
        </mat-card-actions>
      </mat-card>
    </div>
  `,
  styles: [`
    .login-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      padding: 20px;
    }

    .login-card {
      max-width: 400px;
      width: 100%;
      box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
      border-radius: 16px;
    }

    .full-width {
      width: 100%;
      margin-bottom: 16px;
    }

    .button-container {
      margin-top: 24px;
    }

    mat-card-header {
      text-align: center;
      margin-bottom: 24px;
    }

    mat-card-title {
      font-size: 24px;
      font-weight: 600;
      color: #333;
    }

    mat-card-subtitle {
      color: #666;
      margin-top: 8px;
    }

    mat-card-actions {
      display: flex;
      justify-content: center;
      padding: 16px;
    }
  `]
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = false;
  hidePassword = true;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading = true;
      const credentials = this.loginForm.value;

      this.authService.login(credentials).subscribe({
        next: (response) => {
          if (response.success) {
            this.snackBar.open('Login successful!', 'Close', { duration: 3000 });
            this.router.navigate(['/categories']);
          } else {
            this.snackBar.open(response.message || 'Login failed', 'Close', { duration: 5000 });
          }
        },
        error: (error) => {
          this.snackBar.open(error.message || 'Login failed. Please try again.', 'Close', { duration: 5000 });
        },
        complete: () => {
          this.isLoading = false;
        }
      });
    }
  }
} 