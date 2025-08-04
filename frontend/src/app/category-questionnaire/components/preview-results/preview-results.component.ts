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
  templateUrl: './preview-results.component.html',
  styleUrls: ['./preview-results.component.scss']
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