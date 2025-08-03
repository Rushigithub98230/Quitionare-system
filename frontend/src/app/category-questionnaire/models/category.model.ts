export interface Category {
  id: string;
  name: string;
  description?: string;
  icon?: string;
  color?: string;
  isActive: boolean;
  displayOrder: number;
  features?: string;
  consultationDescription?: string;
  basePrice: number;
  requiresQuestionnaireAssessment: boolean;
  allowsMedicationDelivery: boolean;
  allowsFollowUpMessaging: boolean;
  oneTimeConsultationDurationMinutes: number;
  isMostPopular: boolean;
  isTrending: boolean;
  createdDate: string;
  updatedDate: string;
  deletedDate?: string;
  hasQuestionnaireTemplate: boolean;
}

export interface CreateCategoryDto {
  name: string;
  description?: string;
  icon?: string;
  color?: string;
  displayOrder?: number; // Optional - will be auto-generated
  features?: string;
  consultationDescription?: string;
  basePrice: number;
  requiresQuestionnaireAssessment: boolean;
  allowsMedicationDelivery: boolean;
  allowsFollowUpMessaging: boolean;
  oneTimeConsultationDurationMinutes: number;
  isMostPopular: boolean;
  isTrending: boolean;
  isActive: boolean;
}

export interface UpdateCategoryDto {
  name: string;
  description?: string;
  icon?: string;
  color?: string;
  isActive: boolean;
  displayOrder: number;
  features?: string;
  consultationDescription?: string;
  basePrice: number;
  requiresQuestionnaireAssessment: boolean;
  allowsMedicationDelivery: boolean;
  allowsFollowUpMessaging: boolean;
  oneTimeConsultationDurationMinutes: number;
  isMostPopular: boolean;
  isTrending: boolean;
} 