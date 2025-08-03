export interface Response {
  id: string;
  userId: string;
  userName?: string;
  questionnaireId: string;
  questionnaireTitle?: string;
  categoryId?: string;
  categoryName?: string;
  startedAt: string;
  completedAt?: string;
  isCompleted: boolean;
  isDraft: boolean;
  timeTaken?: number; // in minutes
  createdAt: string;
  updatedAt: string;
  deletedAt?: string;
  questionCount?: number;
  answeredQuestions?: number;
  completionPercentage?: number;
  user?: User;
  questionnaire?: Questionnaire;
  questionResponses: QuestionResponse[];
}

export interface ResponseSummary {
  id: string;
  questionnaireId: string;
  questionnaireTitle: string;
  categoryId: string;
  categoryName: string;
  userName: string;
  startedAt: string;
  completedAt?: string;
  isCompleted: boolean;
  timeTaken?: number;
  questionCount: number;
  answeredQuestions: number;
  completionPercentage: number;
}

export interface ResponseAnalytics {
  totalResponses: number;
  completedResponses: number;
  draftResponses: number;
  averageCompletionTime: number;
  averageCompletionPercentage: number;
  responsesByCategory: CategoryResponseCount[];
  responsesByQuestionnaire: QuestionnaireResponseCount[];
  responsesByDate: DateResponseCount[];
  topPerformingCategories: CategoryAnalytics[];
  topPerformingQuestionnaires: QuestionnaireAnalytics[];
}

export interface CategoryResponseCount {
  categoryId: string;
  categoryName: string;
  responseCount: number;
  completedCount: number;
  draftCount: number;
  averageCompletionPercentage: number;
}

export interface QuestionnaireResponseCount {
  questionnaireId: string;
  questionnaireTitle: string;
  categoryId: string;
  categoryName: string;
  responseCount: number;
  completedCount: number;
  draftCount: number;
  averageCompletionPercentage: number;
  averageCompletionTime: number;
}

export interface DateResponseCount {
  date: string;
  responseCount: number;
  completedCount: number;
  draftCount: number;
}

export interface CategoryAnalytics {
  categoryId: string;
  categoryName: string;
  responseCount: number;
  totalQuestions: number;
  questionnaireCount: number;
  questionnaires: string[];
  averageCompletionPercentage: number;
  averageCompletionTime: number;
}

export interface QuestionnaireAnalytics {
  questionnaireId: string;
  questionnaireName: string;
  categoryId: string;
  categoryName: string;
  responseCount: number;
  totalQuestions: number;
  averageCompletionPercentage: number;
  averageCompletionTime: number;
  completionRate: number;
}

export interface ResponseFilter {
  categoryId?: string;
  questionnaireId?: string;
  dateRange?: string;
  completionStatus?: string;
  userId?: string;
  startDate?: string;
  endDate?: string;
}

export interface ResponseExportOptions {
  format: 'csv' | 'excel' | 'pdf';
  includeDetails: boolean;
  includeAnalytics: boolean;
  dateRange?: string;
  filters?: ResponseFilter;
}

// Import types
import { User } from './user.model';
import { Questionnaire } from './questionnaire.model';
import { QuestionResponse } from './question.model'; 