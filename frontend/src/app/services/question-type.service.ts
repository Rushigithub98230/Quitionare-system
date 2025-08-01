import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { QuestionType } from '../models/questionnaire.model';

@Injectable({
  providedIn: 'root'
})
export class QuestionTypeService {
  constructor(private apiService: ApiService) {}

  getQuestionTypes(): Observable<any> {
    return this.apiService.get('/question-types');
  }

  getQuestionTypeById(id: string): Observable<any> {
    return this.apiService.get(`/question-types/${id}`);
  }

  createQuestionType(questionType: Partial<QuestionType>): Observable<any> {
    return this.apiService.post('/question-types', questionType);
  }

  updateQuestionType(id: string, questionType: Partial<QuestionType>): Observable<any> {
    return this.apiService.put(`/question-types/${id}`, questionType);
  }

  deleteQuestionType(id: string): Observable<any> {
    return this.apiService.delete(`/question-types/${id}`);
  }

  getActiveQuestionTypes(): Observable<any> {
    return this.apiService.get('/question-types/active');
  }
} 