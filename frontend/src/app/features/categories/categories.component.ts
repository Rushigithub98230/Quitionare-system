import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CategoryService } from '../../services/category.service';
import { Category } from '../../models/category.model';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule
  ],
  template: `
    <div class="categories-container">
      <div class="header">
        <h1>Categories</h1>
        <p>Select a category to view available questionnaires</p>
      </div>

      <div class="categories-grid" *ngIf="categories.length > 0; else noCategories">
        <mat-card *ngFor="let category of categories" class="category-card" 
                  [class.active]="category.isActive"
                  (click)="selectCategory(category)">
          <mat-card-header>
            <div class="category-header">
              <div class="category-color" [style.background-color]="category.color"></div>
              <div class="category-info">
                <mat-card-title>{{ category.name }}</mat-card-title>
                <mat-card-subtitle>{{ category.description }}</mat-card-subtitle>
              </div>
            </div>
          </mat-card-header>
          
          <mat-card-content>
            <div class="category-status">
              <mat-chip [color]="category.isActive ? 'primary' : 'warn'" selected>
                {{ category.isActive ? 'Active' : 'Inactive' }}
              </mat-chip>
            </div>
          </mat-card-content>
          
          <mat-card-actions>
            <button mat-raised-button color="primary" (click)="viewQuestionnaires(category)">
              <mat-icon>quiz</mat-icon>
              View Questionnaires
            </button>
          </mat-card-actions>
        </mat-card>
      </div>

      <ng-template #noCategories>
        <div class="empty-state">
          <mat-icon>category</mat-icon>
          <h2>No Categories Available</h2>
          <p>There are no categories available for your account. Please contact an administrator.</p>
        </div>
      </ng-template>
    </div>
  `,
  styles: [`
    .categories-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 20px;
    }

    .header {
      text-align: center;
      margin-bottom: 40px;
    }

    .header h1 {
      color: #333;
      margin-bottom: 8px;
      font-size: 2.5rem;
      font-weight: 300;
    }

    .header p {
      color: #666;
      font-size: 1.1rem;
      margin: 0;
    }

    .categories-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
      gap: 24px;
    }

    .category-card {
      cursor: pointer;
      transition: all 0.3s ease;
      border-radius: 12px;
      overflow: hidden;
    }

    .category-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
    }

    .category-card.active {
      border: 2px solid #4caf50;
    }

    .category-header {
      display: flex;
      align-items: center;
      gap: 16px;
    }

    .category-color {
      width: 40px;
      height: 40px;
      border-radius: 50%;
      border: 3px solid #fff;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .category-info {
      flex: 1;
    }

    .category-info mat-card-title {
      margin-bottom: 4px;
      font-size: 1.3rem;
      font-weight: 600;
    }

    .category-info mat-card-subtitle {
      color: #666;
      font-size: 0.9rem;
    }

    .category-status {
      margin: 16px 0;
    }

    mat-card-actions {
      padding: 16px;
      display: flex;
      justify-content: center;
    }

    .empty-state {
      text-align: center;
      padding: 60px 20px;
      color: #666;
    }

    .empty-state mat-icon {
      font-size: 80px;
      width: 80px;
      height: 80px;
      color: #ccc;
      margin-bottom: 24px;
    }

    .empty-state h2 {
      margin: 0 0 16px 0;
      color: #333;
      font-weight: 300;
    }

    .empty-state p {
      margin: 0;
      font-size: 1.1rem;
      line-height: 1.6;
    }

    @media (max-width: 768px) {
      .categories-grid {
        grid-template-columns: 1fr;
      }

      .header h1 {
        font-size: 2rem;
      }
    }
  `]
})
export class CategoriesComponent implements OnInit {
  categories: Category[] = [];
  isLoading = false;

  constructor(
    private categoryService: CategoryService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading = true;
    this.categoryService.getCategories().subscribe({
      next: (response) => {
        if (response.success) {
          this.categories = response.data.filter(category => category.isActive);
        } else {
          this.snackBar.open(response.message || 'Failed to load categories', 'Close', { duration: 5000 });
        }
      },
      error: (error) => {
        this.snackBar.open('Error loading categories', 'Close', { duration: 5000 });
        console.error('Error loading categories:', error);
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  selectCategory(category: Category): void {
    if (category.isActive) {
      this.viewQuestionnaires(category);
    } else {
      this.snackBar.open('This category is currently inactive', 'Close', { duration: 3000 });
    }
  }

  viewQuestionnaires(category: Category): void {
    this.router.navigate(['/questionnaires'], { 
      queryParams: { categoryId: category.id, categoryName: category.name }
    });
  }
} 