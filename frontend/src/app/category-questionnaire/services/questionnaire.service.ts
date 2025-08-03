import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { 
  Questionnaire, 
  CreateQuestionnaireDto, 
  UpdateQuestionnaireDto,
  QuestionnaireSummary 
} from '../models/questionnaire.model';
import { CreateQuestionDto, UpdateQuestionDto } from '../models/question.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class QuestionnaireService {
  constructor(private apiService: ApiService) {}

  getAll(): Observable<ApiResponse<Questionnaire[]>> {
    return this.apiService.get<Questionnaire[]>('/categoryquestionnairetemplates');
  }

  getById(id: string): Observable<ApiResponse<Questionnaire>> {
    return this.apiService.get<Questionnaire>(`/categoryquestionnairetemplates/${id}`);
  }

  getByCategoryId(categoryId: string): Observable<ApiResponse<Questionnaire[]>> {
    return this.apiService.get<Questionnaire[]>(`/categoryquestionnairetemplates/category/${categoryId}`);
  }

  create(questionnaire: CreateQuestionnaireDto): Observable<ApiResponse<Questionnaire>> {
    return this.apiService.post<Questionnaire>('/categoryquestionnairetemplates', questionnaire);
  }

  update(id: string, questionnaire: UpdateQuestionnaireDto): Observable<ApiResponse<Questionnaire>> {
    return this.apiService.put<Questionnaire>(`/categoryquestionnairetemplates/${id}`, questionnaire);
  }

  delete(id: string): Observable<ApiResponse<any>> {
    return this.apiService.delete<any>(`/categoryquestionnairetemplates/${id}`);
  }

  getActive(): Observable<ApiResponse<Questionnaire[]>> {
    return this.apiService.get<Questionnaire[]>('/categoryquestionnairetemplates/active');
  }

  // Question management
  addQuestion(questionnaireId: string, question: CreateQuestionDto): Observable<ApiResponse<any>> {
    return this.apiService.post<any>(`/categoryquestionnairetemplates/${questionnaireId}/questions`, question);
  }

  updateQuestion(questionnaireId: string, questionId: string, question: UpdateQuestionDto): Observable<ApiResponse<any>> {
    return this.apiService.put<any>(`/categoryquestionnairetemplates/${questionnaireId}/questions/${questionId}`, question);
  }

  deleteQuestion(questionnaireId: string, questionId: string): Observable<ApiResponse<any>> {
    return this.apiService.delete<any>(`/categoryquestionnairetemplates/${questionnaireId}/questions/${questionId}`);
  }

  getCountByCategoryId(categoryId: string): Observable<ApiResponse<number>> {
    return this.apiService.get<number>(`/categoryquestionnairetemplates/category/${categoryId}/count`);
  }

  updateOrder(questionnaires: Questionnaire[]): Observable<ApiResponse<Questionnaire[]>> {
    const orderUpdates = questionnaires.map(questionnaire => ({
      id: questionnaire.id,
      displayOrder: questionnaire.displayOrder
    }));

    return this.apiService.put<Questionnaire[]>('/categoryquestionnairetemplates/order', orderUpdates);
  }

  checkTitleExists(title: string): Observable<ApiResponse<{exists: boolean}>> {
    return this.apiService.get<{exists: boolean}>(`/categoryquestionnairetemplates/check-title/${encodeURIComponent(title)}`);
  }
} 