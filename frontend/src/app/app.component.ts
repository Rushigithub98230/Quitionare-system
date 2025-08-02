import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService } from './core/services/auth.service';
import { User } from './core/models/user.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule
  ],
  template: `
    <div class="app-container">
      <!-- Top Navigation Bar -->
      <mat-toolbar color="primary" class="toolbar">
        <span class="app-title">
          <mat-icon class="app-icon">quiz</mat-icon>
          Questionnaire System - Admin Panel
        </span>
        
        <span class="spacer"></span>
        
        <!-- User Menu -->
        <div *ngIf="currentUser" class="user-section">
          <button mat-icon-button [matMenuTriggerFor]="userMenu" class="user-menu-button">
            <mat-icon>account_circle</mat-icon>
          </button>
          
          <mat-menu #userMenu="matMenu">
            <button mat-menu-item (click)="navigateToProfile()">
              <mat-icon>person</mat-icon>
              <span>Profile</span>
            </button>
            <button mat-menu-item (click)="logout()">
              <mat-icon>logout</mat-icon>
              <span>Logout</span>
            </button>
          </mat-menu>
        </div>
        
        <!-- Login/Register buttons for unauthenticated users -->
        <div *ngIf="!currentUser" class="auth-buttons">
          <button mat-button routerLink="/login">
            <mat-icon>login</mat-icon>
            Login
          </button>
          <button mat-raised-button color="accent" routerLink="/register">
            <mat-icon>person_add</mat-icon>
            Register
          </button>
        </div>
      </mat-toolbar>

      <!-- Main Content -->
      <div class="main-content">
        <router-outlet></router-outlet>
      </div>
    </div>
  `,
  styles: [`
    .app-container {
      height: 100vh;
      display: flex;
      flex-direction: column;
      background-color: var(--background-color);
    }

    .toolbar {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      z-index: 1000;
      display: flex;
      align-items: center;
      padding: 0 24px;
      background-color: var(--background-color);
      border-bottom: 1px solid var(--border-color);
      box-shadow: 0 2px 4px var(--shadow-color);
    }

    .app-title {
      display: flex;
      align-items: center;
      font-size: 1.3rem;
      font-weight: 600;
      color: var(--primary-color);
    }

    .app-icon {
      margin-right: 12px;
      color: var(--primary-color);
    }

    .spacer {
      flex: 1 1 auto;
    }

    .user-section {
      display: flex;
      align-items: center;
      gap: 16px;
    }

    .user-menu-button {
      margin-left: 8px;
      color: var(--text-primary);
    }

    .auth-buttons {
      display: flex;
      gap: 12px;
    }

    .main-content {
      flex: 1;
      margin-top: 64px;
      background-color: var(--surface-color);
      min-height: calc(100vh - 64px);
      padding: 24px;
    }

    @media (max-width: 768px) {
      .app-title {
        font-size: 1.1rem;
      }
      
      .toolbar {
        padding: 0 16px;
      }
      
      .main-content {
        padding: 16px;
      }
    }
  `]
})
export class AppComponent implements OnInit {
  currentUser: User | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  navigateToProfile() {
    this.router.navigate(['/profile']);
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
} 