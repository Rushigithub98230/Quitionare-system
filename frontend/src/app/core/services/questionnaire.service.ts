import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { 
  Questionnaire, 
  CreateQuestionnaireDto, 
  UpdateQuestionnaireDto,
  QuestionnaireSummary
} from '../models/questionnaire.model';

@Injectable({
  providedIn: 'root'
})
export class QuestionnaireService {
  private apiUrl = `${environment.apiUrl}/categoryquestionnairetemplates`;

  constructor(private http: HttpClient) { }

  // Questionnaire CRUD operations
  getAllQuestionnaires(): Observable<Questionnaire[]> {
    return this.http.get<any>(this.apiUrl).pipe(
      map(response => {
        if (response && response.data && Array.isArray(response.data)) {
          return response.data;
        } else if (Array.isArray(response)) {
          return response;
        } else {
          console.warn('Unexpected questionnaires response format:', response);
          return [];
        }
      })
    );
  }

  getQuestionnaireById(id: string): Observable<Questionnaire> {
    return this.http.get<any>(`${this.apiUrl}/${id}`).pipe(
      map(response => {
        if (response && response.data) {
          return response.data;
        } else {
          return response;
        }
      })
    );
  }

  getQuestionnairesByCategory(categoryId: string): Observable<Questionnaire[]> {
    return this.http.get<any>(`${environment.apiUrl}/categories/${categoryId}/questionnaires`).pipe(
      map(response => {
        if (response && response.data && Array.isArray(response.data)) {
          return response.data;
        } else if (Array.isArray(response)) {
          return response;
        } else {
          console.warn('Unexpected questionnaires response format:', response);
          return [];
        }
      })
    );
  }

  createQuestionnaire(questionnaire: CreateQuestionnaireDto): Observable<Questionnaire> {
    return this.http.post<Questionnaire>(this.apiUrl, questionnaire);
  }

  updateQuestionnaire(id: string, questionnaire: UpdateQuestionnaireDto): Observable<Questionnaire> {
    return this.http.put<Questionnaire>(`${this.apiUrl}/${id}`, questionnaire);
  }

  deleteQuestionnaire(id: string): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}/${id}`);
  }

  // Questionnaire Summary
  getQuestionnaireSummaries(): Observable<QuestionnaireSummary[]> {
    return this.http.get<any>(`${this.apiUrl}/summaries`).pipe(
      map(response => {
        if (response && response.data && Array.isArray(response.data)) {
          return response.data;
        } else if (Array.isArray(response)) {
          return response;
        } else {
          console.warn('Unexpected questionnaire summaries response format:', response);
          return [];
        }
      })
    );
  }

  // Questionnaire Order Management
  updateQuestionnaireOrder(questionnaires: Questionnaire[]): Observable<Questionnaire[]> {
    const orderUpdates = questionnaires.map(questionnaire => ({
      id: questionnaire.id,
      displayOrder: questionnaire.displayOrder
    }));
    
    return this.http.put<any>(`${this.apiUrl}/order`, orderUpdates).pipe(
      map(response => {
        if (response && response.data) {
          return response.data;
        } else {
          return response;
        }
      })
    );
  }

  // Questionnaire Preview
  previewQuestionnaire(id: string): Observable<Questionnaire> {
    return this.http.get<any>(`${this.apiUrl}/${id}/preview`).pipe(
      map(response => {
        if (response && response.data) {
          return response.data;
        } else {
          return response;
        }
      })
    );
  }

  // Questionnaire Statistics
  getQuestionnaireStats(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}/stats`).pipe(
      map(response => {
        if (response && response.data) {
          return response.data;
        } else {
          return response;
        }
      })
    );
  }
} 