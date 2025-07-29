export interface QuestionOption {
  id: string;
  optionText: string;
  optionValue: string;
  displayOrder: number;
  hasTextInput: boolean;
}

export interface Question {
  id: string;
  questionText: string;
  questionType: string;
  questionTypeDisplayName: string;
  isRequired: boolean;
  displayOrder: number;
  sectionName?: string;
  helpText?: string;
  placeholder?: string;
  imageUrl?: string;
  imageAltText?: string;
  validationRules?: any;
  conditionalLogic?: any;
  settings?: any;
  options: QuestionOption[];
}

export interface Questionnaire {
  id: string;
  title: string;
  description?: string;
  categoryId: string;
  categoryName: string;
  isActive: boolean;
  isMandatory: boolean;
  displayOrder: number;
  version: string;
  createdAt: string;
  questions: Question[];
}

export interface CreateQuestionnaireDto {
  title: string;
  description?: string;
  categoryId: string;
  isMandatory: boolean;
  displayOrder: number;
  questions: CreateQuestionDto[];
}

export interface CreateQuestionDto {
  questionText: string;
  questionTypeId: string;
  isRequired: boolean;
  displayOrder: number;
  sectionName?: string;
  helpText?: string;
  placeholder?: string;
  imageUrl?: string;
  imageAltText?: string;
  validationRules?: any;
  conditionalLogic?: any;
  settings?: any;
  options: CreateQuestionOptionDto[];
}

export interface CreateQuestionOptionDto {
  optionText: string;
  optionValue: string;
  displayOrder: number;
  hasTextInput: boolean;
}

export interface UpdateQuestionnaireDto extends CreateQuestionnaireDto {
  isActive: boolean;
}

export interface UpdateQuestionDto extends CreateQuestionDto {
  id?: string;
} 