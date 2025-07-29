import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';

import { Category } from '../../models/category.model';
import { CategoryService } from '../../services/category.service';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="categories-container">
      <h1>Categories</h1>
      
      <div *ngIf="loading" class="loading">
        <mat-spinner></mat-spinner>
      </div>
      
      <div *ngIf="!loading && categories.length === 0" class="no-data">
        <p>No categories found.</p>
      </div>
      
      <div *ngIf="!loading && categories.length > 0" class="categories-grid">
        <mat-card *ngFor="let category of categories" class="category-card" 
                  [style.border-left-color]="category.color || '#3f51b5'">
          <mat-card-header>
            <mat-card-title>{{ category.name }}</mat-card-title>
            <mat-card-subtitle *ngIf="category.description">
              {{ category.description }}
            </mat-card-subtitle>
          </mat-card-header>
          
          <mat-card-content>
            <div class="category-info">
              <mat-chip-set>
                <mat-chip>{{ category.questionnaireCount }} questionnaires</mat-chip>
                <mat-chip [color]="category.isActive ? 'accent' : 'warn'">
                  {{ category.isActive ? 'Active' : 'Inactive' }}
                </mat-chip>
              </mat-chip-set>
            </div>
          </mat-card-content>
          
          <mat-card-actions>
            <button mat-button color="primary" 
                    (click)="viewQuestionnaires(category.id)">
              View Questionnaires
            </button>
            <button mat-button color="accent" 
                    (click)="editCategory(category)">
              Edit
            </button>
            <button mat-button color="warn" 
                    (click)="deleteCategory(category.id)"
                    [disabled]="!category.isActive">
              Delete
            </button>
          </mat-card-actions>
        </mat-card>
      </div>
    </div>
  `,
  styles: [`
    .categories-container {
      max-width: 1200px;
      margin: 0 auto;
    }
    
    h1 {
      margin-bottom: 24px;
      color: #333;
    }
    
    .loading {
      display: flex;
      justify-content: center;
      padding: 40px;
    }
    
    .no-data {
      text-align: center;
      padding: 40px;
      color: #666;
    }
    
    .categories-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
      gap: 20px;
    }
    
    .category-card {
      transition: transform 0.2s ease-in-out;
    }
    
    .category-card:hover {
      transform: translateY(-2px);
    }
    
    .category-info {
      margin-top: 16px;
    }
    
    mat-card-actions {
      display: flex;
      justify-content: space-between;
      padding: 16px;
    }
  `]
})
export class CategoriesComponent implements OnInit {
  categories: Category[] = [];
  loading = true;

  constructor(
    private categoryService: CategoryService,
    private router: Router,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.loading = true;
    this.categoryService.getAll().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
        this.snackBar.open('Error loading categories', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  viewQuestionnaires(categoryId: string): void {
    this.router.navigate(['/questionnaires'], { queryParams: { category: categoryId } });
  }

  editCategory(category: Category): void {
    // TODO: Implement edit functionality
    console.log('Edit category:', category);
  }

  deleteCategory(categoryId: string): void {
    if (confirm('Are you sure you want to delete this category?')) {
      this.categoryService.delete(categoryId).subscribe({
        next: () => {
          this.snackBar.open('Category deleted successfully', 'Close', { duration: 3000 });
          this.loadCategories();
        },
        error: (error) => {
          console.error('Error deleting category:', error);
          this.snackBar.open('Error deleting category', 'Close', { duration: 3000 });
        }
      });
    }
  }
} 