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
  templateUrl: './response-detail-dialog.component.html',
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