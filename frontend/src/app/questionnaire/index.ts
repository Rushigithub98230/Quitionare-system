// Questionnaire Module - Complete Questionnaire System
// This module contains all questionnaire-related functionality for easy extraction and reuse

// Models
export * from './models/question.model';
export * from './models/questionnaire.model';

// Services
export { QuestionService } from './services/question.service';
export { QuestionnaireService } from './services/questionnaire.service';
export { OrderService } from './services/order.service';

// Components
export { QuestionListComponent } from './components/question-list/question-list.component';
export { OrderManagerComponent } from './components/order-manager/order-manager.component';

// Dialogs
export { QuestionDialogComponent } from './dialogs/question-dialog/question-dialog.component';
export { QuestionnaireDialogComponent } from './dialogs/questionnaire-dialog/questionnaire-dialog.component';

// Module
export { QuestionnaireModule } from './questionnaire.module';

// Types and Interfaces
export interface QuestionnaireConfig {
  apiUrl: string;
  enableOrderManagement?: boolean;
  enableDragDrop?: boolean;
  enableValidation?: boolean;
}

export interface QuestionnaireFeatureFlags {
  orderManagement: boolean;
  dragDrop: boolean;
  validation: boolean;
  categories: boolean;
  questions: boolean;
  questionnaires: boolean;
}

// Utility functions
export const QUESTIONNAIRE_CONSTANTS = {
  DEFAULT_PAGE_SIZE: 10,
  MAX_QUESTIONS_PER_QUESTIONNAIRE: 100,
  MAX_OPTIONS_PER_QUESTION: 10,
  SUPPORTED_QUESTION_TYPES: ['MultipleChoice', 'SingleChoice', 'Text', 'Number', 'Boolean'],
  ORDER_MANAGEMENT_ENABLED: true,
  DRAG_DROP_ENABLED: true,
  VALIDATION_ENABLED: true
} as const;

// Configuration helper
export function createQuestionnaireConfig(config: Partial<QuestionnaireConfig> = {}): QuestionnaireConfig {
  return {
    apiUrl: config.apiUrl || '',
    enableOrderManagement: config.enableOrderManagement ?? true,
    enableDragDrop: config.enableDragDrop ?? true,
    enableValidation: config.enableValidation ?? true
  };
}

// Feature flags helper
export function createFeatureFlags(flags: Partial<QuestionnaireFeatureFlags> = {}): QuestionnaireFeatureFlags {
  return {
    orderManagement: flags.orderManagement ?? true,
    dragDrop: flags.dragDrop ?? true,
    validation: flags.validation ?? true,
    categories: flags.categories ?? true,
    questions: flags.questions ?? true,
    questionnaires: flags.questionnaires ?? true
  };
} 