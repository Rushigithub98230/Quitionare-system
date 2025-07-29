import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { 
  Questionnaire, 
  CreateQuestionnaireDto, 
  UpdateQuestionnaireDto 
} from '../models/questionnaire.model';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class QuestionnaireService {
  constructor(private apiService: ApiService) { }

  getAll(includeInactive = false): Observable<Questionnaire[]> {
    return this.apiService.get<Questionnaire[]>(`/questionnaires?includeInactive=${includeInactive}`);
  }

  getByCategory(categoryId: string): Observable<Questionnaire[]> {
    return this.apiService.get<Questionnaire[]>(`/questionnaires/category/${categoryId}`);
  }

  getById(id: string): Observable<Questionnaire> {
    return this.apiService.get<Questionnaire>(`/questionnaires/${id}`);
  }

  getForResponse(id: string): Observable<Questionnaire> {
    return this.apiService.get<Questionnaire>(`/questionnaires/${id}/response`);
  }

  create(questionnaire: CreateQuestionnaireDto): Observable<Questionnaire> {
    return this.apiService.post<Questionnaire>('/questionnaires', questionnaire);
  }

  update(id: string, questionnaire: UpdateQuestionnaireDto): Observable<Questionnaire> {
    return this.apiService.put<Questionnaire>(`/questionnaires/${id}`, questionnaire);
  }

  delete(id: string): Observable<void> {
    return this.apiService.delete(`/questionnaires/${id}`);
  }
} 