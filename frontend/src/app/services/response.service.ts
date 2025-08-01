import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { SubmitResponseRequest } from '../models/questionnaire.model';

export interface UserQuestionResponse {
  id: string;
  userId: string;
  questionnaireId: string;
  startedAt: string;
  completedAt?: string;
  isCompleted: boolean;
  isDraft: boolean;
  submissionIp?: string;
  userAgent?: string;
  timeTaken?: number;
  createdAt: string;
  updatedAt: string;
}

export interface QuestionResponse {
  id: string;
  responseId: string;
  questionId: string;
  textResponse?: string;
  numberResponse?: number;
  dateResponse?: string;
  booleanResponse?: boolean;
  selectedOptionIds: string[];
  createdAt: string;
  updatedAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class ResponseService {
  constructor(private apiService: ApiService) {}

  submitResponse(request: SubmitResponseRequest): Observable<any> {
    return this.apiService.post('/responses/submit', request);
  }

  getUserResponses(userId?: string): Observable<any> {
    const endpoint = userId ? `/responses/user/${userId}` : '/responses/user';
    return this.apiService.get(endpoint);
  }

  getResponseById(responseId: string): Observable<any> {
    return this.apiService.get(`/responses/${responseId}`);
  }

  getQuestionnaireResponses(questionnaireId: string): Observable<any> {
    return this.apiService.get(`/responses/questionnaire/${questionnaireId}`);
  }

  deleteResponse(responseId: string): Observable<any> {
    return this.apiService.delete(`/responses/${responseId}`);
  }
} 