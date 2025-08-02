import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../core/services/api.service';
import { 
  Questionnaire, 
  CreateQuestionnaireDto, 
  UpdateQuestionnaireDto,
  QuestionnaireSummary 
} from '../../core/models/questionnaire.model';
import { CreateQuestionDto, UpdateQuestionDto } from '../../core/models/question.model';
import { ApiResponse } from '../../core/models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class QuestionnaireService {
  constructor(private apiService: ApiService) {}

  getAll(): Observable<Questionnaire[]> {
    return this.apiService.get<Questionnaire[]>('/categoryquestionnairetemplates').pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to fetch questionnaires');
      })
    );
  }

  getById(id: string): Observable<Questionnaire> {
    return this.apiService.get<Questionnaire>(`/categoryquestionnairetemplates/${id}`).pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to fetch questionnaire');
      })
    );
  }

  getByCategoryId(categoryId: string): Observable<Questionnaire[]> {
    return this.apiService.get<Questionnaire[]>(`/categoryquestionnairetemplates/category/${categoryId}`).pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to fetch questionnaires by category');
      })
    );
  }

  create(questionnaire: CreateQuestionnaireDto): Observable<Questionnaire> {
    return this.apiService.post<Questionnaire>('/categoryquestionnairetemplates', questionnaire).pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to create questionnaire');
      })
    );
  }

  update(id: string, questionnaire: UpdateQuestionnaireDto): Observable<Questionnaire> {
    return this.apiService.put<Questionnaire>(`/categoryquestionnairetemplates/${id}`, questionnaire).pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to update questionnaire');
      })
    );
  }

  delete(id: string): Observable<boolean> {
    return this.apiService.delete<ApiResponse>(`/categoryquestionnairetemplates/${id}`).pipe(
      map(response => {
        if (response.success) {
          return true;
        }
        throw new Error(response.message || 'Failed to delete questionnaire');
      })
    );
  }

  getActive(): Observable<Questionnaire[]> {
    return this.apiService.get<Questionnaire[]>('/categoryquestionnairetemplates/active').pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to fetch active questionnaires');
      })
    );
  }

  // Question management
  addQuestion(questionnaireId: string, question: CreateQuestionDto): Observable<any> {
    return this.apiService.post<any>(`/categoryquestionnairetemplates/${questionnaireId}/questions`, question).pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to add question');
      })
    );
  }

  updateQuestion(questionnaireId: string, questionId: string, question: UpdateQuestionDto): Observable<any> {
    return this.apiService.put<any>(`/categoryquestionnairetemplates/${questionnaireId}/questions/${questionId}`, question).pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to update question');
      })
    );
  }

  deleteQuestion(questionnaireId: string, questionId: string): Observable<boolean> {
    return this.apiService.delete<ApiResponse>(`/categoryquestionnairetemplates/${questionnaireId}/questions/${questionId}`).pipe(
      map(response => {
        if (response.success) {
          return true;
        }
        throw new Error(response.message || 'Failed to delete question');
      })
    );
  }

  getCountByCategoryId(categoryId: string): Observable<number> {
    return this.apiService.get<number>(`/categoryquestionnairetemplates/category/${categoryId}/count`).pipe(
      map(response => {
        if (response.success && response.data !== undefined) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to get questionnaire count');
      })
    );
  }

  updateOrder(questionnaires: Questionnaire[]): Observable<Questionnaire[]> {
    const orderUpdates = questionnaires.map(questionnaire => ({
      id: questionnaire.id,
      displayOrder: questionnaire.displayOrder
    }));

    return this.apiService.put<Questionnaire[]>('/categoryquestionnairetemplates/order', orderUpdates).pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to update questionnaire order');
      })
    );
  }
} 