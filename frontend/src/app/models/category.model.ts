export interface Category {
  id: string;
  name: string;
  description: string;
  icon?: string;
  color?: string;
  isActive: boolean;
  displayOrder: number;
  features: string[];
  consultationDescription?: string;
  basePrice?: number;
  requiresQuestionnaireAssessment?: boolean;
  allowsMedicationDelivery?: boolean;
  allowsFollowUpMessaging?: boolean;
  oneTimeConsultationDurationMinutes?: number;
  isMostPopular?: boolean;
  isTrending?: boolean;
  hasQuestionnaireTemplate?: boolean;
  createdAt: string;
  updatedAt?: string;
  deletedAt?: string;
}

export interface CreateCategoryRequest {
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

export interface UpdateCategoryRequest {
  name?: string;
  description?: string;
  icon?: string;
  color?: string;
  isActive?: boolean;
  displayOrder?: number;
  features?: string;
  consultationDescription?: string;
  basePrice?: number;
  requiresQuestionnaireAssessment?: boolean;
  allowsMedicationDelivery?: boolean;
  allowsFollowUpMessaging?: boolean;
  oneTimeConsultationDurationMinutes?: number;
  isMostPopular?: boolean;
  isTrending?: boolean;
} 