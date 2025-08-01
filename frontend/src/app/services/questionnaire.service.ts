import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService, ApiResponse } from './api.service';
import { 
  CategoryQuestionnaireTemplate, 
  CreateCategoryQuestionnaireTemplateRequest, 
  UpdateCategoryQuestionnaireTemplateRequest,
  CreateCategoryQuestionRequest,
  UpdateCategoryQuestionRequest,
  CategoryQuestion,
  SubmitResponseRequest
} from '../models/questionnaire.model';

@Injectable({
  providedIn: 'root'
})
export class QuestionnaireService {
  constructor(private apiService: ApiService) {}

  // Template Management
  getQuestionnaires(): Observable<ApiResponse<CategoryQuestionnaireTemplate[]>> {
    return this.apiService.get<CategoryQuestionnaireTemplate[]>('/CategoryQuestionnaireTemplates');
  }

  getQuestionnaireById(id: string): Observable<ApiResponse<CategoryQuestionnaireTemplate>> {
    return this.apiService.get<CategoryQuestionnaireTemplate>(`/CategoryQuestionnaireTemplates/${id}/response`);
  }

  getQuestionnaireTemplates(): Observable<ApiResponse<CategoryQuestionnaireTemplate[]>> {
    return this.apiService.get<CategoryQuestionnaireTemplate[]>('/CategoryQuestionnaireTemplates');
  }

  getQuestionnaireTemplate(id: string): Observable<ApiResponse<CategoryQuestionnaireTemplate>> {
    return this.apiService.get<CategoryQuestionnaireTemplate>(`/CategoryQuestionnaireTemplates/${id}`);
  }

  getQuestionnaireTemplateByCategory(categoryId: string): Observable<ApiResponse<CategoryQuestionnaireTemplate>> {
    return this.apiService.get<CategoryQuestionnaireTemplate>(`/CategoryQuestionnaireTemplates/category/${categoryId}`);
  }

  createQuestionnaireTemplate(template: CreateCategoryQuestionnaireTemplateRequest): Observable<ApiResponse<CategoryQuestionnaireTemplate>> {
    return this.apiService.post<CategoryQuestionnaireTemplate>('/CategoryQuestionnaireTemplates', template);
  }

  updateQuestionnaireTemplate(id: string, template: UpdateCategoryQuestionnaireTemplateRequest): Observable<ApiResponse<CategoryQuestionnaireTemplate>> {
    return this.apiService.put<CategoryQuestionnaireTemplate>(`/CategoryQuestionnaireTemplates/${id}`, template);
  }

  deleteQuestionnaireTemplate(id: string): Observable<ApiResponse<any>> {
    return this.apiService.delete(`/CategoryQuestionnaireTemplates/${id}`);
  }

  // Question Management
  addQuestion(questionnaireId: string, question: CreateCategoryQuestionRequest): Observable<ApiResponse<CategoryQuestion>> {
    return this.apiService.post<CategoryQuestion>(`/CategoryQuestionnaireTemplates/${questionnaireId}/questions`, question);
  }

  updateQuestion(questionnaireId: string, questionId: string, question: UpdateCategoryQuestionRequest): Observable<ApiResponse<CategoryQuestion>> {
    return this.apiService.put<CategoryQuestion>(`/CategoryQuestionnaireTemplates/${questionnaireId}/questions/${questionId}`, question);
  }

  deleteQuestion(questionnaireId: string, questionId: string): Observable<ApiResponse<any>> {
    return this.apiService.delete(`/CategoryQuestionnaireTemplates/${questionnaireId}/questions/${questionId}`);
  }

  // Response Management
  submitResponse(response: SubmitResponseRequest): Observable<ApiResponse<any>> {
    return this.apiService.post<any>('/responses', response);
  }

  getQuestionnaireForResponse(id: string): Observable<ApiResponse<CategoryQuestionnaireTemplate>> {
    return this.apiService.get<CategoryQuestionnaireTemplate>(`/CategoryQuestionnaireTemplates/${id}/response`);
  }
} 