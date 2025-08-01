import { Category } from './category.model';

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

// Legacy interfaces for backward compatibility
export interface QuestionnaireTemplate extends CategoryQuestionnaireTemplate {}
export interface Question extends CategoryQuestion {}

export interface CategoryQuestionnaireTemplate {
  id: string;
  title: string;
  description?: string;
  categoryId: string;
  categoryName: string;
  isActive: boolean;
  isMandatory: boolean;
  displayOrder: number;
  version: number;
  createdBy: string;
  createdByUserName: string;
  createdAt: string;
  updatedAt: string;
  questionCount: number;
  questions: CategoryQuestion[];
}

export interface CategoryQuestion {
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
  options: QuestionOption[];
}

export interface QuestionOption {
  id: string;
  questionId: string;
  optionText: string;
  displayOrder: number;
  value?: string;
  isCorrect: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateCategoryQuestionnaireTemplateRequest {
  title: string;
  description?: string;
  categoryId: string;
  isActive: boolean;
  isMandatory: boolean;
  displayOrder: number;
  version: number;
  createdBy: string; // Added to match backend DTO
  questions: CreateCategoryQuestionRequest[]; // Added to match backend DTO
}

export interface UpdateCategoryQuestionnaireTemplateRequest {
  title: string;
  description?: string;
  categoryId: string; // Added to match backend DTO
  isActive: boolean;
  isMandatory: boolean;
  displayOrder: number;
  version: number;
  questions: CreateCategoryQuestionRequest[]; // Added to match backend DTO
}

export interface CreateCategoryQuestionRequest {
  questionnaireId: string; // Added to match backend DTO
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
  options: CreateQuestionOptionRequest[]; // Added to match backend DTO
}

export interface UpdateCategoryQuestionRequest {
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
  options: CreateQuestionOptionRequest[]; // Added to match backend DTO
}

export interface CreateQuestionOptionRequest {
  optionText: string;
  displayOrder: number;
  value?: string;
  isCorrect: boolean;
}

export interface UpdateQuestionOptionRequest {
  optionText: string;
  displayOrder: number;
  value?: string;
  isCorrect: boolean;
}

export interface SubmitResponseRequest {
  questionnaireId: string;
  responses: QuestionResponse[];
}

export interface QuestionResponse {
  questionId: string;
  textResponse?: string;
  numberResponse?: number;
  dateResponse?: string;
  selectedOptionIds: string[];
  fileUrl?: string;
  imageUrl?: string;
} 