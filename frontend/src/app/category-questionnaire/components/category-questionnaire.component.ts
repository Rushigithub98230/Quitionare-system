import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { MatBadgeModule } from '@angular/material/badge';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDividerModule } from '@angular/material/divider';

import { Category } from '../models/category.model';
import { Questionnaire } from '../models/questionnaire.model';
import { CategoryService } from '../services/category.service';
import { QuestionnaireService } from '../services/questionnaire.service';
import { AuthService } from '../services/auth.service';
import { CategoryDialogComponent, CategoryDialogData } from './category-dialog/category-dialog.component';
import { QuestionnaireDialogComponent, QuestionnaireDialogData } from './questionnaire-dialog/questionnaire-dialog.component';
import { QuestionListComponent } from './question-list/question-list.component';
import { OrderManagerComponent } from './order-manager/order-manager.component';
import { OrderService, OrderItem } from '../services/order.service';
import { QuestionnairePreviewComponent } from './questionnaire-preview/questionnaire-preview.component';
import { PreviewResultsComponent } from './preview-results/preview-results.component';
import { ResponseService } from '../services/response.service';
import { AnalyticsService, AnalyticsSummary, AnalyticsTrends, CategoryAnalytics, QuestionnaireAnalytics } from '../services/analytics.service';
import { ResponseDetailDialogComponent, ResponseDetailData } from './response-detail-dialog/response-detail-dialog.component';


@Component({
  selector: 'app-category-questionnaire',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatTabsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatChipsModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatMenuModule,
    MatBadgeModule,
    MatExpansionModule,
    MatDividerModule,
    QuestionListComponent,
    OrderManagerComponent
  ],
  templateUrl: './category-questionnaire.component.html',
  styleUrls: ['./category-questionnaire.component.scss']
})
export class CategoryQuestionnaireComponent implements OnInit {
  categories: Category[] = [];
  questionnaires: Questionnaire[] = [];
  deletedCategories: Category[] = [];
  deactivatedCategories: Category[] = [];
  selectedCategory: Category | null = null;
  loading = false;
  loadingDeleted = false;
  loadingDeactivated = false;
  error = '';
  errorDeleted = '';
  errorDeactivated = '';
  showCategoryOrderManager = false;
  categoryOrderItems: OrderItem[] = [];
  showQuestionnaireOrderManager = false;
  questionnaireOrderItems: OrderItem[] = [];

  // Response management properties
  loadingResponses = false;
  errorResponses = '';
  allResponses: any[] = [];
  filteredResponses: any[] = [];
  totalResponsesCount: number = 0; // Track total count for display
  selectedQuestionnaireFilter = '';
  selectedCategoryFilter = '';
  selectedDateFilter = '';
  selectedCompletionFilter = '';
  responseStatistics: any = {}; // New property for response statistics

  // Backend Analytics properties
  loadingAnalytics = false;
  errorAnalytics = '';
  analyticsSummary: AnalyticsSummary | null = null;
  analyticsTrends: AnalyticsTrends | null = null;
  categoryAnalytics: CategoryAnalytics[] = [];
  questionnaireAnalytics: QuestionnaireAnalytics[] = [];

  // Table data sources
  categoriesDataSource: Category[] = [];
  questionnairesDataSource: Questionnaire[] = [];

  // Pagination
  categoriesPageSize = 10;
  questionnairesPageSize = 10;
  categoriesPageIndex = 0;
  questionnairesPageIndex = 0;

  constructor(
    private categoryService: CategoryService,
    private questionnaireService: QuestionnaireService,
    private authService: AuthService,
    private orderService: OrderService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private responseService: ResponseService,
    private analyticsService: AnalyticsService
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadQuestionnaires();
    this.loadAllResponses();
    this.loadAnalytics();
  }

  onTabChange(event: any): void {
    if (event.index === 1) { // Deleted categories tab
      this.loadDeletedCategories();
    } else if (event.index === 2) { // Deactivated categories tab
      this.loadDeactivatedCategories();
    }
  }

