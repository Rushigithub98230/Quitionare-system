import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { AuthService } from '../../../core/services/auth.service';
import { User } from '../../../core/models/user.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule
  ],
  template: `
    <div class="profile-container">
      <mat-card class="profile-card">
        <mat-card-header>
          <mat-card-title>User Profile</mat-card-title>
          <mat-card-subtitle>Your account information</mat-card-subtitle>
        </mat-card-header>
        
        <mat-card-content *ngIf="currentUser">
          <div class="profile-info">
            <div class="info-row">
              <strong>Name:</strong>
              <span>{{ currentUser.firstName }} {{ currentUser.lastName }}</span>
            </div>
            <div class="info-row">
              <strong>Email:</strong>
              <span>{{ currentUser.email }}</span>
            </div>
            <div class="info-row">
              <strong>Role:</strong>
              <span>{{ currentUser.role }}</span>
            </div>
            <div class="info-row" *ngIf="currentUser.category">
              <strong>Category:</strong>
              <span>{{ currentUser.category }}</span>
            </div>
            <div class="info-row">
              <strong>Member Since:</strong>
              <span>{{ currentUser.createdAt | date:'mediumDate' }}</span>
            </div>
          </div>
        </mat-card-content>
        
        <mat-card-actions>
          <button mat-raised-button color="primary" (click)="goBack()">
            <mat-icon>arrow_back</mat-icon>
            Back to Dashboard
          </button>
        </mat-card-actions>
      </mat-card>
    </div>
  `,
  styles: [`
    .profile-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      padding: 20px;
    }

    .profile-card {
      max-width: 500px;
      width: 100%;
      padding: 24px;
    }

    .profile-info {
      margin: 16px 0;
    }

    .info-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 12px 0;
      border-bottom: 1px solid #eee;
    }

    .info-row:last-child {
      border-bottom: none;
    }

    .info-row strong {
      color: #333;
      font-weight: 500;
    }

    .info-row span {
      color: #666;
    }

    mat-card-actions {
      display: flex;
      justify-content: center;
      padding: 16px 0 0 0;
    }
  `]
})
export class ProfileComponent implements OnInit {
  currentUser: User | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.currentUser = this.authService.getCurrentUser();
    if (!this.currentUser) {
      this.router.navigate(['/login']);
    }
  }

  goBack() {
    this.router.navigate(['/admin']);
  }
} 