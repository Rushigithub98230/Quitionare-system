import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';

interface PreviewResponse {
  questionId: string;
  questionText: string;
  questionType: string;
  response: any;
  isRequired: boolean;
  isValid: boolean;
  validationMessage?: string;
}

@Component({
  selector: 'app-preview-results',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatChipsModule,
    MatDividerModule,
    MatExpansionModule,
    MatListModule
  ],
  template: `
    <div class="results-container">
      <div class="results-header">
        <h2 mat-dialog-title>
          <mat-icon>assessment</mat-icon>
          Preview Results: {{ questionnaire?.title }}
        </h2>
        <button mat-icon-button (click)="closeDialog()" class="close-button">
          <mat-icon>close</mat-icon>
        </button>
      </div>

      <mat-dialog-content class="results-content">
        <div class="summary-section">
          <mat-card>
            <mat-card-content>
              <div class="summary-grid">
                <div class="summary-item">
                  <span class="summary-label">Total Questions:</span>
                  <span class="summary-value">{{ responses.length }}</span>
                </div>
                <div class="summary-item">
                  <span class="summary-label">Answered:</span>
                  <span class="summary-value">{{ getAnsweredCount() }}</span>
                </div>
                <div class="summary-item">
                  <span class="summary-label">Required:</span>
                  <span class="summary-value">{{ getRequiredCount() }}</span>
                </div>
                <div class="summary-item">
                  <span class="summary-label">Valid Responses:</span>
                  <span class="summary-value">{{ getValidCount() }}</span>
                </div>
              </div>
            </mat-card-content>
          </mat-card>
        </div>

        <div class="responses-section">
          <h3>Question Responses</h3>
          
          <mat-accordion>
            <mat-expansion-panel *ngFor="let response of responses; let i = index" class="response-panel">
              <mat-expansion-panel-header>
                <mat-panel-title>
                  <div class="response-header">
                    <span class="question-number">{{ i + 1 }}</span>
                    <span class="question-text">{{ response.questionText }}</span>
                    <mat-chip-set>
                      <mat-chip [color]="response.isRequired ? 'warn' : 'primary'" selected>
                        {{ response.isRequired ? 'Required' : 'Optional' }}
                      </mat-chip>
                      <mat-chip color="accent" selected>
                        {{ getQuestionTypeName(response.questionType) }}
                      </mat-chip>
                      <mat-chip [color]="response.isValid ? 'primary' : 'warn'" selected>
                        {{ response.isValid ? 'Valid' : 'Invalid' }}
                      </mat-chip>
                    </mat-chip-set>
                  </div>
                </mat-panel-title>
              </mat-expansion-panel-header>

              <div class="response-details">
                <div class="response-content">
                  <div class="response-item">
                    <strong>Question Type:</strong>
                    <span>{{ getQuestionTypeName(response.questionType) }}</span>
                  </div>
                  
                  <div class="response-item">
                    <strong>Response:</strong>
                    <div class="response-value">
                      <span *ngIf="response.response !== null && response.response !== undefined && response.response !== ''">
                        {{ formatResponse(response.response, response.questionType) }}
                      </span>
                      <span *ngIf="!response.response || response.response === ''" class="no-response">
                        <em>No response provided</em>
                      </span>
                    </div>
                  </div>

                  <div class="response-item" *ngIf="response.validationMessage">
                    <strong>Validation:</strong>
                    <span class="validation-error">{{ response.validationMessage }}</span>
                  </div>

                  <div class="response-item">
                    <strong>Status:</strong>
                    <mat-chip [color]="response.isValid ? 'primary' : 'warn'" selected>
                      {{ response.isValid ? 'Valid' : 'Invalid' }}
                    </mat-chip>
                  </div>
                </div>
              </div>
            </mat-expansion-panel>
          </mat-accordion>
        </div>

        <div class="export-section" *ngIf="responses.length > 0">
          <mat-divider></mat-divider>
          <div class="export-actions">
            <button mat-raised-button color="primary" (click)="exportToCSV()">
              <mat-icon>download</mat-icon>
              Export to CSV
            </button>
            <button mat-raised-button color="accent" (click)="exportToJSON()">
              <mat-icon>code</mat-icon>
              Export to JSON
            </button>
          </div>
        </div>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button (click)="closeDialog()">Close</button>
      </mat-dialog-actions>
    </div>
  `,
  styles: [`
    .results-container {
      max-width: 900px;
      max-height: 90vh;
      overflow: hidden;
      display: flex;
      flex-direction: column;
    }

    .results-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 16px 24px;
      background: #f5f5f5;
      border-bottom: 1px solid #e0e0e0;
    }

    .results-header h2 {
      display: flex;
      align-items: center;
      gap: 8px;
      margin: 0;
    }

    .close-button {
      position: absolute;
      top: 8px;
      right: 8px;
    }

    .results-content {
      flex: 1;
      overflow-y: auto;
      padding: 24px;
    }

    .summary-section {
      margin-bottom: 24px;
    }

    .summary-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 16px;
    }

    .summary-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 12px;
      background: #f8f9fa;
      border-radius: 4px;
    }

    .summary-label {
      font-weight: 500;
      color: #666;
    }

    .summary-value {
      font-weight: bold;
      color: #3f51b5;
    }

    .responses-section h3 {
      margin: 0 0 16px 0;
      color: #333;
    }

    .response-panel {
      margin-bottom: 8px;
    }

    .response-header {
      display: flex;
      align-items: center;
      gap: 12px;
      flex: 1;
    }

    .question-number {
      background: #3f51b5;
      color: white;
      border-radius: 50%;
      width: 24px;
      height: 24px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 12px;
      font-weight: bold;
    }

    .question-text {
      flex: 1;
      font-weight: 500;
      max-width: 400px;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    .response-details {
      padding: 16px 0;
    }

    .response-content {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }

    .response-item {
      display: flex;
      align-items: flex-start;
      gap: 12px;
      padding: 8px 0;
    }

    .response-item strong {
      min-width: 120px;
      color: #666;
    }

    .response-value {
      flex: 1;
      word-break: break-word;
    }

    .no-response {
      color: #999;
      font-style: italic;
    }

    .validation-error {
      color: #f44336;
      font-size: 12px;
    }

    .export-section {
      margin-top: 24px;
      padding-top: 16px;
    }

    .export-actions {
      display: flex;
      gap: 12px;
      justify-content: center;
      margin-top: 16px;
    }

    mat-dialog-actions {
      padding: 16px 24px;
      background: #f5f5f5;
      border-top: 1px solid #e0e0e0;
    }

    .mat-mdc-dialog-content {
      max-height: 70vh;
    }
  `]
})
export class PreviewResultsComponent {
  questionnaire?: any;
  responses: PreviewResponse[] = [];

