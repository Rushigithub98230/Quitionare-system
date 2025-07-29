import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CategoryService } from '../../services/category.service';
import { QuestionnaireService } from '../../services/questionnaire.service';
import { Category } from '../../models/category.model';
import { Questionnaire } from '../../models/questionnaire.model';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatTabsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatDialogModule
  ],
  template: `
    <div class="admin-container">
      <h1>Admin Dashboard</h1>
      
      <mat-tab-group>
        <mat-tab label="Categories">
          <div class="tab-content">
            <div class="header-actions">
              <h2>Category Management</h2>
              <button mat-raised-button color="primary" (click)="createCategory()">
                <mat-icon>add</mat-icon>
                Create Category
              </button>
            </div>
            
            <mat-card>
              <mat-card-content>
                <table mat-table [dataSource]="categories" class="full-width">
                  <ng-container matColumnDef="name">
                    <th mat-header-cell *matHeaderCellDef>Name</th>
                    <td mat-cell *matCellDef="let category">{{ category.name }}</td>
                  </ng-container>
                  
                  <ng-container matColumnDef="description">
                    <th mat-header-cell *matHeaderCellDef>Description</th>
                    <td mat-cell *matCellDef="let category">{{ category.description }}</td>
                  </ng-container>
                  
                  <ng-container matColumnDef="questionnaireCount">
                    <th mat-header-cell *matHeaderCellDef>Questionnaires</th>
                    <td mat-cell *matCellDef="let category">{{ category.questionnaireCount }}</td>
                  </ng-container>
                  
                  <ng-container matColumnDef="isActive">
                    <th mat-header-cell *matHeaderCellDef>Status</th>
                    <td mat-cell *matCellDef="let category">
                      <span [class]="category.isActive ? 'status-active' : 'status-inactive'">
                        {{ category.isActive ? 'Active' : 'Inactive' }}
                      </span>
                    </td>
                  </ng-container>
                  
                  <ng-container matColumnDef="actions">
                    <th mat-header-cell *matHeaderCellDef>Actions</th>
                    <td mat-cell *matCellDef="let category">
                      <button mat-icon-button (click)="editCategory(category)" matTooltip="Edit">
                        <mat-icon>edit</mat-icon>
                      </button>
                      <button mat-icon-button (click)="deleteCategory(category.id)" matTooltip="Delete">
                        <mat-icon>delete</mat-icon>
                      </button>
                    </td>
                  </ng-container>
                  
                  <tr mat-header-row *matHeaderRowDef="categoryColumns"></tr>
                  <tr mat-row *matRowDef="let row; columns: categoryColumns;"></tr>
                </table>
              </mat-card-content>
            </mat-card>
          </div>
        </mat-tab>
        
        <mat-tab label="Questionnaires">
          <div class="tab-content">
            <div class="header-actions">
              <h2>Questionnaire Management</h2>
              <button mat-raised-button color="primary" (click)="createQuestionnaire()">
                <mat-icon>add</mat-icon>
                Create Questionnaire
              </button>
            </div>
            
            <mat-card>
              <mat-card-content>
                <table mat-table [dataSource]="questionnaires" class="full-width">
                  <ng-container matColumnDef="title">
                    <th mat-header-cell *matHeaderCellDef>Title</th>
                    <td mat-cell *matCellDef="let questionnaire">{{ questionnaire.title }}</td>
                  </ng-container>
                  
                  <ng-container matColumnDef="categoryName">
                    <th mat-header-cell *matHeaderCellDef>Category</th>
                    <td mat-cell *matCellDef="let questionnaire">{{ questionnaire.categoryName }}</td>
                  </ng-container>
                  
                  <ng-container matColumnDef="isActive">
                    <th mat-header-cell *matHeaderCellDef>Status</th>
                    <td mat-cell *matCellDef="let questionnaire">
                      <span [class]="questionnaire.isActive ? 'status-active' : 'status-inactive'">
                        {{ questionnaire.isActive ? 'Active' : 'Inactive' }}
                      </span>
                    </td>
                  </ng-container>
                  
                  <ng-container matColumnDef="version">
                    <th mat-header-cell *matHeaderCellDef>Version</th>
                    <td mat-cell *matCellDef="let questionnaire">{{ questionnaire.version }}</td>
                  </ng-container>
                  
                  <ng-container matColumnDef="actions">
                    <th mat-header-cell *matHeaderCellDef>Actions</th>
                    <td mat-cell *matCellDef="let questionnaire">
                      <button mat-icon-button (click)="editQuestionnaire(questionnaire)" matTooltip="Edit">
                        <mat-icon>edit</mat-icon>
                      </button>
                      <button mat-icon-button (click)="deleteQuestionnaire(questionnaire.id)" matTooltip="Delete">
                        <mat-icon>delete</mat-icon>
                      </button>
                    </td>
                  </ng-container>
                  
                  <tr mat-header-row *matHeaderRowDef="questionnaireColumns"></tr>
                  <tr mat-row *matRowDef="let row; columns: questionnaireColumns;"></tr>
                </table>
              </mat-card-content>
            </mat-card>
          </div>
        </mat-tab>
      </mat-tab-group>
    </div>
  `,
  styles: [`
    .admin-container {
      max-width: 1200px;
      margin: 0 auto;
    }
    
    h1 {
      margin-bottom: 24px;
      color: #333;
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
    
    .header-actions h2 {
      margin: 0;
      color: #333;
    }
    
    .full-width {
      width: 100%;
    }
    
    .status-active {
      color: #4caf50;
      font-weight: 500;
    }
    
    .status-inactive {
      color: #f44336;
      font-weight: 500;
    }
    
    mat-card {
      margin-bottom: 20px;
    }
    
    .mat-column-actions {
      width: 120px;
      text-align: center;
    }
  `]
})
export class AdminComponent implements OnInit {
  categories: Category[] = [];
  questionnaires: Questionnaire[] = [];
  
