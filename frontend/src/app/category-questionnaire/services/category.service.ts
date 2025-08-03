import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from './api.service';
import { Category, CreateCategoryDto, UpdateCategoryDto } from '../models/category.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  constructor(private apiService: ApiService) {}

  getAll(): Observable<Category[]> {
    return this.apiService.get<Category[]>('/categories').pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to fetch categories');
      })
    );
  }

  getById(id: string): Observable<Category> {
    return this.apiService.get<Category>(`/categories/${id}`).pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to fetch category');
      })
    );
  }

  create(category: CreateCategoryDto): Observable<Category> {
    return this.apiService.post<Category>('/categories', category).pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to create category');
      })
    );
  }

  update(id: string, category: UpdateCategoryDto): Observable<Category> {
    return this.apiService.put<Category>(`/categories/${id}`, category).pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to update category');
      })
    );
  }

  delete(id: string): Observable<boolean> {
    return this.apiService.delete<ApiResponse>(`/categories/${id}`).pipe(
      map(response => {
        if (response.success) {
          return true;
        }
        throw new Error(response.message || 'Failed to delete category');
      })
    );
  }

  getActive(): Observable<Category[]> {
    return this.apiService.get<Category[]>('/categories/active').pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to fetch active categories');
      })
    );
  }

  updateOrder(categories: Category[]): Observable<Category[]> {
    const orderUpdates = categories.map(category => ({
      id: category.id,
      displayOrder: category.displayOrder
    }));
    
    return this.apiService.put<Category[]>('/categories/order', orderUpdates).pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to update category order');
      })
    );
  }

  deleteCategory(id: string): Observable<any> {
    return this.apiService.delete<any>(`/categories/${id}`);
  }

  getDeletedCategories(): Observable<any> {
    return this.apiService.get<any>('/categories/deleted');
  }

  restoreCategory(id: string): Observable<any> {
    return this.apiService.post<any>(`/categories/${id}/restore`, {});
  }
} 