  constructor(
    private dialogRef: MatDialogRef<PreviewResultsComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any
  ) {
    this.questionnaire = data.questionnaire;
    this.responses = data.responses || [];
  }

  getAnsweredCount(): number {
    return this.responses.filter(r => r.response !== null && r.response !== undefined && r.response !== '').length;
  }

  getRequiredCount(): number {
    return this.responses.filter(r => r.isRequired).length;
  }

  getValidCount(): number {
    return this.responses.filter(r => r.isValid).length;
  }

  getQuestionTypeName(typeName: string): string {
    // Since we're now passing the actual question type name, we can return it directly
    // or apply some formatting if needed
    if (!typeName || typeName === 'Unknown') {
      return 'Unknown';
    }
    
    // Convert to title case for better display
    return typeName.charAt(0).toUpperCase() + typeName.slice(1);
  }

  formatResponse(response: any, questionType: string): string {
    if (response === null || response === undefined) {
      return 'No response';
    }

    // Handle enhanced response format with option text
    if (response && typeof response === 'object' && response.displayText) {
      return response.displayText;
    }

    switch (questionType) {
      case 'checkbox':
      case 'multiselect':
        if (Array.isArray(response)) {
          return response.join(', ');
        }
        return response.toString();
      
      case 'radio':
      case 'select':
        if (response && typeof response === 'object' && response.text) {
          return response.text;
        }
        return response.toString();
      
      case 'date':
        if (response instanceof Date) {
          return response.toLocaleDateString();
        }
        return response.toString();
      
      case 'file':
        if (response instanceof File) {
          return `File: ${response.name} (${(response.size / 1024).toFixed(2)} KB)`;
        }
        if (response && typeof response === 'object' && response.fileName) {
          return `File: ${response.fileName} (${(response.fileSize / 1024).toFixed(2)} KB)`;
        }
        return response.toString();
      
      case 'rating':
        return `Rating: ${response} ${response === 1 ? 'star' : 'stars'}`;
      
      case 'slider':
        return `Slider: ${response}`;
      
      case 'yes_no':
        return response === 'yes' ? 'Yes' : 'No';
      
      default:
        return response.toString();
    }
  }

  exportToCSV(): void {
    const csvContent = this.generateCSV();
    this.downloadFile(csvContent, 'preview-results.csv', 'text/csv');
  }

  exportToJSON(): void {
    const jsonContent = JSON.stringify({
      questionnaire: this.questionnaire,
      responses: this.responses,
      summary: {
        totalQuestions: this.responses.length,
        answered: this.getAnsweredCount(),
        required: this.getRequiredCount(),
        valid: this.getValidCount()
      }
    }, null, 2);
    
    this.downloadFile(jsonContent, 'preview-results.json', 'application/json');
  }

  private generateCSV(): string {
    const headers = ['Question Number', 'Question Text', 'Question Type', 'Required', 'Response', 'Valid'];
    const rows = this.responses.map((response, index) => [
      index + 1,
      `"${response.questionText}"`,
      this.getQuestionTypeName(response.questionType),
      response.isRequired ? 'Yes' : 'No',
      `"${this.formatResponse(response.response, response.questionType)}"`,
      response.isValid ? 'Yes' : 'No'
    ]);

    return [headers, ...rows]
      .map(row => row.join(','))
      .join('\n');
  }

  private downloadFile(content: string, filename: string, contentType: string): void {
    const blob = new Blob([content], { type: contentType });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    link.click();
    window.URL.revokeObjectURL(url);
  }

  closeDialog(): void {
    this.dialogRef.close();
  }
} 