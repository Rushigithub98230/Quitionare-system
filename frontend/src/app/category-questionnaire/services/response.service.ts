import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ApiResponse } from '../models/api-response.model';

export interface ResponseSummary {
  id: string;
  userId: number;
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

@Injectable({
  providedIn: 'root'
})
export class ResponseService {
  constructor(private apiService: ApiService) {}

  // Get all responses for a questionnaire
  getResponsesByQuestionnaire(questionnaireId: string): Observable<ApiResponse<ResponseSummary[]>> {
    return this.apiService.get<ResponseSummary[]>(`/responses/questionnaire/${questionnaireId}`);
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
    let params: any = {};
    
    if (filters?.questionnaireId) {
      params.questionnaireId = filters.questionnaireId;
    }
    if (filters?.categoryId) {
      params.categoryId = filters.categoryId;
    }
    if (filters?.dateFilter) {
      params.dateFilter = filters.dateFilter;
    }
    if (filters?.completionStatus) {
      params.completionStatus = filters.completionStatus;
    }
    if (filters?.userId) {
      params.userId = filters.userId;
    }
    if (filters?.page) {
      params.page = filters.page;
    }
    if (filters?.pageSize) {
      params.pageSize = filters.pageSize;
    }

    return this.apiService.get<ResponseSummary[]>('/responses/all', params);
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
    let params: any = {};
    
    if (filters?.questionnaireId) {
      params.questionnaireId = filters.questionnaireId;
    }
    if (filters?.categoryId) {
      params.categoryId = filters.categoryId;
    }
    if (filters?.dateFilter) {
      params.dateFilter = filters.dateFilter;
    }

    return this.apiService.get<{
      totalResponses: number;
      completedResponses: number;
      draftResponses: number;
      averageCompletionPercentage: number;
    }>('/responses/statistics', params);
  }

  // Get detailed response by ID
  getResponseById(responseId: string): Observable<ApiResponse<ResponseDetail>> {
    return this.apiService.get<ResponseDetail>(`/responses/${responseId}`);
  }

  // Get responses for a specific user
  getResponsesByUser(userId: number): Observable<ApiResponse<ResponseSummary[]>> {
    return this.apiService.get<ResponseSummary[]>(`/responses/user/${userId}`);
  }

  // Export responses to CSV
  exportResponsesToCSV(questionnaireId: string): Observable<Blob> {
    // Note: This method returns Blob directly, not wrapped in ApiResponse
    // The component will need to handle this differently
    return this.apiService.getBlob(`/responses/export/${questionnaireId}`);
  }

  // Export responses to CSV with proper handling
  exportResponsesToCSVWithData(questionnaireId: string): Observable<ApiResponse<any>> {
    return this.apiService.get<any>(`/responses/export/${questionnaireId}`);
  }

  // Submit a new response
  submitResponse(responseData: any): Observable<ApiResponse<any>> {
    return this.apiService.post<any>('/responses', responseData);
  }

  // Validate responses before submission
  validateResponses(responseData: any): Observable<ApiResponse<any>> {
    return this.apiService.post<any>('/responses/validate', responseData);
  }
} 