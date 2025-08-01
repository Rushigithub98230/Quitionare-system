import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { CategoryService } from '../../../services/category.service';
import { Category } from '../../../models/category.model';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatCardModule,
    MatChipsModule
  ],
  template: `
    <div class="categories-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Category Management</mat-card-title>
          <mat-card-subtitle>Manage questionnaire categories</mat-card-subtitle>
        </mat-card-header>
        <mat-card-content>
          <div class="categories-grid">
            <mat-card *ngFor="let category of categories" class="category-card">
              <mat-card-header>
                <mat-card-title>{{ category.name }}</mat-card-title>
                <mat-card-subtitle>{{ category.description }}</mat-card-subtitle>
              </mat-card-header>
              <mat-card-content>
                <p>{{ category.consultationDescription }}</p>
                <div class="category-features">
                  <mat-chip *ngFor="let feature of category.features" color="primary" selected>
                    {{ feature }}
                  </mat-chip>
                </div>
                <div class="category-status">
                  <mat-chip [color]="category.isActive ? 'accent' : 'warn'" selected>
                    {{ category.isActive ? 'Active' : 'Inactive' }}
                  </mat-chip>
                </div>
              </mat-card-content>
              <mat-card-actions>
                <button mat-button color="primary">
                  <mat-icon>edit</mat-icon>
                  Edit
                </button>
                <button mat-button color="warn">
                  <mat-icon>delete</mat-icon>
                  Delete
                </button>
              </mat-card-actions>
            </mat-card>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .categories-container {
      padding: 20px;
    }
    .categories-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 20px;
      margin-top: 20px;
    }
    .category-card {
      transition: transform 0.2s;
    }
    .category-card:hover {
      transform: translateY(-2px);
    }
    .category-features {
      margin: 10px 0;
    }
    .category-status {
      margin-top: 10px;
    }
  `]
})
export class CategoriesComponent implements OnInit {
  categories: Category[] = [];

  constructor(private categoryService: CategoryService) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.categoryService.getCategories().subscribe({
      next: (response) => {
        this.categories = response.data || [];
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }
} 