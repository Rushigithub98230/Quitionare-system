import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CategoryService } from '../../services/category.service';
import { QuestionnaireService } from '../../services/questionnaire.service';
import { Category } from '../../models/category.model';
import { QuestionnaireTemplate } from '../../models/questionnaire.model';
import { CategoryDialogComponent } from './category-dialog/category-dialog.component';
import { QuestionnaireDialogComponent } from './questionnaire-dialog/questionnaire-dialog.component';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [
    CommonModule,
    MatTabsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule
  ],
  template: `
    <div class="admin-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Admin Dashboard</mat-card-title>
          <mat-card-subtitle>Manage categories and questionnaire templates</mat-card-subtitle>
        </mat-card-header>
        
        <mat-card-content>
          <mat-tab-group>
            <mat-tab label="Categories">
              <div class="tab-content">
                <div class="header-actions">
                  <h3>Categories</h3>
                  <button mat-raised-button color="primary" (click)="openCategoryDialog()">
                    <mat-icon>add</mat-icon>
                    Add Category
                  </button>
                </div>
                
                <div class="grid-container" *ngIf="categories.length > 0; else noCategories">
                  <mat-card *ngFor="let category of categories" class="category-card">
                    <mat-card-header>
                      <mat-card-title>{{ category.name }}</mat-card-title>
                      <mat-card-subtitle>{{ category.description }}</mat-card-subtitle>
                    </mat-card-header>
                    <mat-card-content>
                      <div class="category-info">
                        <p><strong>Base Price:</strong> \${{ category.basePrice }}</p>
                        <p><strong>Status:</strong> {{ category.isActive ? 'Active' : 'Inactive' }}</p>
                        <p><strong>Order:</strong> {{ category.displayOrder }}</p>
                        <p><strong>Template:</strong> 
                          <span *ngIf="category.hasQuestionnaireTemplate" class="status-active">Has Template</span>
                          <span *ngIf="!category.hasQuestionnaireTemplate" class="status-inactive">No Template</span>
                        </p>
                      </div>
                    </mat-card-content>
                    <mat-card-actions>
                      <button mat-button color="primary" (click)="editCategory(category)">
                        <mat-icon>edit</mat-icon>
                        Edit
                      </button>
                      <button mat-button color="accent" (click)="manageTemplate(category)">
                        <mat-icon>quiz</mat-icon>
                        {{ category.hasQuestionnaireTemplate ? 'Edit Template' : 'Add Template' }}
                      </button>
                      <button mat-button color="warn" (click)="deleteCategory(category.id)">
                        <mat-icon>delete</mat-icon>
                        Delete
                      </button>
                    </mat-card-actions>
                  </mat-card>
                </div>
                
                <ng-template #noCategories>
                  <div class="empty-state">
                    <mat-icon>category</mat-icon>
                    <h3>No Categories</h3>
                    <p>Create your first category to get started</p>
                    <button mat-raised-button color="primary" (click)="openCategoryDialog()">
                      <mat-icon>add</mat-icon>
                      Add Category
                    </button>
                  </div>
                </ng-template>
              </div>
            </mat-tab>
            
            <mat-tab label="Questionnaire Templates">
              <div class="tab-content">
                <div class="header-actions">
                  <h3>Questionnaire Templates</h3>
                  <p class="info-text">Each category can have one questionnaire template. Use the Categories tab to manage templates.</p>
                </div>
                
                <div class="grid-container" *ngIf="questionnaireTemplates.length > 0; else noTemplates">
                  <mat-card *ngFor="let template of questionnaireTemplates" class="template-card">
                    <mat-card-header>
                      <mat-card-title>{{ template.title }}</mat-card-title>
                      <mat-card-subtitle>{{ template.categoryName }}</mat-card-subtitle>
                    </mat-card-header>
                    <mat-card-content>
                      <div class="template-info">
                        <p><strong>Description:</strong> {{ template.description || 'No description' }}</p>
                        <p><strong>Status:</strong> {{ template.isActive ? 'Active' : 'Inactive' }}</p>
                        <p><strong>Questions:</strong> {{ template.questionCount }}</p>
                        <p><strong>Version:</strong> {{ template.version }}</p>
                        <p><strong>Created by:</strong> {{ template.createdByUserName }}</p>
                      </div>
                    </mat-card-content>
                    <mat-card-actions>
                      <button mat-button color="primary" (click)="editTemplate(template)">
                        <mat-icon>edit</mat-icon>
                        Edit Template
                      </button>
                      <button mat-button color="warn" (click)="deleteTemplate(template.id)">
                        <mat-icon>delete</mat-icon>
                        Delete Template
                      </button>
                    </mat-card-actions>
                  </mat-card>
                </div>
                
                <ng-template #noTemplates>
                  <div class="empty-state">
                    <mat-icon>quiz</mat-icon>
                    <h3>No Templates</h3>
                    <p>Create categories first, then add questionnaire templates to them</p>
                  </div>
                </ng-template>
              </div>
            </mat-tab>
          </mat-tab-group>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .admin-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 20px;
    }
    
    .tab-content {
      padding: 20px 0;
    }
    
    .header-actions {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
    }
    
    .header-actions h3 {
      margin: 0;
    }
    
    .info-text {
      color: #666;
      font-style: italic;
      margin: 0;
    }
    
    .grid-container {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
      gap: 20px;
    }
    
    .category-card, .template-card {
      height: fit-content;
    }
    
    .category-info, .template-info {
      margin: 10px 0;
    }
    
    .category-info p, .template-info p {
      margin: 5px 0;
      font-size: 14px;
    }
    
    .status-active {
      color: #4caf50;
      font-weight: 500;
    }
    
    .status-inactive {
      color: #f44336;
      font-weight: 500;
    }
    
    .empty-state {
      text-align: center;
      padding: 40px;
      color: #666;
    }
    
    .empty-state mat-icon {
      font-size: 64px;
      width: 64px;
      height: 64px;
      color: #ddd;
      margin-bottom: 20px;
    }
    
    .empty-state h3 {
      margin: 0 0 10px 0;
      color: #333;
    }
    
    .empty-state p {
      margin: 0 0 20px 0;
    }
    
    @media (max-width: 768px) {
      .header-actions {
        flex-direction: column;
        gap: 10px;
        align-items: flex-start;
      }
      
      .grid-container {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class AdminComponent implements OnInit {
  categories: Category[] = [];
  questionnaireTemplates: QuestionnaireTemplate[] = [];
  isLoading = false;

  constructor(
    private categoryService: CategoryService,
    private questionnaireService: QuestionnaireService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadQuestionnaireTemplates();
  }

  loadCategories(): void {
    this.isLoading = true;
    this.categoryService.getCategories().subscribe({
      next: (response) => {
        if (response.success) {
          this.categories = response.data;
        }
      },
      error: (error) => {
        this.snackBar.open('Error loading categories', 'Close', { duration: 3000 });
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  loadQuestionnaireTemplates(): void {
    this.questionnaireService.getQuestionnaireTemplates().subscribe({
      next: (response) => {
        if (response.success) {
          this.questionnaireTemplates = response.data;
        }
      },
      error: (error) => {
        this.snackBar.open('Error loading questionnaire templates', 'Close', { duration: 3000 });
      }
    });
  }

  openCategoryDialog(category?: Category): void {
    const dialogRef = this.dialog.open(CategoryDialogComponent, {
      width: '800px',
      data: {
        category: category,
        isEdit: !!category
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadCategories();
        this.snackBar.open(
          category ? 'Category updated successfully!' : 'Category created successfully!',
          'Close',
          { duration: 3000 }
        );
      }
    });
  }

  editCategory(category: Category): void {
    this.openCategoryDialog(category);
  }

  deleteCategory(categoryId: string): void {
    if (confirm('Are you sure you want to delete this category?')) {
      this.categoryService.deleteCategory(categoryId).subscribe({
        next: (response) => {
          if (response.success) {
            this.loadCategories();
            this.snackBar.open('Category deleted successfully!', 'Close', { duration: 3000 });
          } else {
            this.snackBar.open(response.message || 'Failed to delete category', 'Close', { duration: 5000 });
          }
        },
        error: (error) => {
          this.snackBar.open('Error deleting category', 'Close', { duration: 5000 });
        }
      });
    }
  }

  manageTemplate(category: Category): void {
    if (category.hasQuestionnaireTemplate) {
      // Get the template for this category and edit it
      this.questionnaireService.getQuestionnaireTemplateByCategory(category.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.editTemplate(response.data);
          }
        },
        error: (error) => {
          this.snackBar.open('Error loading template', 'Close', { duration: 3000 });
        }
      });
    } else {
      // Create new template for this category
      this.openQuestionnaireDialog(category);
    }
  }

  openQuestionnaireDialog(category?: Category, template?: QuestionnaireTemplate): void {
    const dialogRef = this.dialog.open(QuestionnaireDialogComponent, {
      width: '900px',
      data: {
        questionnaire: template,
        category: category,
        isEdit: !!template
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadCategories();
        this.loadQuestionnaireTemplates();
        this.snackBar.open(
          template ? 'Template updated successfully!' : 'Template created successfully!',
          'Close',
          { duration: 3000 }
        );
      }
    });
  }

  editTemplate(template: QuestionnaireTemplate): void {
    this.openQuestionnaireDialog(undefined, template);
  }

  deleteTemplate(templateId: string): void {
    if (confirm('Are you sure you want to delete this template?')) {
      this.questionnaireService.deleteQuestionnaireTemplate(templateId).subscribe({
        next: (response) => {
          if (response.success) {
            this.loadCategories();
            this.loadQuestionnaireTemplates();
            this.snackBar.open('Template deleted successfully!', 'Close', { duration: 3000 });
          } else {
            this.snackBar.open(response.message || 'Failed to delete template', 'Close', { duration: 5000 });
          }
        },
        error: (error) => {
          this.snackBar.open('Error deleting template', 'Close', { duration: 5000 });
        }
      });
    }
  }
} 