  loadDeletedCategories(): void {
    this.loadingDeleted = true;
    this.errorDeleted = '';
    
    this.categoryService.getDeletedCategories().subscribe({
      next: (response: any) => {
        if (response.statusCode === 200 && response.data) {
          this.deletedCategories = response.data;
        } else {
          this.errorDeleted = response.message || 'Failed to load deleted categories';
        }
        this.loadingDeleted = false;
      },
      error: (error: any) => {
        this.errorDeleted = 'Error loading deleted categories';
        this.loadingDeleted = false;
        console.error('Error loading deleted categories:', error);
      }
    });
  }

  loadDeactivatedCategories(): void {
    this.loadingDeactivated = true;
    this.errorDeactivated = '';
    
    this.categoryService.getDeactivated().subscribe({
      next: (response) => {
        if (response.statusCode === 200 && response.data) {
          this.deactivatedCategories = response.data;
        } else {
          this.errorDeactivated = response.message || 'Failed to load deactivated categories';
        }
        this.loadingDeactivated = false;
      },
      error: (error) => {
        this.errorDeactivated = 'Error loading deactivated categories';
        this.loadingDeactivated = false;
        console.error('Error loading deactivated categories:', error);
      }
    });
  }

  restoreCategory(category: Category): void {
    if (confirm(`Are you sure you want to restore the category "${category.name}"?`)) {
      this.categoryService.restoreCategory(category.id).subscribe({
        next: (response) => {
          if (response.statusCode === 200) {
            // Remove from deleted categories
            this.deletedCategories = this.deletedCategories.filter(c => c.id !== category.id);
            // Add back to active categories
            this.categories.push(category);
            this.snackBar.open('Category restored successfully!', 'Close', { duration: 3000 });
          } else {
            this.snackBar.open('Error restoring category: ' + response.message, 'Close', { duration: 5000 });
          }
        },
        error: (error) => {
          this.snackBar.open('Error restoring category: ' + error.message, 'Close', { duration: 5000 });
        }
      });
    }
  }

  viewCategoryDetails(category: Category): void {
    // Show category details in a dialog
    this.dialog.open(CategoryDialogComponent, {
      width: '600px',
      data: { category, mode: 'edit', isReadOnly: true } as CategoryDialogData
    });
  }

  loadCategories(): void {
    this.loading = true;
    this.error = '';

    this.categoryService.getAll().subscribe({
      next: (response) => {
        if (response.statusCode === 200 && response.data) {
          this.categories = response.data;
          this.categoriesDataSource = response.data;
          this.loading = false;
        } else {
          this.error = response.message || 'Failed to load categories';
          this.loading = false;
        }
      },
      error: (error) => {
        this.error = 'Failed to load categories: ' + error.message;
        this.loading = false;
      }
    });
  }

  loadQuestionnaires(): void {
    this.loading = true;
    this.error = '';

    this.questionnaireService.getAll().subscribe({
      next: (response) => {
        if (response.statusCode === 200 && response.data) {
          this.questionnaires = response.data;
          this.questionnairesDataSource = response.data;
          this.loading = false;
        } else {
          this.error = response.message || 'Failed to load questionnaires';
          this.loading = false;
        }
      },
      error: (error) => {
        this.error = 'Failed to load questionnaires: ' + error.message;
        this.loading = false;
      }
    });
  }

  onCategorySelect(category: Category): void {
    this.selectedCategory = category;
    this.loadQuestionnairesByCategory(category.id);
  }

  loadQuestionnairesByCategory(categoryId: string): void {
    this.loading = true;
    this.error = '';

    this.questionnaireService.getByCategoryId(categoryId).subscribe({
      next: (response) => {
        if (response.statusCode === 200 && response.data) {
          this.questionnaires = response.data;
          this.questionnairesDataSource = response.data;
          this.loading = false;
        } else {
          this.error = response.message || 'Failed to load questionnaires for category';
          this.loading = false;
        }
      },
      error: (error) => {
        this.error = 'Failed to load questionnaires for category: ' + error.message;
        this.loading = false;
      }
    });
  }

