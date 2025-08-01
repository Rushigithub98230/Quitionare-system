import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatDividerModule } from '@angular/material/divider';
import { MatBadgeModule } from '@angular/material/badge';
import { MatChipsModule } from '@angular/material/chips';
import { AuthService } from './services/auth.service';
import { User } from './models/user.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatSidenavModule,
    MatListModule,
    MatDividerModule,
    MatBadgeModule,
    MatChipsModule
  ],
  template: `
    <div class="app-container">
      <!-- Top Navigation Bar -->
      <mat-toolbar color="primary" class="toolbar">
        <button mat-icon-button (click)="sidenav.toggle()" class="menu-button">
          <mat-icon>menu</mat-icon>
        </button>
        
        <span class="app-title">
          <mat-icon class="app-icon">quiz</mat-icon>
          Questionnaire System
        </span>
        
        <span class="spacer"></span>
        
        <!-- User Menu -->
        <div *ngIf="currentUser" class="user-section">
          <mat-chip-set>
            <mat-chip color="accent" selected>
              <mat-icon>person</mat-icon>
              {{ currentUser.firstName }} {{ currentUser.lastName }}
            </mat-chip>
            <mat-chip *ngIf="currentUser.category" color="primary" selected>
              <mat-icon>category</mat-icon>
              {{ currentUser.category }}
            </mat-chip>
          </mat-chip-set>
          
          <button mat-icon-button [matMenuTriggerFor]="userMenu" class="user-menu-button">
            <mat-icon>account_circle</mat-icon>
          </button>
          
          <mat-menu #userMenu="matMenu">
            <button mat-menu-item (click)="navigateToDashboard()">
              <mat-icon>dashboard</mat-icon>
              <span>Dashboard</span>
            </button>
            <button mat-menu-item (click)="navigateToProfile()">
              <mat-icon>person</mat-icon>
              <span>Profile</span>
            </button>
            <mat-divider></mat-divider>
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

      <!-- Side Navigation -->
      <mat-sidenav-container class="sidenav-container">
        <mat-sidenav #sidenav mode="side" opened class="sidenav">
          <mat-nav-list>
            <!-- Dashboard -->
            <a mat-list-item routerLink="/dashboard" routerLinkActive="active-link">
              <mat-icon matListItemIcon>dashboard</mat-icon>
              <span matListItemTitle>Dashboard</span>
            </a>
            
            <!-- Categories -->
            <a mat-list-item routerLink="/categories" routerLinkActive="active-link">
              <mat-icon matListItemIcon>category</mat-icon>
              <span matListItemTitle>Categories</span>
            </a>
            
            <!-- Questionnaires -->
            <a mat-list-item routerLink="/questionnaires" routerLinkActive="active-link">
              <mat-icon matListItemIcon>quiz</mat-icon>
              <span matListItemTitle>Questionnaires</span>
            </a>
            
            <!-- Admin Section -->
            <mat-divider *ngIf="isAdmin"></mat-divider>
            <div *ngIf="isAdmin" class="admin-section">
              <div class="admin-header">
                <mat-icon>admin_panel_settings</mat-icon>
                <span>Admin Panel</span>
              </div>
              
              <a mat-list-item routerLink="/admin" routerLinkActive="active-link">
                <mat-icon matListItemIcon>settings</mat-icon>
                <span matListItemTitle>Admin Dashboard</span>
              </a>
              
              <a mat-list-item routerLink="/admin/questionnaire-builder" routerLinkActive="active-link">
                <mat-icon matListItemIcon>build</mat-icon>
                <span matListItemTitle>Questionnaire Builder</span>
              </a>
              
              <a mat-list-item routerLink="/admin/categories" routerLinkActive="active-link">
                <mat-icon matListItemIcon>category</mat-icon>
                <span matListItemTitle>Manage Categories</span>
              </a>
            </div>
            
            <!-- User Section -->
            <mat-divider></mat-divider>
            <div class="user-section-nav">
              <div class="user-header">
                <mat-icon>person</mat-icon>
                <span>My Account</span>
              </div>
              
              <a mat-list-item routerLink="/profile" routerLinkActive="active-link">
                <mat-icon matListItemIcon>account_circle</mat-icon>
                <span matListItemTitle>Profile</span>
              </a>
              
              <a mat-list-item routerLink="/my-responses" routerLinkActive="active-link">
                <mat-icon matListItemIcon>history</mat-icon>
                <span matListItemTitle>My Responses</span>
              </a>
            </div>
          </mat-nav-list>
        </mat-sidenav>

        <!-- Main Content -->
        <mat-sidenav-content class="main-content">
          <div class="content-wrapper">
            <router-outlet></router-outlet>
          </div>
        </mat-sidenav-content>
      </mat-sidenav-container>
    </div>
  `,
  styles: [`
    .app-container {
      height: 100vh;
      display: flex;
      flex-direction: column;
    }

    .toolbar {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      z-index: 1000;
      display: flex;
      align-items: center;
      padding: 0 16px;
    }

    .app-title {
      display: flex;
      align-items: center;
      font-size: 1.2rem;
      font-weight: 500;
      margin-left: 8px;
    }

    .app-icon {
      margin-right: 8px;
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
    }

    .auth-buttons {
      display: flex;
      gap: 8px;
    }

    .sidenav-container {
      flex: 1;
      margin-top: 64px;
    }

    .sidenav {
      width: 280px;
      background: #fafafa;
    }

    .main-content {
      background: #f5f5f5;
      min-height: calc(100vh - 64px);
    }

    .content-wrapper {
      padding: 24px;
      max-width: 1200px;
      margin: 0 auto;
    }

    .active-link {
      background-color: rgba(63, 81, 181, 0.1);
      color: #3f51b5;
    }

    .admin-section, .user-section-nav {
      margin-top: 16px;
    }

    .admin-header, .user-header {
      display: flex;
      align-items: center;
      padding: 16px;
      font-weight: 500;
      color: #666;
      font-size: 0.9rem;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .admin-header mat-icon, .user-header mat-icon {
      margin-right: 8px;
      font-size: 18px;
    }

    .menu-button {
      margin-right: 8px;
    }

    @media (max-width: 768px) {
      .sidenav {
        width: 100%;
      }
      
      .content-wrapper {
        padding: 16px;
      }
      
      .app-title {
        font-size: 1rem;
      }
    }
  `]
})
export class AppComponent implements OnInit {
  currentUser: User | null = null;
  isAdmin = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      this.isAdmin = user?.role === 'Admin';
    });
  }

  navigateToDashboard() {
    this.router.navigate(['/dashboard']);
  }

  navigateToProfile() {
    this.router.navigate(['/profile']);
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
} 