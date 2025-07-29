export interface Category {
  id: string;
  name: string;
  description?: string;
  color?: string;
  isActive: boolean;
  displayOrder: number;
  createdAt: string;
  updatedAt: string;
  questionnaireCount: number;
}

export interface CreateCategoryDto {
  name: string;
  description?: string;
  color?: string;
  displayOrder: number;
}

export interface UpdateCategoryDto {
  name: string;
  description?: string;
  color?: string;
  isActive: boolean;
  displayOrder: number;
} 