import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService, ApiResponse } from './api.service';
import { Category, CreateCategoryRequest, UpdateCategoryRequest } from '../models/category.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  constructor(private apiService: ApiService) {}

  getCategories(): Observable<ApiResponse<Category[]>> {
    return this.apiService.get<Category[]>('/categories');
  }

  getCategory(id: string): Observable<ApiResponse<Category>> {
    return this.apiService.get<Category>(`/categories/${id}`);
  }

  createCategory(category: CreateCategoryRequest): Observable<ApiResponse<Category>> {
    return this.apiService.post<Category>('/categories', category);
  }

  updateCategory(id: string, category: UpdateCategoryRequest): Observable<ApiResponse<Category>> {
    return this.apiService.put<Category>(`/categories/${id}`, category);
  }

  deleteCategory(id: string): Observable<ApiResponse<any>> {
    return this.apiService.delete(`/categories/${id}`);
  }

  getCategoriesWithTemplates(): Observable<ApiResponse<Category[]>> {
    return this.apiService.get<Category[]>('/categories');
  }

  getCategoryWithTemplate(id: string): Observable<ApiResponse<Category>> {
    return this.apiService.get<Category>(`/categories/${id}`);
  }
} 