  categoryColumns = ['name', 'description', 'questionnaireCount', 'isActive', 'actions'];
  questionnaireColumns = ['title', 'categoryName', 'isActive', 'version', 'actions'];

  constructor(
    private categoryService: CategoryService,
    private questionnaireService: QuestionnaireService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadQuestionnaires();
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (categories: Category[]) => {
        this.categories = categories;
      },
      error: (error: any) => {
        this.snackBar.open('Error loading categories', 'Close', { duration: 3000 });
      }
    });
  }

  loadQuestionnaires(): void {
    this.questionnaireService.getAll().subscribe({
      next: (questionnaires: Questionnaire[]) => {
        this.questionnaires = questionnaires;
      },
      error: (error: any) => {
        this.snackBar.open('Error loading questionnaires', 'Close', { duration: 3000 });
      }
    });
  }

  get activeCategories(): number {
    return this.categories.filter(c => c.isActive).length;
  }

  get activeQuestionnaires(): number {
    return this.questionnaires.filter(q => q.isActive).length;
  }

  createCategory(): void {
    // TODO: Implement category creation dialog
    this.snackBar.open('Category creation coming soon!', 'Close', { duration: 3000 });
  }

  editCategory(category: Category): void {
    // TODO: Implement category editing dialog
    this.snackBar.open('Category editing coming soon!', 'Close', { duration: 3000 });
  }

  deleteCategory(categoryId: string): void {
    if (confirm('Are you sure you want to delete this category?')) {
      this.categoryService.delete(categoryId).subscribe({
        next: () => {
          this.snackBar.open('Category deleted successfully', 'Close', { duration: 3000 });
          this.loadCategories();
        },
        error: (error: any) => {
          this.snackBar.open('Error deleting category', 'Close', { duration: 3000 });
        }
      });
    }
  }

  createQuestionnaire(): void {
    // TODO: Implement questionnaire creation dialog
    this.snackBar.open('Questionnaire creation coming soon!', 'Close', { duration: 3000 });
  }

  editQuestionnaire(questionnaire: Questionnaire): void {
    // TODO: Implement questionnaire editing dialog
    this.snackBar.open('Questionnaire editing coming soon!', 'Close', { duration: 3000 });
  }

  deleteQuestionnaire(questionnaireId: string): void {
    if (confirm('Are you sure you want to delete this questionnaire?')) {
      this.questionnaireService.delete(questionnaireId).subscribe({
        next: () => {
          this.snackBar.open('Questionnaire deleted successfully', 'Close', { duration: 3000 });
          this.loadQuestionnaires();
        },
        error: (error: any) => {
          this.snackBar.open('Error deleting questionnaire', 'Close', { duration: 3000 });
        }
      });
    }
  }
} 