import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { 
  Question, 
  CreateQuestionDto, 
  UpdateQuestionDto,
  QuestionOption,
  CreateQuestionOptionDto,
  UpdateQuestionOptionDto,
  QuestionType
} from '../models/question.model';

@Injectable({
  providedIn: 'root'
})
export class QuestionService {
  private apiUrl = `${environment.apiUrl}/questions`;

  constructor(private http: HttpClient) { }

  // Question CRUD operations
  getAllQuestions(): Observable<Question[]> {
    return this.http.get<Question[]>(this.apiUrl);
  }

  getQuestionById(id: string): Observable<Question> {
    return this.http.get<Question>(`${this.apiUrl}/${id}`);
  }

  getQuestionsByQuestionnaireId(questionnaireId: string): Observable<Question[]> {
    return this.http.get<Question[]>(`${environment.apiUrl}/categoryquestionnairetemplates/${questionnaireId}/questions`);
  }

  createQuestion(question: CreateQuestionDto): Observable<Question> {
    return this.http.post<Question>(`${environment.apiUrl}/categoryquestionnairetemplates/${question.questionnaireId}/questions`, question);
  }

  updateQuestion(questionnaireId: string, questionId: string, question: UpdateQuestionDto): Observable<Question> {
    return this.http.put<Question>(`${environment.apiUrl}/categoryquestionnairetemplates/${questionnaireId}/questions/${questionId}`, question);
  }

  deleteQuestion(questionnaireId: string, questionId: string): Observable<boolean> {
    return this.http.delete<boolean>(`${environment.apiUrl}/categoryquestionnairetemplates/${questionnaireId}/questions/${questionId}`);
  }

  // Question Options CRUD operations
  getQuestionOptions(questionnaireId: string, questionId: string): Observable<QuestionOption[]> {
    return this.http.get<QuestionOption[]>(`${environment.apiUrl}/categoryquestionnairetemplates/${questionnaireId}/questions/${questionId}/options`);
  }

  createQuestionOption(questionnaireId: string, questionId: string, option: CreateQuestionOptionDto): Observable<QuestionOption> {
    return this.http.post<QuestionOption>(`${environment.apiUrl}/categoryquestionnairetemplates/${questionnaireId}/questions/${questionId}/options`, option);
  }

  updateQuestionOption(questionnaireId: string, questionId: string, optionId: string, option: UpdateQuestionOptionDto): Observable<QuestionOption> {
    return this.http.put<QuestionOption>(`${environment.apiUrl}/categoryquestionnairetemplates/${questionnaireId}/questions/${questionId}/options/${optionId}`, option);
  }

  deleteQuestionOption(questionnaireId: string, questionId: string, optionId: string): Observable<boolean> {
    return this.http.delete<boolean>(`${environment.apiUrl}/categoryquestionnairetemplates/${questionnaireId}/questions/${questionId}/options/${optionId}`);
  }

  // Question Types
  getQuestionTypes(): Observable<QuestionType[]> {
    return this.http.get<any>(`${environment.apiUrl}/questiontypes`).pipe(
      map(response => {
        // Handle JsonModel wrapper
        if (response && response.data && Array.isArray(response.data)) {
          return response.data;
        } else if (Array.isArray(response)) {
          return response;
        } else {
          console.warn('Unexpected question types response format:', response);
          return [];
        }
      })
    );
  }

  getQuestionTypeById(id: string): Observable<QuestionType> {
    return this.http.get<any>(`${environment.apiUrl}/questiontypes/${id}`).pipe(
      map(response => {
        // Handle JsonModel wrapper
        if (response && response.data) {
          return response.data;
        } else {
          return response;
        }
      })
    );
  }

  updateQuestionOrder(questionnaireId: string, questions: Question[]): Observable<Question[]> {
    const orderUpdates = questions.map(question => ({
      id: question.id,
      displayOrder: question.displayOrder
    }));
    
    return this.http.put<any>(`${environment.apiUrl}/categoryquestionnairetemplates/${questionnaireId}/questions/order`, orderUpdates).pipe(
      map(response => {
        // Handle JsonModel wrapper
        if (response && response.data) {
          return response.data;
        } else {
          return response;
        }
      })
    );
  }
} 