import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet, 
    RouterLink, 
    MatToolbarModule, 
    MatButtonModule,
    MatMenuModule,
    MatIconModule,
    MatDividerModule
  ],
  template: `
    <mat-toolbar color="primary">
      <span>Questionnaire System</span>
      <span class="spacer"></span>
      
      <ng-container *ngIf="currentUser$ | async as user; else authButtons">
        <button mat-button routerLink="/categories">Categories</button>
        <button mat-button routerLink="/questionnaires">Questionnaires</button>
        <button mat-button routerLink="/admin" *ngIf="user.role === 'Admin'">Admin</button>
        
        <button mat-icon-button [matMenuTriggerFor]="userMenu">
          <mat-icon>account_circle</mat-icon>
        </button>
        <mat-menu #userMenu="matMenu">
          <div mat-menu-item>
            <span>{{ user.firstName }} {{ user.lastName }}</span>
          </div>
          <div mat-menu-item>
            <span>{{ user.email }}</span>
          </div>
          <mat-divider></mat-divider>
          <button mat-menu-item (click)="logout()">
            <mat-icon>logout</mat-icon>
            <span>Logout</span>
          </button>
        </mat-menu>
      </ng-container>
      
      <ng-template #authButtons>
        <button mat-button routerLink="/login">Login</button>
        <button mat-button routerLink="/register">Register</button>
      </ng-template>
    </mat-toolbar>
    
    <main>
      <router-outlet></router-outlet>
    </main>
  `,
  styles: [`
    .spacer {
      flex: 1 1 auto;
    }
    
    main {
      padding: 20px;
      max-width: 1200px;
      margin: 0 auto;
    }
    
    mat-toolbar {
      position: sticky;
      top: 0;
      z-index: 1000;
    }
  `]
})
export class AppComponent {
  currentUser$ = this.authService.currentUser$;

  constructor(private authService: AuthService) {}

  logout(): void {
    this.authService.logout();
  }
} 