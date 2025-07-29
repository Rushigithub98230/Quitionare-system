import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-questionnaire-detail',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatProgressSpinnerModule],
  template: `
    <div class="questionnaire-detail-container">
      <h1>Questionnaire Detail</h1>
      <div class="loading-container">
        <mat-spinner></mat-spinner>
        <p>Loading questionnaire...</p>
      </div>
    </div>
  `,
  styles: [`
    .questionnaire-detail-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 20px;
    }
    
    .loading-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      min-height: 200px;
    }
  `]
})
export class QuestionnaireDetailComponent {
  // TODO: Implement questionnaire detail functionality
} 