  createCategory(): void {
    const dialogRef = this.dialog.open(CategoryDialogComponent, {
      width: '800px',
      data: { mode: 'create' } as CategoryDialogData
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.action === 'create') {
        this.categoryService.create(result.data).subscribe({
          next: (response) => {
            if ((response.statusCode === 200 || response.statusCode === 201) && response.data) {
              this.categories.push(response.data);
              this.snackBar.open('Category created successfully!', 'Close', { duration: 3000 });
            } else {
              this.snackBar.open('Error creating category: ' + response.message, 'Close', { duration: 5000 });
            }
          },
          error: (error) => {
            this.snackBar.open('Error creating category: ' + error.message, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  editCategory(category: Category): void {
    const dialogRef = this.dialog.open(CategoryDialogComponent, {
      width: '800px',
      data: { mode: 'edit', category } as CategoryDialogData
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.action === 'update') {
        this.categoryService.update(category.id, result.data).subscribe({
          next: (response) => {
            if (response.statusCode === 200 && response.data) {
              const index = this.categories.findIndex(c => c.id === category.id);
              if (index !== -1) {
                this.categories[index] = response.data;
              }
              this.snackBar.open('Category updated successfully!', 'Close', { duration: 3000 });
            } else {
              this.snackBar.open('Error updating category: ' + response.message, 'Close', { duration: 5000 });
            }
          },
          error: (error) => {
            this.snackBar.open('Error updating category: ' + error.message, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  deleteCategory(category: Category): void {
    if (confirm(`Are you sure you want to delete the category "${category.name}"? This action cannot be undone.`)) {
      this.categoryService.delete(category.id).subscribe({
        next: (response) => {
          if (response.statusCode === 200) {
            this.categories = this.categories.filter(c => c.id !== category.id);
            this.snackBar.open('Category deleted successfully', 'Close', { duration: 3000 });
          } else {
            this.snackBar.open('Error deleting category: ' + response.message, 'Close', { duration: 5000 });
          }
        },
        error: (error) => {
          this.snackBar.open('Error deleting category: ' + error.message, 'Close', { duration: 5000 });
        }
      });
    }
  }

  deactivateCategory(category: Category): void {
    if (confirm(`Are you sure you want to deactivate the category "${category.name}"? This will also deactivate all associated questionnaires.`)) {
      this.categoryService.deactivate(category.id).subscribe({
        next: (response) => {
          if (response.statusCode === 200) {
            this.categories = this.categories.filter(c => c.id !== category.id);
            this.snackBar.open('Category deactivated successfully', 'Close', { duration: 3000 });
            // Reload deactivated categories
            this.loadDeactivatedCategories();
          } else {
            this.snackBar.open('Error deactivating category: ' + response.message, 'Close', { duration: 5000 });
          }
        },
        error: (error) => {
          this.snackBar.open('Error deactivating category: ' + error.message, 'Close', { duration: 5000 });
        }
      });
    }
  }

  reactivateCategory(category: Category): void {
    if (confirm(`Are you sure you want to reactivate the category "${category.name}"? This will also reactivate all associated questionnaires.`)) {
      this.categoryService.reactivate(category.id).subscribe({
        next: (response) => {
          if (response.statusCode === 200) {
            this.deactivatedCategories = this.deactivatedCategories.filter(c => c.id !== category.id);
            this.snackBar.open('Category reactivated successfully', 'Close', { duration: 3000 });
            // Reload active categories
            this.loadCategories();
          } else {
            this.snackBar.open('Error reactivating category: ' + response.message, 'Close', { duration: 5000 });
          }
        },
        error: (error) => {
          this.snackBar.open('Error reactivating category: ' + error.message, 'Close', { duration: 5000 });
        }
      });
    }
  }

  createQuestionnaire(): void {
    const dialogRef = this.dialog.open(QuestionnaireDialogComponent, {
      width: '600px',
      data: { mode: 'create', categories: this.categories } as QuestionnaireDialogData
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.action === 'create') {
        this.questionnaireService.create(result.data).subscribe({
          next: (response) => {
            if ((response.statusCode === 200 || response.statusCode === 201) && response.data) {
              this.questionnaires.push(response.data);
              this.snackBar.open('Questionnaire created successfully!', 'Close', { duration: 3000 });
            } else {
              this.snackBar.open('Error creating questionnaire: ' + response.message, 'Close', { duration: 5000 });
            }
          },
          error: (error) => {
            this.snackBar.open('Error creating questionnaire: ' + error.message, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  editQuestionnaire(questionnaire: Questionnaire): void {
    const dialogRef = this.dialog.open(QuestionnaireDialogComponent, {
      width: '600px',
      data: { mode: 'edit', questionnaire, categories: this.categories } as QuestionnaireDialogData
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.action === 'update') {
        this.questionnaireService.update(questionnaire.id, result.data).subscribe({
          next: (response) => {
            if (response.statusCode === 200 && response.data) {
              const index = this.questionnaires.findIndex(q => q.id === questionnaire.id);
              if (index !== -1) {
                this.questionnaires[index] = response.data;
              }
              this.snackBar.open('Questionnaire updated successfully!', 'Close', { duration: 3000 });
            } else {
              this.snackBar.open('Error updating questionnaire: ' + response.message, 'Close', { duration: 5000 });
            }
          },
          error: (error) => {
            this.snackBar.open('Error updating questionnaire: ' + error.message, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  deleteQuestionnaire(questionnaire: Questionnaire): void {
    if (confirm(`Are you sure you want to delete the questionnaire "${questionnaire.title}"?`)) {
      this.questionnaireService.delete(questionnaire.id).subscribe({
        next: (response) => {
          if (response.statusCode === 200) {
            this.questionnaires = this.questionnaires.filter(q => q.id !== questionnaire.id);
            this.snackBar.open('Questionnaire deleted successfully!', 'Close', { duration: 3000 });
          } else {
            this.snackBar.open('Error deleting questionnaire: ' + response.message, 'Close', { duration: 5000 });
          }
        },
        error: (error) => {
          this.snackBar.open('Error deleting questionnaire: ' + error.message, 'Close', { duration: 5000 });
        }
      });
    }
  }

  manageQuestions(questionnaire: Questionnaire): void {
    const dialogRef = this.dialog.open(QuestionListComponent, {
      width: '90vw',
      maxWidth: '1200px',
      height: '90vh',
      data: { questionnaire: questionnaire }
    });
  }

  previewQuestionnaire(questionnaire: Questionnaire): void {
    const dialogRef = this.dialog.open(QuestionnairePreviewComponent, {
      width: '90vw',
      maxWidth: '1000px',
      height: '90vh',
      data: { questionnaire: questionnaire }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.action === 'preview-complete') {
        // Show preview results
        this.showPreviewResults(result.responses, result.questionnaire);
      }
    });
  }

  showPreviewResults(responses: any[], questionnaire: Questionnaire): void {
    const dialogRef = this.dialog.open(PreviewResultsComponent, {
      width: '90vw',
      maxWidth: '1000px',
      height: '90vh',
      data: { 
        responses: responses,
        questionnaire: questionnaire
      }
    });
  }

  solveAsAdmin(questionnaire: Questionnaire): void {
    const dialogRef = this.dialog.open(QuestionnairePreviewComponent, {
      width: '90vw',
      maxWidth: '1000px',
      height: '90vh',
      data: { 
        questionnaire: questionnaire,
        isAdminSolve: true // Flag to indicate this is admin solving
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.action === 'preview-complete') {
        // Show preview results
        this.showPreviewResults(result.responses, result.questionnaire);
      }
    });
  }

  getCategoryQuestionnaireCount(categoryId: string): number {
    return this.getQuestionnairesByCategory(categoryId).length;
  }

  getQuestionnairesByCategory(categoryId: string): Questionnaire[] {
    return this.questionnaires.filter(q => q.categoryId === categoryId);
  }

  getCategoryName(categoryId: string): string {
    const category = this.categories.find(c => c.id === categoryId);
    return category ? category.name : 'Unknown Category';
  }

  getQuestionnaireCategoryId(questionnaireId: string): string {
    const questionnaire = this.questionnaires.find(q => q.id === questionnaireId);
    return questionnaire ? questionnaire.categoryId : '';
  }

  getQuestionnaireQuestionCount(questionnaireId: string): number {
    // This would need to be implemented based on your data structure
    // For now, returning 0 as placeholder
    return 0;
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  // Category Order Management
  toggleCategoryOrderManager(): void {
    this.showCategoryOrderManager = !this.showCategoryOrderManager;
    if (this.showCategoryOrderManager) {
      this.loadCategoryOrderItems();
    }
  }

  loadCategoryOrderItems(): void {
    this.categoryOrderItems = this.categories.map(category => ({
      id: category.id,
      displayOrder: category.displayOrder,
      name: category.name,
      type: 'category' as const
    }));
  }

  onCategoryOrderChanged(orderItems: OrderItem[]): void {
    this.categoryOrderItems = orderItems;
    
    // Update the categories array with new order
    this.categories = this.categories.map(category => {
      const orderItem = orderItems.find(item => item.id === category.id);
      if (orderItem) {
        return { ...category, displayOrder: orderItem.displayOrder };
      }
      return category;
    });

    // Sort categories by display order
    this.categories.sort((a, b) => a.displayOrder - b.displayOrder);
  }

  saveCategoryOrder(): void {
    this.categoryService.updateOrder(this.categories).subscribe({
      next: (response) => {
        if (response.statusCode === 200 && response.data) {
          this.categories = response.data;
          this.snackBar.open('Category order updated successfully!', 'Close', { duration: 3000 });
          this.showCategoryOrderManager = false;
        } else {
          this.snackBar.open('Error updating category order: ' + response.message, 'Close', { duration: 5000 });
        }
      },
      error: (error) => {
        this.snackBar.open('Error updating category order: ' + error.message, 'Close', { duration: 5000 });
      }
    });
  }

  // Questionnaire Order Management
  toggleQuestionnaireOrderManager(): void {
    this.showQuestionnaireOrderManager = !this.showQuestionnaireOrderManager;
    if (this.showQuestionnaireOrderManager) {
      this.loadQuestionnaireOrderItems();
    }
  }

  loadQuestionnaireOrderItems(): void {
    this.questionnaireOrderItems = this.questionnaires.map(questionnaire => ({
      id: questionnaire.id,
      displayOrder: questionnaire.displayOrder,
      name: questionnaire.title,
      type: 'questionnaire' as const
    }));
  }

  onQuestionnaireOrderChanged(orderItems: OrderItem[]): void {
    this.questionnaireOrderItems = orderItems;
    
    // Update the questionnaires array with new order
    this.questionnaires = this.questionnaires.map(questionnaire => {
      const orderItem = orderItems.find(item => item.id === questionnaire.id);
      if (orderItem) {
        return { ...questionnaire, displayOrder: orderItem.displayOrder };
      }
      return questionnaire;
    });

    // Sort questionnaires by display order
    this.questionnaires.sort((a, b) => a.displayOrder - b.displayOrder);
  }

  saveQuestionnaireOrder(): void {
    this.questionnaireService.updateOrder(this.questionnaires).subscribe({
      next: (response) => {
        if (response.statusCode === 200 && response.data) {
          this.questionnaires = response.data;
          this.snackBar.open('Questionnaire order updated successfully!', 'Close', { duration: 3000 });
          this.showQuestionnaireOrderManager = false;
        } else {
          this.snackBar.open('Error updating questionnaire order: ' + response.message, 'Close', { duration: 5000 });
        }
      },
      error: (error: any) => {
        this.snackBar.open('Error updating questionnaire order: ' + error.message, 'Close', { duration: 5000 });
      }
    });
  }

  // Response Management Methods
  loadAllResponses(): void {
    this.loadingResponses = true;
    this.errorResponses = '';

    // Use enhanced getAllResponses method with no filters for initial load
    this.responseService.getAllResponses().subscribe({
      next: (response) => {
        if (response.statusCode === 200 && response.data) {
          this.filteredResponses = response.data;
          this.totalResponsesCount = response.data.length; // Set total count
          console.log(`Loaded ${this.filteredResponses.length} responses from backend`);
        } else {
          this.errorResponses = response.message || 'Failed to load responses';
          this.filteredResponses = [];
          this.totalResponsesCount = 0;
        }
        this.loadingResponses = false;
      },
      error: (error: any) => {
        this.errorResponses = 'Error loading responses: ' + error.message;
        this.loadingResponses = false;
        this.filteredResponses = [];
        this.totalResponsesCount = 0;
      }
    });
  }

  // Load responses with backend filtering
  loadFilteredResponses(): void {
    this.loadingResponses = true;
    this.errorResponses = '';

    const filters: any = {};
    
    if (this.selectedQuestionnaireFilter) {
      filters.questionnaireId = this.selectedQuestionnaireFilter;
    }
    if (this.selectedCategoryFilter) {
      filters.categoryId = this.selectedCategoryFilter;
    }
    if (this.selectedDateFilter) {
      filters.dateFilter = this.selectedDateFilter;
    }
    if (this.selectedCompletionFilter) {
      filters.completionStatus = this.selectedCompletionFilter;
    }

    // Use the enhanced getAllResponses method with filters
    this.responseService.getAllResponses(filters).subscribe({
      next: (response) => {
        if (response.statusCode === 200 && response.data) {
          this.filteredResponses = response.data;
          this.totalResponsesCount = response.data.length; // Set total count
          console.log(`Loaded ${this.filteredResponses.length} filtered responses from backend`);
        } else {
          this.errorResponses = response.message || 'Failed to load responses';
          this.filteredResponses = [];
          this.totalResponsesCount = 0;
        }
        this.loadingResponses = false;
      },
      error: (error: any) => {
        this.errorResponses = 'Error loading responses: ' + error.message;
        this.loadingResponses = false;
        this.filteredResponses = [];
        this.totalResponsesCount = 0;
      }
    });
  }

  // Load response statistics from backend
  loadResponseStatistics(): void {
    const filters: any = {};
    
    if (this.selectedQuestionnaireFilter) {
      filters.questionnaireId = this.selectedQuestionnaireFilter;
    }
    if (this.selectedCategoryFilter) {
      filters.categoryId = this.selectedCategoryFilter;
    }
    if (this.selectedDateFilter) {
      filters.dateFilter = this.selectedDateFilter;
    }

    this.responseService.getResponseStatistics(filters).subscribe({
      next: (response) => {
        if (response.statusCode === 200 && response.data) {
          // Update statistics from backend
          this.responseStatistics = response.data;
        }
      },
      error: (error: any) => {
        console.error('Error loading response statistics:', error);
      }
    });
  }

  loadAnalytics(): void {
    this.loadingAnalytics = true;
    this.errorAnalytics = '';

    this.analyticsService.getAnalyticsSummary().subscribe({
      next: (response) => {
        if (response.statusCode === 200 && response.data) {
          this.analyticsSummary = response.data;
          this.analyticsTrends = response.data.trends;
          this.categoryAnalytics = response.data.topCategories;
          this.questionnaireAnalytics = response.data.topQuestionnaires;
          console.log('Analytics loaded successfully:', response.data);
        } else {
          this.errorAnalytics = response.message || 'Failed to load analytics';
          console.error('Analytics error:', response.message);
        }
        this.loadingAnalytics = false;
      },
      error: (error: any) => {
        this.errorAnalytics = 'Error loading analytics: ' + error.message;
        this.loadingAnalytics = false;
      }
    });
  }

  applyFilters(): void {
    // This method is deprecated - using backend filtering instead
    console.warn('applyFilters() is deprecated. Using backend filtering.');
  }

  getQuestionnaireName(questionnaireId: string): string {
    const questionnaire = this.questionnaires.find(q => q.id === questionnaireId);
    return questionnaire ? questionnaire.title : 'Unknown Questionnaire';
  }

  viewResponseDetails(response: any): void {
    // Get the questionnaire details
    const questionnaire = this.questionnaires.find(q => q.id === response.questionnaireId);
    
    // Get detailed response data
    this.responseService.getResponseById(response.id).subscribe({
      next: (result: any) => {
        if (result.success && result.data) {
          const detailedResponse = result.data;
          
          // Show response details in the new dialog
          const dialogRef = this.dialog.open(ResponseDetailDialogComponent, {
            width: '900px',
            maxHeight: '90vh',
            data: {
              response: detailedResponse,
              questionnaire: questionnaire
            } as ResponseDetailData
          });
          
          dialogRef.afterClosed().subscribe(result => {
            if (result === 'export') {
              this.exportResponse(response);
            }
          });
        } else {
          this.snackBar.open('Error loading response details: ' + (result.message || 'Unknown error'), 'Close', { duration: 3000 });
        }
      },
      error: (error: any) => {
        this.snackBar.open('Error loading response details: ' + error.message, 'Close', { duration: 3000 });
      }
    });
  }

  private getResponseValue(questionResponse: any): any {
    // Return the appropriate response value based on the question type
    if (questionResponse.textResponse) {
      return questionResponse.textResponse;
    }
    if (questionResponse.numberResponse !== null && questionResponse.numberResponse !== undefined) {
      return questionResponse.numberResponse;
    }
    if (questionResponse.dateResponse) {
      return questionResponse.dateResponse;
    }
    if (questionResponse.booleanResponse !== null && questionResponse.booleanResponse !== undefined) {
      return questionResponse.booleanResponse;
    }
    if (questionResponse.optionResponses && questionResponse.optionResponses.length > 0) {
      return questionResponse.optionResponses.map((opt: any) => opt.optionText).join(', ');
    }
    
    // Handle enhanced response format with displayText
    if (questionResponse.response && typeof questionResponse.response === 'object') {
      if (questionResponse.response.displayText) {
        return questionResponse.response.displayText;
      }
      if (questionResponse.response.text) {
        return questionResponse.response.text;
      }
    }
    
    return null;
  }

  private isResponseValid(questionResponse: any): boolean {
    // Check if the response has a valid value
    const hasValue = this.getResponseValue(questionResponse) !== null;
    return hasValue;
  }

  private getValidationMessage(questionResponse: any): string | undefined {
    // Return validation message if response is invalid
    if (!this.isResponseValid(questionResponse)) {
      return 'No response provided';
    }
    return undefined;
  }

  exportResponse(response: any): void {
    // Export individual response
    this.responseService.exportResponsesToCSVWithData(response.questionnaireId).subscribe({
      next: (result: any) => {
        if (result.success && result.data) {
          // Create and download CSV file
          const csvContent = result.data.csvContent;
          const blob = new Blob([csvContent], { type: 'text/csv' });
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.download = result.data.fileName || `response-${response.id}.csv`;
          link.click();
          window.URL.revokeObjectURL(url);
        } else {
          this.snackBar.open('Error exporting response: ' + (result.message || 'Unknown error'), 'Close', { duration: 3000 });
        }
      },
      error: (error: any) => {
        this.snackBar.open('Error exporting response: ' + error.message, 'Close', { duration: 3000 });
      }
    });
  }

  exportResponses(): void {
    // Export all responses for selected questionnaire
    if (!this.selectedQuestionnaireFilter) {
      this.snackBar.open('Please select a questionnaire to export', 'Close', { duration: 3000 });
      return;
    }

    this.responseService.exportResponsesToCSVWithData(this.selectedQuestionnaireFilter).subscribe({
      next: (result: any) => {
        if (result.success && result.data) {
          // Create and download CSV file
          const csvContent = result.data.csvContent;
          const blob = new Blob([csvContent], { type: 'text/csv' });
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.download = result.data.fileName || `responses-${this.selectedQuestionnaireFilter}.csv`;
          link.click();
          window.URL.revokeObjectURL(url);
        } else {
          this.snackBar.open('Error exporting responses: ' + (result.message || 'Unknown error'), 'Close', { duration: 3000 });
        }
      },
      error: (error: any) => {
        this.snackBar.open('Error exporting responses: ' + error.message, 'Close', { duration: 3000 });
      }
    });
  }

  // Analytics Methods
  getCategoryAnalytics(): any[] {
    if (this.categoryAnalytics && this.categoryAnalytics.length > 0) {
      return this.categoryAnalytics.map(category => ({
        categoryId: category.categoryId,
        categoryName: category.categoryName,
        responseCount: category.responseCount,
        questionnaireCount: category.questionnaireCount,
        totalQuestions: category.totalQuestions,
        questionnaires: category.questionnaires
      }));
    }
    
    // Fallback to empty array if backend analytics not available
    return [];
  }

  getQuestionnaireAnalytics(): any[] {
    if (this.questionnaireAnalytics && this.questionnaireAnalytics.length > 0) {
      return this.questionnaireAnalytics.map(questionnaire => ({
        questionnaireId: questionnaire.questionnaireId,
        questionnaireName: questionnaire.questionnaireName,
        responseCount: questionnaire.responseCount,
        categoryName: questionnaire.categoryName,
        totalQuestions: questionnaire.totalQuestions
      }));
    }
    
    // Fallback to empty array if backend analytics not available
    return [];
  }

  getResponseTrends(): any {
    if (this.analyticsTrends) {
      return {
        today: this.analyticsTrends.today,
        thisWeek: this.analyticsTrends.thisWeek,
        thisMonth: this.analyticsTrends.thisMonth,
        total: this.analyticsTrends.total
      };
    }
    
    // Fallback to old calculation if backend analytics not available
    return {
      today: 0,
      thisWeek: 0,
      thisMonth: 0,
      total: 0
    };
  }

  getTopPerformingCategories(): any[] {
    return this.getCategoryAnalytics().slice(0, 5);
  }

  getTopPerformingQuestionnaires(): any[] {
    return this.getQuestionnaireAnalytics().slice(0, 5);
  }

  // Enhanced Filtering Methods
  // applyEnhancedFilters(): void { ... } - REMOVED

  // Handle filter changes automatically
  onFilterChange(): void {
    // Use backend filtering instead of frontend filtering
    this.loadFilteredResponses();
    this.loadResponseStatistics();
  }

  // Response statistics methods - now using backend data with fallbacks
  getCompletedResponsesCount(): number {
    // Use backend statistics if available, otherwise calculate from filtered responses
    if (this.responseStatistics?.completedResponses !== undefined) {
      return this.responseStatistics.completedResponses;
    }
    // Fallback to frontend calculation
    return this.filteredResponses.filter(r => r.isCompleted).length;
  }

  getDraftResponsesCount(): number {
    // Use backend statistics if available, otherwise calculate from filtered responses
    if (this.responseStatistics?.draftResponses !== undefined) {
      return this.responseStatistics.draftResponses;
    }
    // Fallback to frontend calculation
    return this.filteredResponses.filter(r => !r.isCompleted).length;
  }

  getAverageCompletionPercentage(): number {
    // Use backend statistics if available, otherwise calculate from filtered responses
    if (this.responseStatistics?.averageCompletionPercentage !== undefined) {
      return this.responseStatistics.averageCompletionPercentage;
    }
    // Fallback to frontend calculation
    if (this.filteredResponses.length === 0) return 0;
    const total = this.filteredResponses.reduce((sum, r) => sum + (r.completionPercentage || 0), 0);
    return Math.round(total / this.filteredResponses.length);
  }

  getTotalResponsesCount(): number {
    // Use backend statistics if available, otherwise use filtered responses count
    if (this.responseStatistics?.totalResponses !== undefined) {
      return this.responseStatistics.totalResponses;
    }
    // Fallback to filtered responses count
    return this.filteredResponses.length;
  }

  clearFilters(): void {
    this.selectedQuestionnaireFilter = '';
    this.selectedCategoryFilter = '';
    this.selectedDateFilter = '';
    this.selectedCompletionFilter = '';
    // Reload with no filters
    this.loadFilteredResponses();
    this.loadResponseStatistics();
  }

  getFormattedDate(date: string | Date): string {
    if (!date) return 'Not specified';
    const dateObj = new Date(date);
    return dateObj.toLocaleDateString() + ' ' + dateObj.toLocaleTimeString();
  }
} 