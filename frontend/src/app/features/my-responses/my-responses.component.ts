import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialogModule } from '@angular/material/dialog';
import { RouterModule } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { ResponseService } from 'src/app/services/response.service';


interface UserResponse {
  id: string;
  questionnaireId: string;
  questionnaireTitle: string;
  startedAt: string;
  completedAt?: string;
  isCompleted: boolean;
  isDraft: boolean;
  timeTaken?: number;
  createdAt: string;
}

@Component({
  selector: 'app-my-responses',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatChipsModule,
    MatDialogModule,
    RouterModule
  ],
  template: `
    <div class="responses-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>My Responses</mat-card-title>
          <mat-card-subtitle>View your questionnaire responses</mat-card-subtitle>
        </mat-card-header>
        <mat-card-content>
          <div class="responses-grid">
            <mat-card *ngFor="let response of responses" class="response-card">
              <mat-card-header>
                <mat-card-title>{{ response.questionnaireTitle }}</mat-card-title>
                <mat-card-subtitle>
                  Started: {{ response.startedAt | date:'short' }}
                </mat-card-subtitle>
              </mat-card-header>
              <mat-card-content>
                <div class="response-status">
                  <mat-chip [color]="response.isCompleted ? 'accent' : 'warn'" selected>
                    {{ response.isCompleted ? 'Completed' : 'In Progress' }}
                  </mat-chip>
                  <mat-chip *ngIf="response.isDraft" color="primary" selected>
                    Draft
                  </mat-chip>
                </div>
                
                <div class="response-details">
                  <p *ngIf="response.completedAt">
                    <strong>Completed:</strong> {{ response.completedAt | date:'short' }}
                  </p>
                  <p *ngIf="response.timeTaken">
                    <strong>Time Taken:</strong> {{ response.timeTaken }} minutes
                  </p>
                  <p>
                    <strong>Created:</strong> {{ response.createdAt | date:'short' }}
                  </p>
                </div>
              </mat-card-content>
              <mat-card-actions>
                <button mat-button color="primary" (click)="viewResponse(response.id)">
                  <mat-icon>visibility</mat-icon>
                  View Details
                </button>
                <button mat-button color="warn" (click)="deleteResponse(response.id)">
                  <mat-icon>delete</mat-icon>
                  Delete
                </button>
              </mat-card-actions>
            </mat-card>
          </div>
          
          <div *ngIf="responses.length === 0" class="no-responses">
            <mat-icon>assignment</mat-icon>
            <h3>No responses yet</h3>
            <p>You haven't submitted any questionnaire responses yet.</p>
            <button mat-raised-button color="primary" routerLink="/questionnaires">
              <mat-icon>add</mat-icon>
              Take a Questionnaire
            </button>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .responses-container {
      padding: 20px;
    }
    .responses-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
      gap: 20px;
      margin-top: 20px;
    }
    .response-card {
      transition: transform 0.2s;
    }
    .response-card:hover {
      transform: translateY(-2px);
    }
    .response-status {
      margin: 10px 0;
    }
    .response-details {
      margin: 15px 0;
    }
    .response-details p {
      margin: 5px 0;
    }
    .no-responses {
      text-align: center;
      padding: 40px;
      color: #666;
    }
    .no-responses mat-icon {
      font-size: 48px;
      width: 48px;
      height: 48px;
      color: #ccc;
    }
    .no-responses h3 {
      margin: 20px 0 10px 0;
      color: #333;
    }
    .no-responses p {
      margin-bottom: 20px;
    }
  `]
})
export class MyResponsesComponent implements OnInit {
  responses: UserResponse[] = [];

  constructor(
    private responseService: ResponseService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadResponses();
  }

  loadResponses(): void {
    const currentUser = this.authService.getCurrentUser();
    if (currentUser) {
      this.responseService.getUserResponses(currentUser.id).subscribe({
        next: (response: any) => {
          this.responses = response.data || [];
        },
        error: (error: any) => {
          console.error('Error loading responses:', error);
        }
      });
    }
  }

  viewResponse(responseId: string): void {
    console.log('View response:', responseId);
    // TODO: Implement response detail view
  }

  deleteResponse(responseId: string): void {
    if (confirm('Are you sure you want to delete this response?')) {
      this.responseService.deleteResponse(responseId).subscribe({
        next: () => {
          this.loadResponses(); // Reload the list
        },
        error: (error: any) => {
          console.error('Error deleting response:', error);
        }
      });
    }
  }
} 