import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService, User } from 'src/app/services/auth.service';


@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule
  ],
  template: `
    <div class="profile-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>User Profile</mat-card-title>
          <mat-card-subtitle>Manage your account information</mat-card-subtitle>
        </mat-card-header>
        <mat-card-content>
          <form [formGroup]="profileForm" (ngSubmit)="onSubmit()">
            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>First Name</mat-label>
                <input matInput formControlName="firstName" required>
              </mat-form-field>
              
              <mat-form-field appearance="outline">
                <mat-label>Last Name</mat-label>
                <input matInput formControlName="lastName" required>
              </mat-form-field>
            </div>
            
            <mat-form-field appearance="outline">
              <mat-label>Email</mat-label>
              <input matInput formControlName="email" type="email" required>
            </mat-form-field>
            
            <mat-form-field appearance="outline">
              <mat-label>Category</mat-label>
              <input matInput formControlName="category" readonly>
            </mat-form-field>
            
            <div class="profile-info">
              <p><strong>Role:</strong> {{ currentUser?.role }}</p>
              <p><strong>Account Status:</strong> 
                <span [class]="currentUser?.isActive ? 'status-active' : 'status-inactive'">
                  {{ currentUser?.isActive ? 'Active' : 'Inactive' }}
                </span>
              </p>
              <p><strong>Member Since:</strong> {{ currentUser?.createdAt | date }}</p>
              <p *ngIf="currentUser?.lastLoginAt"><strong>Last Login:</strong> {{ currentUser?.lastLoginAt | date }}</p>
            </div>
            
            <div class="form-actions">
              <button mat-raised-button color="primary" type="submit" [disabled]="!profileForm.valid">
                <mat-icon>save</mat-icon>
                Update Profile
              </button>
              <button mat-button type="button" (click)="resetForm()">
                <mat-icon>refresh</mat-icon>
                Reset
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .profile-container {
      padding: 20px;
      max-width: 600px;
      margin: 0 auto;
    }
    .form-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 20px;
      margin-bottom: 20px;
    }
    mat-form-field {
      width: 100%;
      margin-bottom: 20px;
    }
    .profile-info {
      background: #f5f5f5;
      padding: 20px;
      border-radius: 8px;
      margin: 20px 0;
    }
    .profile-info p {
      margin: 10px 0;
    }
    .status-active {
      color: #4caf50;
      font-weight: bold;
    }
    .status-inactive {
      color: #f44336;
      font-weight: bold;
    }
    .form-actions {
      display: flex;
      gap: 10px;
      margin-top: 20px;
    }
  `]
})
export class ProfileComponent implements OnInit {
  profileForm!: FormGroup;
  currentUser: User | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    this.initForm();
  }

  initForm(): void {
    this.profileForm = this.fb.group({
      firstName: [this.currentUser?.firstName || '', Validators.required],
      lastName: [this.currentUser?.lastName || '', Validators.required],
      email: [this.currentUser?.email || '', [Validators.required, Validators.email]],
      category: [this.currentUser?.category || '', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.profileForm.valid) {
      console.log('Profile update:', this.profileForm.value);
      // TODO: Implement profile update API call
    }
  }

  resetForm(): void {
    this.initForm();
  }
} 