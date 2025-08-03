export interface Questionnaire {
  id: string;
  title: string;
  description?: string;
  categoryId: string;
  categoryName: string;
  isActive: boolean;
  isMandatory: boolean;
  displayOrder: number;
  version: number;
  createdBy: number;
  createdByUserName: string;
  createdDate: string;
  updatedDate: string;
  deletedDate?: string;
  questionCount: number;
  category?: Category;
  createdByUser?: User;
  questions: Question[];
  userResponses: UserQuestionResponse[];
}

export interface CreateQuestionnaireDto {
  title: string;
  description?: string;
  categoryId: string;
  isActive: boolean;
  isMandatory: boolean;
  displayOrder: number;
  version: number;
  createdBy?: number;
  questions?: CreateQuestionDto[];
}

export interface UpdateQuestionnaireDto {
  title: string;
  description?: string;
  categoryId: string;
  isActive: boolean;
  isMandatory: boolean;
  displayOrder: number;
  version: number;
  questions?: CreateQuestionDto[];
}

export interface QuestionnaireSummary {
  id: string;
  title: string;
  description?: string;
  categoryId: string;
  isActive: boolean;
  isMandatory: boolean;
  displayOrder: number;
  version: number;
  createdBy: number;
  createdDate: string;
  updatedDate: string;
  deletedDate?: string;
  category?: Category;
  createdByUser?: User;
  questionCount: number;
  responseCount: number;
}

// Import types
import { Category } from './category.model';
import { User } from './user.model';
import { Question, CreateQuestionDto } from './question.model';
import { UserQuestionResponse } from './user-question-response.model'; 