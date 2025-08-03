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

  // Get all responses (admin only) - now with filtering support
  getAllResponses(filters?: {
    questionnaireId?: string;
    categoryId?: string;
    dateFilter?: string;
    completionStatus?: string;
    userId?: string;
    page?: number;
    pageSize?: number;
  }): Observable<ApiResponse<ResponseSummary[]>> {
    let params = new URLSearchParams();
    
    if (filters?.questionnaireId) {
      params.append('questionnaireId', filters.questionnaireId);
    }
    if (filters?.categoryId) {
      params.append('categoryId', filters.categoryId);
    }
    if (filters?.dateFilter) {
      params.append('dateFilter', filters.dateFilter);
    }
    if (filters?.completionStatus) {
      params.append('completionStatus', filters.completionStatus);
    }
    if (filters?.userId) {
      params.append('userId', filters.userId);
    }
    if (filters?.page) {
      params.append('page', filters.page.toString());
    }
    if (filters?.pageSize) {
      params.append('pageSize', filters.pageSize.toString());
    }

    const url = params.toString() ? `${this.apiUrl}/all?${params.toString()}` : `${this.apiUrl}/all`;
    return this.http.get<ApiResponse<ResponseSummary[]>>(url);
  }

  // Get filtered responses with backend filtering
  getFilteredResponses(filters: {
    questionnaireId?: string;
    categoryId?: string;
    dateFilter?: string;
    completionStatus?: string;
    userId?: string;
    page?: number;
    pageSize?: number;
  }): Observable<ApiResponse<ResponseSummary[]>> {
    // Use the enhanced getAllResponses method instead
    return this.getAllResponses(filters);
  }

  // Get response statistics using existing API structure
  getResponseStatistics(filters?: {
    questionnaireId?: string;
    categoryId?: string;
    dateFilter?: string;
  }): Observable<ApiResponse<{
    totalResponses: number;
    completedResponses: number;
    draftResponses: number;
    averageCompletionPercentage: number;
  }>> {
    let params = new URLSearchParams();
    
    if (filters?.questionnaireId) {
      params.append('questionnaireId', filters.questionnaireId);
    }
    if (filters?.categoryId) {
      params.append('categoryId', filters.categoryId);
    }
    if (filters?.dateFilter) {
      params.append('dateFilter', filters.dateFilter);
    }

    const url = params.toString() ? `${this.apiUrl}/statistics?${params.toString()}` : `${this.apiUrl}/statistics`;
    return this.http.get<ApiResponse<{
      totalResponses: number;
      completedResponses: number;
      draftResponses: number;
      averageCompletionPercentage: number;
    }>>(url);
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