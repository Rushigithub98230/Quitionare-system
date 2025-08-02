export interface Question {
  id: string;
  questionnaireId: string;
  questionText: string;
  questionTypeId: string;
  questionTypeName: string;
  isRequired: boolean;
  displayOrder: number;
  sectionName?: string;
  helpText?: string;
  placeholder?: string;
  minLength?: number;
  maxLength?: number;
  minValue?: number;
  maxValue?: number;
  imageUrl?: string;
  imageAltText?: string;
  validationRules?: string;
  conditionalLogic?: string;
  settings?: string;
  createdAt: string;
  updatedAt: string;
  deletedAt?: string;
  questionnaire?: Questionnaire;
  questionType?: QuestionType;
  options: QuestionOption[];
  responses: QuestionResponse[];
}

export interface CreateQuestionDto {
  questionnaireId: string;
  questionText: string;
  questionTypeId: string;
  isRequired: boolean;
  displayOrder?: number; // Optional - will be auto-generated
  sectionName?: string;
  helpText?: string;
  placeholder?: string;
  minLength?: number;
  maxLength?: number;
  minValue?: number;
  maxValue?: number;
  imageUrl?: string;
  imageAltText?: string;
  validationRules?: string;
  conditionalLogic?: string;
  settings?: string;
}

export interface UpdateQuestionDto {
  questionText: string;
  questionTypeId: string;
  isRequired: boolean;
  displayOrder: number;
  sectionName?: string;
  helpText?: string;
  placeholder?: string;
  minLength?: number;
  maxLength?: number;
  minValue?: number;
  maxValue?: number;
  imageUrl?: string;
  imageAltText?: string;
  validationRules?: string;
  conditionalLogic?: string;
  settings?: string;
}

export interface QuestionType {
  id: string;
  typeName: string;
  displayName: string;
  description?: string;
  hasOptions: boolean;
  supportsFileUpload: boolean;
  supportsImage: boolean;
  validationRules?: string;
  isActive: boolean;
}

export interface QuestionOption {
  id: string;
  questionId: string;
  optionText: string;
  optionValue: string;
  displayOrder: number;
  isCorrect?: boolean;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  question?: Question;
}

export interface CreateQuestionOptionDto {
  questionId: string;
  optionText: string;
  optionValue: string;
  displayOrder: number;
  isCorrect?: boolean;
  isActive: boolean;
}

export interface UpdateQuestionOptionDto {
  optionText: string;
  optionValue: string;
  displayOrder: number;
  isCorrect?: boolean;
  isActive: boolean;
}

export interface QuestionResponse {
  id: string;
  questionId: string;
  textResponse?: string;
  numberResponse?: number;
  dateResponse?: string;
  selectedOptionIds: string[];
  fileUrl?: string;
  imageUrl?: string;
  createdAt: string;
  updatedAt: string;
  deletedAt?: string;
  question?: Question;
  userQuestionResponse?: UserQuestionResponse;
}

// Import types
import { Questionnaire } from './questionnaire.model';
import { UserQuestionResponse } from '../../core/models/user-question-response.model'; 