import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar } from '@angular/material/snack-bar';
import { QuestionnaireService } from '../../services/questionnaire.service';
import { CategoryQuestionnaireTemplate } from '../../models/questionnaire.model';

@Component({
  selector: 'app-questionnaires',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule
  ],
  template: `
    <div class="questionnaires-container">
      <div class="header">
        <div class="header-content">
          <div class="breadcrumb">
            <button mat-button routerLink="/categories">
              <mat-icon>arrow_back</mat-icon>
              Back to Categories
            </button>
          </div>
          <h1>{{ categoryName || 'Questionnaire Templates' }}</h1>
          <p *ngIf="categoryName">Select a questionnaire template to respond to</p>
          <p *ngIf="!categoryName">All available questionnaire templates</p>
        </div>
      </div>

      <div class="questionnaires-grid" *ngIf="questionnaireTemplates.length > 0; else noQuestionnaires">
        <mat-card *ngFor="let template of questionnaireTemplates" class="questionnaire-card">
          <mat-card-header>
            <mat-card-title>{{ template.title }}</mat-card-title>
            <mat-card-subtitle>{{ template.description }}</mat-card-subtitle>
          </mat-card-header>
          
          <mat-card-content>
            <div class="questionnaire-info">
              <div class="info-row">
                <span class="label">Category:</span>
                <span class="value">{{ template.categoryName }}</span>
              </div>
              <div class="info-row">
                <span class="label">Questions:</span>
                <span class="value">{{ template.questionCount }}</span>
              </div>
              <div class="info-row">
                <span class="label">Status:</span>
                <mat-chip [color]="template.isActive ? 'primary' : 'warn'" selected>
                  {{ template.isActive ? 'Active' : 'Inactive' }}
                </mat-chip>
              </div>
              <div class="info-row">
                <span class="label">Mandatory:</span>
                <span class="value">{{ template.isMandatory ? 'Yes' : 'No' }}</span>
              </div>
              <div class="info-row">
                <span class="label">Version:</span>
                <span class="value">{{ template.version }}</span>
              </div>
            </div>
          </mat-card-content>
          
          <mat-card-actions>
            <button mat-raised-button color="primary" 
                    [disabled]="!template.isActive"
                    (click)="startQuestionnaire(template)">
              <mat-icon>play_arrow</mat-icon>
              {{ template.isActive ? 'Start Questionnaire' : 'Not Available' }}
            </button>
            <button mat-button color="accent" (click)="previewQuestionnaire(template)">
              <mat-icon>visibility</mat-icon>
              Preview
            </button>
          </mat-card-actions>
        </mat-card>
      </div>

      <ng-template #noQuestionnaires>
        <div class="empty-state">
          <mat-icon>quiz</mat-icon>
          <h2>No Questionnaire Templates Available</h2>
          <p *ngIf="categoryName">There are no questionnaire templates available in this category.</p>
          <p *ngIf="!categoryName">There are no questionnaire templates available for your account.</p>
        </div>
      </ng-template>
    </div>
  `,
  styles: [`
    .questionnaires-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 20px;
    }

    .header {
      margin-bottom: 40px;
    }

    .header-content {
      text-align: center;
    }

    .breadcrumb {
      margin-bottom: 20px;
      text-align: left;
    }

    .header h1 {
      color: #333;
      margin-bottom: 8px;
      font-size: 2.5rem;
      font-weight: 300;
    }

    .header p {
      color: #666;
      font-size: 1.1rem;
      margin: 0;
    }

    .questionnaires-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
      gap: 24px;
    }

    .questionnaire-card {
      transition: all 0.3s ease;
      border-radius: 12px;
      overflow: hidden;
    }

    .questionnaire-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
    }

    .questionnaire-info {
      margin: 16px 0;
    }

    .info-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin: 12px 0;
      padding: 8px 0;
      border-bottom: 1px solid #f0f0f0;
    }

    .info-row:last-child {
      border-bottom: none;
    }

    .label {
      font-weight: 500;
      color: #666;
    }

    .value {
      color: #333;
      font-weight: 600;
    }

    mat-card-actions {
      display: flex;
      justify-content: space-between;
      padding: 16px;
    }

    .empty-state {
      text-align: center;
      padding: 60px 20px;
      color: #666;
    }

    .empty-state mat-icon {
      font-size: 80px;
      width: 80px;
      height: 80px;
      color: #ccc;
      margin-bottom: 24px;
    }

    .empty-state h2 {
      margin: 0 0 16px 0;
      color: #333;
      font-weight: 300;
    }

    .empty-state p {
      margin: 0;
      font-size: 1.1rem;
      line-height: 1.6;
    }

    @media (max-width: 768px) {
      .questionnaires-grid {
        grid-template-columns: 1fr;
      }

      .header h1 {
        font-size: 2rem;
      }

      mat-card-actions {
        flex-direction: column;
        gap: 12px;
      }
    }
  `]
})
export class QuestionnairesComponent implements OnInit {
  questionnaireTemplates: CategoryQuestionnaireTemplate[] = [];
  categoryId?: string;
  categoryName?: string;
  isLoading = false;

  constructor(
    private questionnaireService: QuestionnaireService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.categoryId = params['categoryId'];
      this.categoryName = params['categoryName'];
      this.loadQuestionnaireTemplates();
    });
  }

  loadQuestionnaireTemplates(): void {
    this.isLoading = true;
    this.questionnaireService.getQuestionnaireTemplates().subscribe({
      next: (response) => {
        if (response.success) {
          // Filter by category if specified
          if (this.categoryId) {
            this.questionnaireTemplates = response.data.filter(q => 
              q.categoryId === this.categoryId && q.isActive
            );
          } else {
            this.questionnaireTemplates = response.data.filter(q => q.isActive);
          }
        } else {
          this.snackBar.open(response.message || 'Failed to load questionnaire templates', 'Close', { duration: 5000 });
        }
      },
      error: (error) => {
        this.snackBar.open('Error loading questionnaire templates', 'Close', { duration: 5000 });
        console.error('Error loading questionnaire templates:', error);
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  startQuestionnaire(template: CategoryQuestionnaireTemplate): void {
    if (template.isActive) {
      this.router.navigate(['/questionnaire', template.id, 'stepper']);
    } else {
      this.snackBar.open('This questionnaire template is currently inactive', 'Close', { duration: 3000 });
    }
  }

  previewQuestionnaire(template: CategoryQuestionnaireTemplate): void {
    // TODO: Implement preview functionality
    this.snackBar.open('Preview functionality will be implemented', 'Close', { duration: 3000 });
  }
} 