export interface UserQuestionResponse {
  id: string;
  userId: number;
  questionnaireId: string;
  questionnaireTitle: string;
  categoryName: string;
  startedAt: string;
  completedAt?: string;
  isCompleted: boolean;
  isDraft: boolean;
  timeTaken?: number;
  createdDate: string;
  updatedDate: string;
  deletedDate?: string;
  user?: User;
  questionnaire?: Questionnaire;
  questionResponses: QuestionResponse[];
}

export interface CreateUserQuestionResponseDto {
  userId: number;
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