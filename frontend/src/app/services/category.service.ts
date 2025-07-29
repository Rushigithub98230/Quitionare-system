import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Category, CreateCategoryDto, UpdateCategoryDto } from '../models/category.model';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  constructor(private apiService: ApiService) { }

  getAll(includeInactive = false): Observable<Category[]> {
    return this.apiService.get<Category[]>(`/categories?includeInactive=${includeInactive}`);
  }

  getById(id: string): Observable<Category> {
    return this.apiService.get<Category>(`/categories/${id}`);
  }

  create(category: CreateCategoryDto): Observable<Category> {
    return this.apiService.post<Category>('/categories', category);
  }

  update(id: string, category: UpdateCategoryDto): Observable<Category> {
    return this.apiService.put<Category>(`/categories/${id}`, category);
  }

  delete(id: string): Observable<void> {
    return this.apiService.delete(`/categories/${id}`);
  }
} 