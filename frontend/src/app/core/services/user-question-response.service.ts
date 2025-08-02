import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { QuestionType } from '../models/question.model';

export interface UserQuestionResponse {
  id?: string;
  questionnaireId: string;
  questionId: string;
  response: any;
  submittedAt?: Date;
}

export interface SubmitResponseDto {
  questionnaireId: string;
  responses: CreateQuestionResponseDto[];
}

export interface CreateQuestionResponseDto {
  questionId: string;
  textResponse?: string;
  numberResponse?: number;
  dateResponse?: Date;
  selectedOptionIds?: string[];
  fileUrl?: string;
  imageUrl?: string;
}

export interface QuestionOption {
  id: string;
  questionId: string;
  optionText: string;
  optionValue: string;
  displayOrder: number;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class UserQuestionResponseService {
  private apiUrl = `${environment.apiUrl}/responses`;

  constructor(private http: HttpClient) {}

  // Improved response mapping for all question types
  saveResponses(responses: UserQuestionResponse[]): Observable<any> {
    if (responses.length === 0) {
      return new Observable(observer => {
        observer.error('No responses to save');
      });
    }

    // Transform responses to match backend DTO structure
    const questionnaireId = responses[0].questionnaireId;
    const submitDto: SubmitResponseDto = {
      questionnaireId: questionnaireId,
      responses: responses.map(response => {
        const createDto: CreateQuestionResponseDto = {
          questionId: response.questionId,
          textResponse: undefined,
          numberResponse: undefined,
          dateResponse: undefined,
          selectedOptionIds: [],
          fileUrl: undefined,
          imageUrl: undefined
        };

        // Improved response mapping based on type
        this.mapResponseToDto(response.response, createDto);

        return createDto;
      })
    };

    console.log('Submitting response data:', submitDto);
    console.log('Questionnaire ID type:', typeof questionnaireId, questionnaireId);
    console.log('First question ID type:', typeof responses[0].questionId, responses[0].questionId);
    return this.http.post(this.apiUrl, submitDto);
  }

  // Enhanced response mapping for all question types
  private mapResponseToDto(response: any, createDto: CreateQuestionResponseDto): void {
    // Handle null/undefined responses
    if (response === null || response === undefined) {
      return; // Leave all fields undefined for backend validation
    }

    // Handle different response types comprehensively
    
    // 1. STRING RESPONSES (text, textarea, email, phone)
    if (typeof response === 'string') {
      createDto.textResponse = response;
      return;
    }

    // 2. NUMBER RESPONSES (number, rating, slider)
    if (typeof response === 'number') {
      createDto.numberResponse = response;
      return;
    }

    // 3. DATE RESPONSES (date)
    if (response instanceof Date) {
      createDto.dateResponse = response;
      return;
    }

    // 4. ARRAY RESPONSES (checkbox, multiselect)
    if (Array.isArray(response)) {
      // For checkbox/multiselect, store as selectedOptionIds
      createDto.selectedOptionIds = response;
      console.log('Array response stored as selectedOptionIds:', response);
      return;
    }

    // 5. FILE RESPONSES (file uploads) - Updated to handle fileUrl
    if (response instanceof File) {
      // For now, store file name as text response
      // TODO: Implement proper file upload handling
      createDto.textResponse = `File: ${response.name} (${(response.size / 1024).toFixed(2)} KB)`;
      console.log('File response stored as text:', response.name);
      return;
    }

    // 6. OBJECT RESPONSES (complex objects) - Updated for file handling
    if (response && typeof response === 'object') {
      // Handle file uploads with URL
      if (response.fileUrl) {
        createDto.fileUrl = response.fileUrl;
        // Also store file name as text for display
        if (response.fileName) {
          createDto.textResponse = `File: ${response.fileName}`;
        }
      }
      if (response.imageUrl) {
        createDto.imageUrl = response.imageUrl;
      }
      
      // Handle other object properties
      if (response.text) {
        createDto.textResponse = response.text;
      }
      if (response.number) {
        createDto.numberResponse = response.number;
      }
      if (response.date) {
        createDto.dateResponse = response.date;
      }
      if (response.selectedOptions && Array.isArray(response.selectedOptions)) {
        createDto.selectedOptionIds = response.selectedOptions;
      }
      
      return;
    }

    // 7. BOOLEAN RESPONSES (boolean, checkbox)
    if (typeof response === 'boolean') {
      createDto.textResponse = response ? 'Yes' : 'No';
      return;
    }

    // 8. FALLBACK: Convert to string
    createDto.textResponse = String(response);
    console.log('Fallback response conversion:', response);
  }

  // Get responses for a specific questionnaire
  getResponsesByQuestionnaire(questionnaireId: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/questionnaire/${questionnaireId}`);
  }

  // Get responses for a specific user
  getResponsesByUser(userId: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/user/${userId}`);
  }

  // Export responses to CSV
  exportResponsesToCSV(questionnaireId: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/export/${questionnaireId}`, {
      responseType: 'blob'
    });
  }
} 