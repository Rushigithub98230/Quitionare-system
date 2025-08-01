import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { MatBadgeModule } from '@angular/material/badge';
import { MatGridListModule } from '@angular/material/grid-list';
import { AuthService } from '../../services/auth.service';
import { QuestionnaireService } from '../../services/questionnaire.service';
import { CategoryService } from '../../services/category.service';
import { User } from '../../models/user.model';
import { QuestionnaireTemplate } from '../../models/questionnaire.model';
import { Category } from '../../models/category.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressBarModule,
    MatListModule,
    MatMenuModule,
    MatDividerModule,
    MatBadgeModule,
    MatGridListModule
  ],
  template: `
    <div class="dashboard-container">
      <!-- Welcome Section -->
      <div class="welcome-section">
        <mat-card class="welcome-card">
          <mat-card-content>
            <div class="welcome-content">
              <div class="welcome-text">
                <h1>Welcome back, {{ currentUser?.firstName }}!</h1>
                <p class="subtitle">Here's what's happening with your questionnaires today.</p>
                <div class="user-info">
                  <mat-chip-set>
                    <mat-chip color="primary" selected>
                      <mat-icon>person</mat-icon>
                      {{ currentUser?.role }}
                    </mat-chip>
                    <mat-chip *ngIf="currentUser?.category" color="accent" selected>
                      <mat-icon>category</mat-icon>
                      {{ currentUser?.category }}
                    </mat-chip>
                  </mat-chip-set>
                </div>
              </div>
                             <div class="welcome-actions">
                 <button mat-raised-button color="primary" routerLink="/categories">
                   <mat-icon>category</mat-icon>
                   Browse Categories
                 </button>
                 <button mat-button routerLink="/questionnaires">
                   <mat-icon>quiz</mat-icon>
                   View All Questionnaires
                 </button>
                 <button mat-button routerLink="/my-responses">
                   <mat-icon>history</mat-icon>
                   View History
                 </button>
               </div>
            </div>
          </mat-card-content>
        </mat-card>
      </div>

      <!-- Statistics Cards -->
      <div class="stats-section">
        <mat-grid-list cols="4" rowHeight="120px" gutterSize="16px">
          <mat-grid-tile>
            <mat-card class="stat-card">
              <mat-card-content>
                <div class="stat-content">
                  <div class="stat-icon">
                    <mat-icon>quiz</mat-icon>
                  </div>
                  <div class="stat-info">
                    <h3>{{ stats.totalQuestionnaires }}</h3>
                    <p>Available Questionnaires</p>
                  </div>
                </div>
              </mat-card-content>
            </mat-card>
          </mat-grid-tile>

          <mat-grid-tile>
            <mat-card class="stat-card">
              <mat-card-content>
                <div class="stat-content">
                  <div class="stat-icon completed">
                    <mat-icon>check_circle</mat-icon>
                  </div>
                  <div class="stat-info">
                    <h3>{{ stats.completedResponses }}</h3>
                    <p>Completed Responses</p>
                  </div>
                </div>
              </mat-card-content>
            </mat-card>
          </mat-grid-tile>

          <mat-grid-tile>
            <mat-card class="stat-card">
              <mat-card-content>
                <div class="stat-content">
                  <div class="stat-icon pending">
                    <mat-icon>pending</mat-icon>
                  </div>
                  <div class="stat-info">
                    <h3>{{ stats.pendingResponses }}</h3>
                    <p>Pending Responses</p>
                  </div>
                </div>
              </mat-card-content>
            </mat-card>
          </mat-grid-tile>

          <mat-grid-tile>
            <mat-card class="stat-card">
              <mat-card-content>
                <div class="stat-content">
                  <div class="stat-icon categories">
                    <mat-icon>category</mat-icon>
                  </div>
                  <div class="stat-info">
                    <h3>{{ stats.totalCategories }}</h3>
                    <p>Categories</p>
                  </div>
                </div>
              </mat-card-content>
            </mat-card>
          </mat-grid-tile>
        </mat-grid-list>
      </div>

      <!-- Recent Activities & Quick Actions -->
      <div class="content-section">
        <div class="left-column">
          <!-- Recent Questionnaires -->
          <mat-card class="activity-card">
            <mat-card-header>
              <mat-card-title>
                <mat-icon>recent_actors</mat-icon>
                Recent Questionnaires
              </mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <mat-list>
                <mat-list-item *ngFor="let questionnaire of recentQuestionnaires" 
                              [routerLink]="['/questionnaire', questionnaire.id, 'stepper']"
                              class="questionnaire-item">
                  <mat-icon matListItemIcon>quiz</mat-icon>
                  <div matListItemTitle>{{ questionnaire.title }}</div>
                  <div matListItemLine>
                    <mat-chip size="small" color="primary">
                      {{ questionnaire.categoryName }}
                    </mat-chip>
                    <span class="question-count">{{ questionnaire.questionCount }} questions</span>
                  </div>
                  <button mat-icon-button [matMenuTriggerFor]="questionnaireMenu">
                    <mat-icon>more_vert</mat-icon>
                  </button>
                  <mat-menu #questionnaireMenu="matMenu">
                    <button mat-menu-item (click)="startQuestionnaire(questionnaire)">
                      <mat-icon>play_arrow</mat-icon>
                      Start
                    </button>
                    <button mat-menu-item (click)="viewQuestionnaire(questionnaire)">
                      <mat-icon>visibility</mat-icon>
                      Preview
                    </button>
                  </mat-menu>
                </mat-list-item>
              </mat-list>
            </mat-card-content>
          </mat-card>
        </div>

        <div class="right-column">
          <!-- Quick Actions -->
          <mat-card class="quick-actions-card">
            <mat-card-header>
              <mat-card-title>
                <mat-icon>flash_on</mat-icon>
                Quick Actions
              </mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <div class="quick-actions">
                <button mat-raised-button color="primary" class="action-button" routerLink="/categories">
                  <mat-icon>category</mat-icon>
                  Browse Categories
                </button>
                
                <button mat-stroked-button class="action-button" routerLink="/questionnaires">
                  <mat-icon>quiz</mat-icon>
                  View All Questionnaires
                </button>
                
                <button mat-stroked-button class="action-button" routerLink="/my-responses">
                  <mat-icon>history</mat-icon>
                  View My Responses
                </button>
                
                <button mat-stroked-button class="action-button" routerLink="/profile">
                  <mat-icon>person</mat-icon>
                  Update Profile
                </button>
              </div>
            </mat-card-content>
          </mat-card>

          <!-- Progress Summary -->
          <mat-card class="progress-card">
            <mat-card-header>
              <mat-card-title>
                <mat-icon>trending_up</mat-icon>
                Your Progress
              </mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <div class="progress-item">
                <div class="progress-label">
                  <span>Completion Rate</span>
                  <span>{{ progress.completionRate }}%</span>
                </div>
                <mat-progress-bar 
                  [value]="progress.completionRate" 
                  color="primary"
                  mode="determinate">
                </mat-progress-bar>
              </div>
              
              <div class="progress-item">
                <div class="progress-label">
                  <span>This Month</span>
                  <span>{{ progress.thisMonth }}</span>
                </div>
                <mat-progress-bar 
                  [value]="progress.thisMonthPercentage" 
                  color="accent"
                  mode="determinate">
                </mat-progress-bar>
              </div>
            </mat-card-content>
          </mat-card>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-container {
      max-width: 1400px;
      margin: 0 auto;
    }

    .welcome-section {
      margin-bottom: 24px;
    }

    .welcome-card {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }

    .welcome-content {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 24px 0;
    }

    .welcome-text h1 {
      margin: 0 0 8px 0;
      font-size: 2rem;
      font-weight: 300;
    }

    .subtitle {
      margin: 0 0 16px 0;
      opacity: 0.9;
      font-size: 1.1rem;
    }

    .user-info {
      margin-bottom: 16px;
    }

    .welcome-actions {
      display: flex;
      gap: 12px;
    }

    .stats-section {
      margin-bottom: 24px;
    }

    .stat-card {
      height: 100%;
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .stat-content {
      display: flex;
      align-items: center;
      height: 100%;
      padding: 16px;
    }

    .stat-icon {
      width: 48px;
      height: 48px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      margin-right: 16px;
      background: #e3f2fd;
      color: #1976d2;
    }

    .stat-icon.completed {
      background: #e8f5e8;
      color: #4caf50;
    }

    .stat-icon.pending {
      background: #fff3e0;
      color: #ff9800;
    }

    .stat-icon.categories {
      background: #f3e5f5;
      color: #9c27b0;
    }

    .stat-info h3 {
      margin: 0;
      font-size: 1.8rem;
      font-weight: 500;
    }

    .stat-info p {
      margin: 4px 0 0 0;
      color: #666;
      font-size: 0.9rem;
    }

    .content-section {
      display: grid;
      grid-template-columns: 2fr 1fr;
      gap: 24px;
    }

    .activity-card, .quick-actions-card, .progress-card {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .activity-card mat-card-header {
      padding-bottom: 16px;
    }

    .activity-card mat-card-title {
      display: flex;
      align-items: center;
      font-size: 1.2rem;
    }

    .activity-card mat-card-title mat-icon {
      margin-right: 8px;
    }

    .questionnaire-item {
      border-radius: 8px;
      margin-bottom: 8px;
      transition: background-color 0.2s;
    }

    .questionnaire-item:hover {
      background-color: #f5f5f5;
    }

    .question-count {
      margin-left: 8px;
      color: #666;
      font-size: 0.9rem;
    }

    .quick-actions {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }

    .action-button {
      justify-content: flex-start;
      padding: 12px 16px;
      height: auto;
    }

    .action-button mat-icon {
      margin-right: 8px;
    }

    .progress-card mat-card-header {
      padding-bottom: 16px;
    }

    .progress-card mat-card-title {
      display: flex;
      align-items: center;
      font-size: 1.2rem;
    }

    .progress-card mat-card-title mat-icon {
      margin-right: 8px;
    }

    .progress-item {
      margin-bottom: 20px;
    }

    .progress-label {
      display: flex;
      justify-content: space-between;
      margin-bottom: 8px;
      font-size: 0.9rem;
      color: #666;
    }

    @media (max-width: 1024px) {
      .content-section {
        grid-template-columns: 1fr;
      }
      
      .welcome-content {
        flex-direction: column;
        text-align: center;
      }
      
      .welcome-actions {
        margin-top: 16px;
      }
    }

    @media (max-width: 768px) {
      mat-grid-list {
        grid-template-columns: repeat(2, 1fr) !important;
      }
      
      .welcome-text h1 {
        font-size: 1.5rem;
      }
    }
  `]
})
export class DashboardComponent implements OnInit {
  currentUser: User | null = null;
  recentQuestionnaires: QuestionnaireTemplate[] = [];
  stats = {
    totalQuestionnaires: 0,
    completedResponses: 0,
    pendingResponses: 0,
    totalCategories: 0
  };
  progress = {
    completionRate: 75,
    thisMonth: 8,
    thisMonthPercentage: 80
  };

  constructor(
    private authService: AuthService,
    private questionnaireService: QuestionnaireService,
    private categoryService: CategoryService,
    private router: Router
  ) {}

  ngOnInit() {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });

    this.loadDashboardData();
  }

  loadDashboardData() {
    // Load recent questionnaires
    this.questionnaireService.getQuestionnaires().subscribe(response => {
      if (response.success) {
        this.recentQuestionnaires = response.data.slice(0, 5);
        this.stats.totalQuestionnaires = response.data.length;
      }
    });

    // Load categories
    this.categoryService.getCategories().subscribe(response => {
      if (response.success) {
        this.stats.totalCategories = response.data.length;
      }
    });

    // TODO: Load user-specific statistics
    this.loadUserStats();
  }

  loadUserStats() {
    // TODO: Implement user-specific statistics
    // This would include completed responses, pending responses, etc.
  }

  startQuestionnaire(questionnaire: QuestionnaireTemplate) {
    this.router.navigate(['/questionnaire', questionnaire.id, 'stepper']);
  }

  viewQuestionnaire(questionnaire: QuestionnaireTemplate) {
    this.router.navigate(['/questionnaire', questionnaire.id]);
  }
} 