import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Category, CreateCategoryDto, UpdateCategoryDto } from '../models/category.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  constructor(private apiService: ApiService) {}

  getAll(): Observable<ApiResponse<Category[]>> {
    return this.apiService.get<Category[]>('/categories');
  }

  getById(id: string): Observable<ApiResponse<Category>> {
    return this.apiService.get<Category>(`/categories/${id}`);
  }

  create(category: CreateCategoryDto): Observable<ApiResponse<Category>> {
    return this.apiService.post<Category>('/categories', category);
  }

  update(id: string, category: UpdateCategoryDto): Observable<ApiResponse<Category>> {
    return this.apiService.put<Category>(`/categories/${id}`, category);
  }

  delete(id: string): Observable<ApiResponse<any>> {
    return this.apiService.delete<any>(`/categories/${id}`);
  }

  getActive(): Observable<ApiResponse<Category[]>> {
    return this.apiService.get<Category[]>('/categories/active');
  }

  updateOrder(categories: Category[]): Observable<ApiResponse<Category[]>> {
    const orderUpdates = categories.map(category => ({
      id: category.id,
      displayOrder: category.displayOrder
    }));
    
    return this.apiService.put<Category[]>('/categories/order', orderUpdates);
  }

  deleteCategory(id: string): Observable<ApiResponse<any>> {
    return this.apiService.delete<any>(`/categories/${id}`);
  }

  getDeletedCategories(): Observable<ApiResponse<any>> {
    return this.apiService.get<any>('/categories/deleted');
  }

  restoreCategory(id: string): Observable<ApiResponse<any>> {
    return this.apiService.post<any>(`/categories/${id}/restore`, {});
  }

  checkNameExists(name: string): Observable<ApiResponse<{exists: boolean, existsActive: boolean, existsInactive: boolean}>> {
    return this.apiService.get<{exists: boolean, existsActive: boolean, existsInactive: boolean}>(`/categories/check-name/${encodeURIComponent(name)}`);
  }

  deactivate(id: string): Observable<ApiResponse<any>> {
    return this.apiService.post<any>(`/categories/${id}/deactivate`, {});
  }

  reactivate(id: string): Observable<ApiResponse<any>> {
    return this.apiService.post<any>(`/categories/${id}/reactivate`, {});
  }

  getDeactivated(): Observable<ApiResponse<Category[]>> {
    return this.apiService.get<Category[]>('/categories/deactivated');
  }
} 