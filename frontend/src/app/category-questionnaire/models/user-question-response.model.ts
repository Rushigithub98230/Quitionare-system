export interface UserQuestionResponse {
  id: string;
  userId: string;
  questionnaireId: string;
  questionnaireTitle: string;
  categoryName: string;
  startedAt: string;
  completedAt?: string;
  isCompleted: boolean;
  isDraft: boolean;
  timeTaken?: number;
  createdAt: string;
  updatedAt: string;
  deletedAt?: string;
  user?: User;
  questionnaire?: Questionnaire;
  questionResponses: QuestionResponse[];
}

export interface CreateUserQuestionResponseDto {
  userId: string;
  questionnaireId: string;
  isDraft: boolean;
}

export interface UpdateUserQuestionResponseDto {
  isCompleted: boolean;
  isDraft: boolean;
  completedAt?: string;
  timeTaken?: number;
}

// Import types
import { User } from './user.model';
import { Questionnaire } from './questionnaire.model';
import { QuestionResponse } from './question.model'; 