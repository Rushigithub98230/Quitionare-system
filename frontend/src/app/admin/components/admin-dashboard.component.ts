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

import { Category } from '../../core/models/category.model';
import { Questionnaire } from '../../core/models/questionnaire.model';
import { CategoryService } from '../services/category.service';
import { QuestionnaireService } from '../services/questionnaire.service';
import { AuthService } from '../../core/services/auth.service';
import { CategoryDialogComponent, CategoryDialogData } from './category-dialog/category-dialog.component';
import { QuestionnaireDialogComponent, QuestionnaireDialogData } from './questionnaire-dialog/questionnaire-dialog.component';
import { QuestionListComponent } from './question-list/question-list.component';
import { OrderManagerComponent } from '../../shared/components/order-manager/order-manager.component';
import { OrderService, OrderItem } from '../../core/services/order.service';
import { QuestionnairePreviewComponent } from '../../questionnaire/components/questionnaire-preview/questionnaire-preview.component';
import { PreviewResultsComponent } from '../../questionnaire/components/preview-results/preview-results.component';
import { ResponseService } from 'src/app/core/services/response.service';


@Component({
  selector: 'app-admin-dashboard',
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
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent implements OnInit {
  categories: Category[] = [];
  questionnaires: Questionnaire[] = [];
  deletedCategories: Category[] = [];
  selectedCategory: Category | null = null;
  loading = false;
  loadingDeleted = false;
  error = '';
  errorDeleted = '';
  showCategoryOrderManager = false;
  categoryOrderItems: OrderItem[] = [];
  showQuestionnaireOrderManager = false;
  questionnaireOrderItems: OrderItem[] = [];

  // Response management properties
  loadingResponses = false;
  errorResponses = '';
  allResponses: any[] = [];
  filteredResponses: any[] = [];
  selectedQuestionnaireFilter = '';
  selectedDateFilter = '';

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
    private responseService: ResponseService
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadQuestionnaires();
  }

  onTabChange(event: any): void {
    // Load deleted categories when the "Deleted Categories" tab is selected
    if (event.index === 2) { // Index 2 is the "Deleted Categories" tab
      this.loadDeletedCategories();
    }
  }

  loadDeletedCategories(): void {
    this.loadingDeleted = true;
    this.errorDeleted = '';

    this.categoryService.getDeletedCategories().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.deletedCategories = response.data;
        } else {
          this.errorDeleted = response.message || 'Failed to load deleted categories';
        }
        this.loadingDeleted = false;
      },
      error: (error) => {
        this.errorDeleted = 'Error loading deleted categories';
        this.loadingDeleted = false;
        console.error('Error loading deleted categories:', error);
      }
    });
  }

  restoreCategory(category: Category): void {
    if (confirm(`Are you sure you want to restore "${category.name}" and all its questionnaires?`)) {
      this.categoryService.restoreCategory(category.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.snackBar.open('Category restored successfully', 'Close', { duration: 3000 });
            this.loadDeletedCategories(); // Refresh deleted categories list
            this.loadCategories(); // Refresh active categories list
          } else {
            this.snackBar.open(response.message || 'Failed to restore category', 'Close', { duration: 3000 });
          }
        },
        error: (error) => {
          this.snackBar.open('Error restoring category', 'Close', { duration: 3000 });
          console.error('Error restoring category:', error);
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
      next: (categories) => {
        this.categories = categories;
        this.categoriesDataSource = categories;
        this.loading = false;
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
      next: (questionnaires) => {
        this.questionnaires = questionnaires;
        this.questionnairesDataSource = questionnaires;
        this.loading = false;
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
      next: (questionnaires) => {
        this.questionnaires = questionnaires;
        this.questionnairesDataSource = questionnaires;
        this.loading = false;
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
          next: (category) => {
            this.categories.push(category);
            this.snackBar.open('Category created successfully!', 'Close', { duration: 3000 });
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
          next: (updatedCategory) => {
            const index = this.categories.findIndex(c => c.id === category.id);
            if (index !== -1) {
              this.categories[index] = updatedCategory;
            }
            this.snackBar.open('Category updated successfully!', 'Close', { duration: 3000 });
          },
          error: (error) => {
            this.snackBar.open('Error updating category: ' + error.message, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  deleteCategory(category: Category): void {
    if (confirm(`Are you sure you want to delete the category "${category.name}"?`)) {
      this.categoryService.delete(category.id).subscribe({
        next: () => {
          this.categories = this.categories.filter(c => c.id !== category.id);
          this.snackBar.open('Category deleted successfully!', 'Close', { duration: 3000 });
        },
        error: (error) => {
          this.snackBar.open('Error deleting category: ' + error.message, 'Close', { duration: 5000 });
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
          next: (questionnaire) => {
            this.questionnaires.push(questionnaire);
            this.snackBar.open('Questionnaire created successfully!', 'Close', { duration: 3000 });
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
          next: (updatedQuestionnaire) => {
            const index = this.questionnaires.findIndex(q => q.id === questionnaire.id);
            if (index !== -1) {
              this.questionnaires[index] = updatedQuestionnaire;
            }
            this.snackBar.open('Questionnaire updated successfully!', 'Close', { duration: 3000 });
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
        next: () => {
          this.questionnaires = this.questionnaires.filter(q => q.id !== questionnaire.id);
          this.snackBar.open('Questionnaire deleted successfully!', 'Close', { duration: 3000 });
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
      next: (updatedCategories) => {
        this.categories = updatedCategories;
        this.snackBar.open('Category order updated successfully!', 'Close', { duration: 3000 });
        this.showCategoryOrderManager = false;
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
      next: (updatedQuestionnaires: Questionnaire[]) => {
        this.questionnaires = updatedQuestionnaires;
        this.snackBar.open('Questionnaire order updated successfully!', 'Close', { duration: 3000 });
        this.showQuestionnaireOrderManager = false;
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

    // If a questionnaire is selected, get responses for that questionnaire
    // Otherwise, get all responses
    const observable = this.selectedQuestionnaireFilter 
      ? this.responseService.getResponsesByQuestionnaire(this.selectedQuestionnaireFilter)
      : this.responseService.getAllResponses();

    observable.subscribe({
      next: (response: any) => {
        if (response.success && response.data) {
          this.allResponses = response.data;
          this.filteredResponses = [...this.allResponses];
        } else {
          this.errorResponses = response.message || 'Failed to load responses';
        }
        this.loadingResponses = false;
      },
      error: (error: any) => {
        this.errorResponses = 'Error loading responses: ' + error.message;
        this.loadingResponses = false;
        console.error('Error loading responses:', error);
      }
    });
  }

  applyFilters(): void {
    this.filteredResponses = this.allResponses.filter(response => {
      let matches = true;

      // Filter by questionnaire
      if (this.selectedQuestionnaireFilter && response.questionnaireId !== this.selectedQuestionnaireFilter) {
        matches = false;
      }

      // Filter by date
      if (this.selectedDateFilter) {
        const responseDate = new Date(response.submittedAt);
        const now = new Date();
        
        switch (this.selectedDateFilter) {
          case 'today':
            matches = matches && responseDate.toDateString() === now.toDateString();
            break;
          case 'week':
            const weekAgo = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);
            matches = matches && responseDate >= weekAgo;
            break;
          case 'month':
            const monthAgo = new Date(now.getTime() - 30 * 24 * 60 * 60 * 1000);
            matches = matches && responseDate >= monthAgo;
            break;
        }
      }

      return matches;
    });
  }

  getQuestionnaireName(questionnaireId: string): string {
    const questionnaire = this.questionnaires.find(q => q.id === questionnaireId);
    return questionnaire ? questionnaire.title : 'Unknown Questionnaire';
  }

  viewResponseDetails(response: any): void {
    // Get detailed response data
    this.responseService.getResponseById(response.id).subscribe({
      next: (responseDetail: any) => {
        if (responseDetail.success && responseDetail.data) {
          // Transform the response data to match PreviewResponse interface
          const transformedResponses = responseDetail.data.questionResponses.map((qr: any) => ({
            questionId: qr.questionId,
            questionText: qr.questionText,
            questionType: qr.questionType || 'Unknown',
            response: this.getResponseValue(qr),
            isRequired: true, // We'll need to get this from the question data
            isValid: this.isResponseValid(qr),
            validationMessage: this.getValidationMessage(qr)
          }));

          // Show response details in a dialog
          this.dialog.open(PreviewResultsComponent, {
            width: '90vw',
            maxWidth: '1000px',
            height: '90vh',
            data: { 
              responses: transformedResponses,
              questionnaire: responseDetail.data.questionnaire,
              isSavedResponse: true,
              responseMetadata: {
                userId: responseDetail.data.userId,
                submittedAt: responseDetail.data.completedAt,
                timeTaken: responseDetail.data.timeTaken,
                isCompleted: responseDetail.data.isCompleted
              }
            }
          });
        } else {
          this.snackBar.open('Error loading response details', 'Close', { duration: 3000 });
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
} 