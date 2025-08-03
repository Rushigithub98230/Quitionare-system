import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { 
  Question, 
  CreateQuestionDto, 
  UpdateQuestionDto,
  QuestionOption,
  CreateQuestionOptionDto,
  UpdateQuestionOptionDto,
  QuestionType
} from '../models/question.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class QuestionService {
  constructor(private apiService: ApiService) { }

  // Question CRUD operations
  getAllQuestions(): Observable<ApiResponse<Question[]>> {
    return this.apiService.get<Question[]>('/questions');
  }

  getQuestionById(id: string): Observable<ApiResponse<Question>> {
    return this.apiService.get<Question>(`/questions/${id}`);
  }

  getQuestionsByQuestionnaireId(questionnaireId: string): Observable<ApiResponse<Question[]>> {
    return this.apiService.get<Question[]>(`/categoryquestionnairetemplates/${questionnaireId}/questions`);
  }

  createQuestion(question: CreateQuestionDto): Observable<ApiResponse<Question>> {
    return this.apiService.post<Question>(`/categoryquestionnairetemplates/${question.questionnaireId}/questions`, question);
  }

  updateQuestion(questionnaireId: string, questionId: string, question: UpdateQuestionDto): Observable<ApiResponse<Question>> {
    return this.apiService.put<Question>(`/categoryquestionnairetemplates/${questionnaireId}/questions/${questionId}`, question);
  }

  deleteQuestion(questionnaireId: string, questionId: string): Observable<ApiResponse<any>> {
    return this.apiService.delete<any>(`/categoryquestionnairetemplates/${questionnaireId}/questions/${questionId}`);
  }

  // Question Options CRUD operations
  getQuestionOptions(questionnaireId: string, questionId: string): Observable<ApiResponse<QuestionOption[]>> {
    return this.apiService.get<QuestionOption[]>(`/categoryquestionnairetemplates/${questionnaireId}/questions/${questionId}/options`);
  }

  createQuestionOption(questionnaireId: string, questionId: string, option: CreateQuestionOptionDto): Observable<ApiResponse<QuestionOption>> {
    return this.apiService.post<QuestionOption>(`/categoryquestionnairetemplates/${questionnaireId}/questions/${questionId}/options`, option);
  }

  updateQuestionOption(questionnaireId: string, questionId: string, optionId: string, option: UpdateQuestionOptionDto): Observable<ApiResponse<QuestionOption>> {
    return this.apiService.put<QuestionOption>(`/categoryquestionnairetemplates/${questionnaireId}/questions/${questionId}/options/${optionId}`, option);
  }

  deleteQuestionOption(questionnaireId: string, questionId: string, optionId: string): Observable<ApiResponse<any>> {
    return this.apiService.delete<any>(`/categoryquestionnairetemplates/${questionnaireId}/questions/${questionId}/options/${optionId}`);
  }

  // Question Types
  getQuestionTypes(): Observable<ApiResponse<QuestionType[]>> {
    return this.apiService.get<QuestionType[]>('/questiontypes');
  }

  getQuestionTypeById(id: string): Observable<ApiResponse<QuestionType>> {
    return this.apiService.get<QuestionType>(`/questiontypes/${id}`);
  }

  updateQuestionOrder(questionnaireId: string, questions: Question[]): Observable<ApiResponse<Question[]>> {
    const orderUpdates = questions.map(question => ({
      id: question.id,
      displayOrder: question.displayOrder
    }));
    
    return this.apiService.put<Question[]>(`/categoryquestionnairetemplates/${questionnaireId}/questions/order`, orderUpdates);
  }
} 