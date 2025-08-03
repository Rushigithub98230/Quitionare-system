import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';

export interface ResponseDetailData {
  response: any;
  questionnaire: any;
}

@Component({
  selector: 'app-response-detail-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatCardModule,
    MatIconModule,
    MatChipsModule,
    MatButtonModule,
    MatDividerModule,
    MatProgressBarModule,
    MatTooltipModule
  ],
  template: `
    <div class="response-detail-dialog">
      <!-- Dialog Header -->
      <div class="dialog-header">
        <div class="header-content">
          <div class="header-icon">
            <mat-icon>assignment</mat-icon>
          </div>
          <div class="header-text">
            <h2 mat-dialog-title>Response Details</h2>
            <p class="subtitle">{{ data.questionnaire?.title || 'Questionnaire Response' }}</p>
          </div>
        </div>
        <button mat-icon-button (click)="close()" class="close-btn">
          <mat-icon>close</mat-icon>
        </button>
      </div>

      <mat-dialog-content>
        <!-- Response Summary Section -->
        <div class="response-summary">
          <mat-card class="summary-card">
            <mat-card-content>
              <div class="summary-header">
                <h3>
                  <mat-icon>info</mat-icon>
                  Response Overview
                </h3>
                <div class="completion-status">
                  <mat-chip [class.completed]="data.response?.isCompleted" [class.draft]="!data.response?.isCompleted">
                    <mat-icon>{{ data.response?.isCompleted ? 'check_circle' : 'schedule' }}</mat-icon>
                    {{ data.response?.isCompleted ? 'Completed' : 'Draft' }}
                  </mat-chip>
                </div>
              </div>
              
              <div class="summary-grid">
                <div class="summary-item">
                  <div class="item-icon">
                    <mat-icon>quiz</mat-icon>
                  </div>
                  <div class="item-content">
                    <span class="label">Questionnaire</span>
                    <span class="value">{{ data.questionnaire?.title || 'Unknown' }}</span>
                  </div>
                </div>
                
                <div class="summary-item">
                  <div class="item-icon">
                    <mat-icon>category</mat-icon>
                  </div>
                  <div class="item-content">
                    <span class="label">Category</span>
                    <span class="value category-tag">{{ data.response?.categoryName || 'Unknown' }}</span>
                  </div>
                </div>
                
                <div class="summary-item">
                  <div class="item-icon">
                    <mat-icon>person</mat-icon>
                  </div>
                  <div class="item-content">
                    <span class="label">User</span>
                    <span class="value">{{ data.response?.userName || 'Admin' }}</span>
                  </div>
                </div>
                
                <div class="summary-item">
                  <div class="item-icon">
                    <mat-icon>schedule</mat-icon>
                  </div>
                  <div class="item-content">
                    <span class="label">Started</span>
                    <span class="value">{{ getFormattedDate(data.response?.startedAt) }}</span>
                  </div>
                </div>
                
                <div class="summary-item" *ngIf="data.response?.completedAt">
                  <div class="item-icon">
                    <mat-icon>check_circle</mat-icon>
                  </div>
                  <div class="item-content">
                    <span class="label">Completed</span>
                    <span class="value">{{ getFormattedDate(data.response?.completedAt) }}</span>
                  </div>
                </div>
                
                <div class="summary-item" *ngIf="data.response?.timeTaken">
                  <div class="item-icon">
                    <mat-icon>timer</mat-icon>
                  </div>
                  <div class="item-content">
                    <span class="label">Time Taken</span>
                    <span class="value">{{ data.response?.timeTaken }} minutes</span>
                  </div>
                </div>
              </div>

              <!-- Progress Section -->
              <div class="progress-section" *ngIf="data.response?.questionCount">
                <div class="progress-header">
                  <span class="progress-label">Completion Progress</span>
                  <span class="progress-percentage">{{ getCompletionPercentage() }}%</span>
                </div>
                <mat-progress-bar 
                  [value]="getCompletionPercentage()" 
                  [color]="data.response?.isCompleted ? 'primary' : 'accent'"
                  class="progress-bar">
                </mat-progress-bar>
                <div class="progress-stats">
                  <span class="stat">{{ data.response?.answeredQuestions || 0 }} of {{ data.response?.questionCount }} questions answered</span>
                </div>
              </div>
            </mat-card-content>
          </mat-card>
        </div>

        <!-- Questions Section -->
        <div class="questions-section">
          <div class="section-header">
            <h3>
              <mat-icon>question_answer</mat-icon>
              Question Responses
            </h3>
            <span class="question-count">{{ data.response?.questionResponses?.length || 0 }} questions</span>
          </div>
          
          <div class="questions-list">
            <mat-card *ngFor="let questionResponse of data.response?.questionResponses; let i = index" 
                     class="question-card"
                     [class.answered]="isQuestionAnswered(questionResponse)"
                     [class.required]="questionResponse?.question?.isRequired">
              <mat-card-header>
                <div class="question-header">
                  <div class="question-number">
                    <span class="number">{{ i + 1 }}</span>
                  </div>
                  <div class="question-info">
                    <h4 class="question-title">{{ questionResponse?.questionText || questionResponse?.question?.questionText }}</h4>
                    <div class="question-meta">
                      <span class="question-type">{{ questionResponse?.questionType || questionResponse?.question?.questionTypeName }}</span>
                      <mat-chip *ngIf="questionResponse?.question?.isRequired" class="required-chip">
                        Required
                      </mat-chip>
                    </div>
                  </div>
                  <div class="question-status">
                    <mat-icon *ngIf="isQuestionAnswered(questionResponse)" class="answered-icon">check_circle</mat-icon>
                    <mat-icon *ngIf="!isQuestionAnswered(questionResponse)" class="unanswered-icon">radio_button_unchecked</mat-icon>
                  </div>
                </div>
              </mat-card-header>
              
              <mat-card-content>
                <div class="question-content">
                  <div class="question-type">
                    {{ questionResponse?.questionType || questionResponse?.question?.questionTypeName }}
                  </div>
                  
                  <div class="response-content">
                    <div class="response-value" *ngIf="isQuestionAnswered(questionResponse)">
                      <span class="value-label">Response:</span>
                      <div class="text-response">
                        {{ getResponseDisplayValue(questionResponse) }}
                      </div>
                    </div>
                    
                    <div class="no-response" *ngIf="!isQuestionAnswered(questionResponse)">
                      <mat-icon>info</mat-icon>
                      <span>No response provided</span>
                    </div>
                    
                    <!-- Options for multiple choice questions -->
                    <div class="question-options" *ngIf="questionResponse?.question?.options?.length">
                      <div class="options-label">Available Options:</div>
                      <div class="options-grid">
                        <div *ngFor="let option of questionResponse?.question?.options" 
                             class="option-item"
                             [class.selected]="isOptionSelected(questionResponse, option)">
                          <mat-icon *ngIf="isOptionSelected(questionResponse, option)" class="option-selected">check_circle</mat-icon>
                          <mat-icon *ngIf="!isOptionSelected(questionResponse, option)" class="option-checkbox">radio_button_unchecked</mat-icon>
                          <span class="option-text">{{ option.optionText }}</span>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </mat-card-content>
            </mat-card>
          </div>
        </div>
      </mat-dialog-content>

      <!-- Dialog Actions -->
      <mat-dialog-actions align="end">
        <button mat-button (click)="close()">
          <mat-icon>close</mat-icon>
          Close
        </button>
        <button mat-raised-button color="primary" (click)="exportResponse()">
          <mat-icon>download</mat-icon>
          Export Response
        </button>
      </mat-dialog-actions>
    </div>
  `,
  styleUrls: ['./response-detail-dialog.component.scss']
})
export class ResponseDetailDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<ResponseDetailDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ResponseDetailData
  ) {}

  close(): void {
    this.dialogRef.close();
  }

  exportResponse(): void {
    // Implementation for exporting response
    console.log('Exporting response:', this.data.response);
  }

  getFormattedDate(date: string | Date): string {
    if (!date) return 'Not specified';
    return new Date(date).toLocaleString();
  }

  getCompletionPercentage(): number {
    if (!this.data.response?.questionCount) return 0;
    const answered = this.data.response?.answeredQuestions || 0;
    return Math.round((answered / this.data.response.questionCount) * 100);
  }

  isQuestionAnswered(questionResponse: any): boolean {
    if (!questionResponse) return false;
    
    // Check for text response
    if (questionResponse.textResponse && questionResponse.textResponse.trim()) return true;
    
    // Check for number response
    if (questionResponse.numberResponse !== null && questionResponse.numberResponse !== undefined) return true;
    
    // Check for date response
    if (questionResponse.dateResponse) return true;
    
    // Check for datetime response
    if (questionResponse.datetimeResponse) return true;
    
    // Check for boolean response (Yes/No)
    if (questionResponse.booleanResponse !== null && questionResponse.booleanResponse !== undefined) return true;
    
    // Check for JSON response
    if (questionResponse.jsonResponse && questionResponse.jsonResponse.trim()) return true;
    
    // Check for selected options - backend format
    if (questionResponse.optionResponses && questionResponse.optionResponses.length > 0) return true;
    
    // Check for selected options - legacy frontend format
    if (questionResponse.selectedOptionIds && questionResponse.selectedOptionIds.length > 0) return true;
    
    // Check for file upload
    if (questionResponse.filePath || questionResponse.fileName) return true;
    
    // Check for image upload
    if (questionResponse.imageUrl) return true;
    
    return false;
  }

  getResponseDisplayValue(questionResponse: any): string {
    if (!questionResponse) return 'No response';
    
    // Text response
    if (questionResponse.textResponse && questionResponse.textResponse.trim()) {
      return questionResponse.textResponse;
    }
    
    // Number response
    if (questionResponse.numberResponse !== null && questionResponse.numberResponse !== undefined) {
      // Check if it's a Yes/No question (values 1/0)
      if (questionResponse.numberResponse === 1) return 'Yes';
      if (questionResponse.numberResponse === 0) return 'No';
      return questionResponse.numberResponse.toString();
    }
    
    // Date response
    if (questionResponse.dateResponse) {
      return new Date(questionResponse.dateResponse).toLocaleDateString();
    }
    
    // Datetime response
    if (questionResponse.datetimeResponse) {
      return new Date(questionResponse.datetimeResponse).toLocaleString();
    }
    
    // Boolean response (Yes/No)
    if (questionResponse.booleanResponse !== null && questionResponse.booleanResponse !== undefined) {
      return questionResponse.booleanResponse ? 'Yes' : 'No';
    }
    
    // JSON response (for complex responses)
    if (questionResponse.jsonResponse) {
      try {
        const jsonData = JSON.parse(questionResponse.jsonResponse);
        if (jsonData.value) return jsonData.value;
        if (jsonData.text) return jsonData.text;
        if (jsonData.selectedOptions) return jsonData.selectedOptions.join(', ');
        return 'Complex response data';
      } catch {
        return questionResponse.jsonResponse;
      }
    }
    
    // Selected options - check for OptionResponses (backend format)
    if (questionResponse.optionResponses && questionResponse.optionResponses.length > 0) {
      const optionTexts = questionResponse.optionResponses.map((opt: any) => opt.optionText);
      return optionTexts.join(', ');
    }
    
    // Legacy format - check for selectedOptionIds (frontend format)
    if (questionResponse.selectedOptionIds && questionResponse.selectedOptionIds.length > 0) {
      const selectedOptions = questionResponse.question?.options?.filter((opt: any) => 
        questionResponse.selectedOptionIds.includes(opt.id)
      );
      return selectedOptions?.map((opt: any) => opt.optionText).join(', ') || 'Selected options';
    }
    
    // File upload
    if (questionResponse.filePath || questionResponse.fileName) {
      return questionResponse.fileName || 'File uploaded';
    }
    
    // Image upload
    if (questionResponse.imageUrl) {
      return 'Image uploaded';
    }
    
    return 'No response provided';
  }

  isOptionSelected(questionResponse: any, option: any): boolean {
    // Check backend format (OptionResponses)
    if (questionResponse?.optionResponses) {
      return questionResponse.optionResponses.some((opt: any) => opt.optionId === option.id);
    }
    
    // Check legacy frontend format (selectedOptionIds)
    return questionResponse?.selectedOptionIds?.includes(option.id) || false;
  }
} 