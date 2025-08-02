import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ResponseSummary {
  id: string;
  userId: string;
  questionnaireId: string;
  questionnaireTitle: string;
  categoryName: string;
  startedAt: string;
  completedAt?: string;
  isCompleted: boolean;
  isDraft: boolean;
  timeTaken?: number;
  createdAt: string;
}

export interface ResponseDetail extends ResponseSummary {
  questionResponses: QuestionResponseDetail[];
}

export interface QuestionResponseDetail {
  id: string;
  questionId: string;
  questionText: string;
  questionType: string;
  textResponse?: string;
  numberResponse?: number;
  dateResponse?: string;
  datetimeResponse?: string;
  booleanResponse?: boolean;
  jsonResponse?: string;
  filePath?: string;
  fileName?: string;
  fileSize?: number;
  fileType?: string;
  optionResponses: QuestionOptionResponseDetail[];
}

export interface QuestionOptionResponseDetail {
  id: string;
  optionId: string;
  optionText: string;
  optionValue: string;
  customText?: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  statusCode?: number;
}

@Injectable({
  providedIn: 'root'
})
export class ResponseService {
  private apiUrl = `${environment.apiUrl}/responses`;

  constructor(private http: HttpClient) {}

  // Get all responses for a questionnaire
  getResponsesByQuestionnaire(questionnaireId: string): Observable<ApiResponse<ResponseSummary[]>> {
    return this.http.get<ApiResponse<ResponseSummary[]>>(`${this.apiUrl}/questionnaire/${questionnaireId}`);
  }

  // Get all responses (admin only)
  getAllResponses(): Observable<ApiResponse<ResponseSummary[]>> {
    return this.http.get<ApiResponse<ResponseSummary[]>>(`${this.apiUrl}/all`);
  }

  // Get detailed response by ID
  getResponseById(responseId: string): Observable<ApiResponse<ResponseDetail>> {
    return this.http.get<ApiResponse<ResponseDetail>>(`${this.apiUrl}/${responseId}`);
  }

  // Get responses for a specific user
  getResponsesByUser(userId: string): Observable<ApiResponse<ResponseSummary[]>> {
    return this.http.get<ApiResponse<ResponseSummary[]>>(`${this.apiUrl}/user/${userId}`);
  }

  // Export responses to CSV
  exportResponsesToCSV(questionnaireId: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/export/${questionnaireId}`, {
      responseType: 'blob'
    });
  }

  // Export responses to CSV with proper handling
  exportResponsesToCSVWithData(questionnaireId: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/export/${questionnaireId}`);
  }

  // Submit a new response
  submitResponse(responseData: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}`, responseData);
  }

  // Validate responses before submission
  validateResponses(responseData: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/validate`, responseData);
  }
} 