import { Component, Input, OnInit, Inject, Optional } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';

import { Question, QuestionType } from '../../models/question.model';
import { Questionnaire } from '../../models/questionnaire.model';
import { QuestionService } from '../../services/question.service';
import { QuestionDialogComponent } from '../question-dialog/question-dialog.component';
import { OrderManagerComponent } from '../order-manager/order-manager.component';
import { OrderService, OrderItem } from '../../services/order.service';

@Component({
  selector: 'app-question-list',
  standalone: true,
  imports: [
    CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatChipsModule,
    MatExpansionModule, MatListModule, MatDividerModule, MatTooltipModule,
    OrderManagerComponent
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>
          <mat-icon>quiz</mat-icon>
          Questions for "{{ questionnaire.title || 'Loading...' }}"
        </mat-card-title>
        <mat-card-subtitle>
          {{ questions.length }} question(s) • {{ getRequiredCount() }} required
        </mat-card-subtitle>
        <button *ngIf="isDialogMode()" mat-icon-button (click)="closeDialog()" class="close-button">
          <mat-icon>close</mat-icon>
        </button>
      </mat-card-header>

      <mat-card-content>
        <div class="actions-bar">
          <button mat-raised-button color="primary" (click)="addQuestion()">
            <mat-icon>add</mat-icon>
            Add Question
          </button>
          <button mat-raised-button color="accent" (click)="toggleQuestionOrderManager()">
            <mat-icon>sort</mat-icon>
            {{ showQuestionOrderManager ? 'Hide' : 'Manage' }} Order
          </button>
          <button mat-button (click)="refreshQuestions()" [disabled]="loading">
            <mat-icon>refresh</mat-icon>
            Refresh
          </button>
        </div>

        <!-- Question Order Manager -->
        <div *ngIf="showQuestionOrderManager" class="order-manager-section">
          <app-order-manager 
            [items]="questionOrderItems"
            [title]="'Question Order Management'"
            (orderChanged)="onQuestionOrderChanged($event)">
          </app-order-manager>
          
          <div class="order-actions">
            <button mat-raised-button color="primary" (click)="saveQuestionOrder()">
              <mat-icon>save</mat-icon>
              Save Order
            </button>
            <button mat-button (click)="toggleQuestionOrderManager()">
              <mat-icon>close</mat-icon>
              Cancel
            </button>
          </div>
        </div>

        <div *ngIf="loading" class="loading">
          <mat-icon>hourglass_empty</mat-icon>
          Loading questions...
        </div>

        <div *ngIf="!loading && questions.length === 0" class="no-questions">
          <mat-icon>quiz</mat-icon>
          <p>No questions found for this questionnaire.</p>
          <button mat-raised-button color="primary" (click)="addQuestion()">
            <mat-icon>add</mat-icon>
            Add First Question
          </button>
        </div>

        <mat-accordion *ngIf="!loading && questions.length > 0">
          <mat-expansion-panel *ngFor="let question of questions; trackBy: trackByQuestionId" class="question-panel">
            <mat-expansion-panel-header>
              <mat-panel-title>
                <div class="question-header">
                  <span class="question-number">{{ question.displayOrder }}</span>
                  <span class="question-text">{{ question.questionText }}</span>
                  <mat-chip-set>
                    <mat-chip [color]="question.isRequired ? 'warn' : 'primary'" selected>
                      {{ question.isRequired ? 'Required' : 'Optional' }}
                    </mat-chip>
                    <mat-chip color="accent" selected>
                      {{ getQuestionTypeName(question.questionTypeId) }}
                    </mat-chip>
                    <mat-chip *ngIf="question.options && question.options.length > 0" color="primary" selected>
                      {{ question.options.length }} option(s)
                    </mat-chip>
                  </mat-chip-set>
                </div>
              </mat-panel-title>
              <mat-panel-description>
                <div class="question-actions">
                  <button mat-icon-button (click)="editQuestion(question); $event.stopPropagation()" 
                          matTooltip="Edit Question">
                    <mat-icon>edit</mat-icon>
                  </button>
                  <button mat-icon-button color="warn" (click)="deleteQuestion(question); $event.stopPropagation()" 
                          matTooltip="Delete Question">
                    <mat-icon>delete</mat-icon>
                  </button>
                </div>
              </mat-panel-description>
            </mat-expansion-panel-header>

            <div class="question-details">
              <div class="detail-row" *ngIf="question.sectionName">
                <strong>Section:</strong> {{ question.sectionName }}
              </div>
              <div class="detail-row" *ngIf="question.helpText">
                <strong>Help Text:</strong> {{ question.helpText }}
              </div>
              <div class="detail-row" *ngIf="question.placeholder">
                <strong>Placeholder:</strong> {{ question.placeholder }}
              </div>
              
              <div class="validation-details" *ngIf="hasValidation(question)">
                <h4>Validation Rules:</h4>
                <div class="validation-grid">
                  <div *ngIf="question.minLength" class="validation-item">
                    <strong>Min Length:</strong> {{ question.minLength }}
                  </div>
                  <div *ngIf="question.maxLength" class="validation-item">
                    <strong>Max Length:</strong> {{ question.maxLength }}
                  </div>
                  <div *ngIf="question.minValue" class="validation-item">
                    <strong>Min Value:</strong> {{ question.minValue }}
                  </div>
                  <div *ngIf="question.maxValue" class="validation-item">
                    <strong>Max Value:</strong> {{ question.maxValue }}
                  </div>
                </div>
              </div>

              <div class="options-section" *ngIf="question.options && question.options.length > 0">
                <h4>Options:</h4>
                <mat-list>
                  <mat-list-item *ngFor="let option of question.options; trackBy: trackByOptionId">
                    <div class="option-item">
                      <span class="option-text">{{ option.optionText }}</span>
                      <span class="option-value">({{ option.optionValue }})</span>
                      <mat-chip-set>
                        <mat-chip *ngIf="option.isCorrect" color="accent" selected>Correct</mat-chip>
                        <mat-chip *ngIf="!option.isActive" color="warn" selected>Inactive</mat-chip>
                      </mat-chip-set>
                    </div>
                  </mat-list-item>
                </mat-list>
              </div>
            </div>
          </mat-expansion-panel>
        </mat-accordion>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .actions-bar {
      display: flex;
      gap: 12px;
      margin-bottom: 16px;
      align-items: center;
    }
    
    .loading, .no-questions {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: 32px;
      text-align: center;
      color: #666;
    }
    
    .loading mat-icon, .no-questions mat-icon {
      font-size: 48px;
      width: 48px;
      height: 48px;
      margin-bottom: 16px;
      color: #ccc;
    }
    
    .question-panel {
      margin-bottom: 8px;
    }
    
    .question-header {
      display: flex;
      align-items: center;
      gap: 12px;
      flex: 1;
    }
    
    .question-number {
      background: #3f51b5;
      color: white;
      border-radius: 50%;
      width: 24px;
      height: 24px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 12px;
      font-weight: bold;
    }
    
    .question-text {
      flex: 1;
      font-weight: 500;
      max-width: 400px;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }
    
    .question-actions {
      display: flex;
      gap: 4px;
    }
    
    .question-details {
      padding: 16px 0;
    }
    
    .detail-row {
      margin-bottom: 8px;
      padding: 8px;
      background: #f5f5f5;
      border-radius: 4px;
    }
    
    .validation-details {
      margin: 16px 0;
      padding: 16px;
      background: #e3f2fd;
      border-radius: 4px;
    }
    
    .validation-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 8px;
      margin-top: 8px;
    }
    
    .validation-item {
      padding: 4px 8px;
      background: white;
      border-radius: 4px;
      font-size: 12px;
    }
    
    .options-section {
      margin-top: 16px;
      padding: 16px;
      background: #f9f9f9;
      border-radius: 4px;
    }
    
    .options-section h4 {
      margin: 0 0 12px 0;
      color: #333;
    }
    
    .option-item {
      display: flex;
      align-items: center;
      gap: 12px;
      width: 100%;
    }
    
    .option-text {
      flex: 1;
      font-weight: 500;
    }
    
    .option-value {
      color: #666;
      font-size: 12px;
    }
    
    mat-card-header {
      background: #f5f5f5;
      margin: -16px -16px 16px -16px;
      padding: 16px;
    }
    
    mat-card-title {
      display: flex;
      align-items: center;
      gap: 8px;
    }
    
    .close-button {
      position: absolute;
      top: 8px;
      right: 8px;
    }
    
    mat-card-header {
      position: relative;
    }

    .order-manager-section {
      margin-bottom: 24px;
      padding: 16px;
      background: #f8f9fa;
      border-radius: 8px;
      border: 1px solid #e9ecef;

      .order-actions {
        display: flex;
        gap: 8px;
        margin-top: 16px;
        justify-content: flex-end;
      }
    }
  `]
})
export class QuestionListComponent implements OnInit {
  @Input() questionnaire!: Questionnaire;
  
  questions: Question[] = [];
  questionTypes: QuestionType[] = [];
  loading = false;
  showQuestionOrderManager = false;
  questionOrderItems: OrderItem[] = [];

  constructor(
    private questionService: QuestionService,
    private orderService: OrderService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    @Optional() private dialogRef: MatDialogRef<QuestionListComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) private dialogData: any
  ) {}

  ngOnInit(): void {
    // If this component is opened as a dialog, get questionnaire from dialog data
    if (this.dialogData && this.dialogData.questionnaire) {
      this.questionnaire = this.dialogData.questionnaire;
    }
    
    this.loadQuestionTypes();
    this.loadQuestions();
  }

  loadQuestionTypes(): void {
    this.questionService.getQuestionTypes().subscribe({
      next: (response) => {
        if (response.statusCode === 200 && response.data) {
          this.questionTypes = response.data;
        } else {
          console.error('Error loading question types:', response.message);
          this.snackBar.open('Error loading question types: ' + response.message, 'Close', { duration: 3000 });
          this.questionTypes = [];
        }
      },
      error: (error) => {
        console.error('Error loading question types:', error);
        this.snackBar.open('Error loading question types', 'Close', { duration: 3000 });
        this.questionTypes = [];
      }
    });
  }

  loadQuestions(): void {
    if (!this.questionnaire?.id) {
      console.warn('No questionnaire ID available');
      return;
    }
    
    this.loading = true;
    this.questionService.getQuestionsByQuestionnaireId(this.questionnaire.id).subscribe({
      next: (response) => {
        if (response.statusCode === 200 && response.data) {
          this.questions = response.data.sort((a, b) => a.displayOrder - b.displayOrder);
        } else {
          console.warn('Failed to load questions:', response.message);
          this.snackBar.open('Error loading questions: ' + response.message, 'Close', { duration: 3000 });
          this.questions = [];
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading questions:', error);
        this.snackBar.open('Error loading questions', 'Close', { duration: 3000 });
        this.loading = false;
        this.questions = [];
      }
    });
  }

  refreshQuestions(): void {
    this.loadQuestions();
  }

  addQuestion(): void {
    if (!this.questionnaire?.id) {
      this.snackBar.open('No questionnaire selected', 'Close', { duration: 3000 });
      return;
    }
    
    // Ensure question types are loaded
    if (!this.questionTypes || this.questionTypes.length === 0) {
      this.loadQuestionTypes();
      this.snackBar.open('Loading question types...', 'Close', { duration: 2000 });
      return;
    }
    
    const dialogRef = this.dialog.open(QuestionDialogComponent, {
      width: '800px',
      maxHeight: '90vh',
      data: {
        questionnaireId: this.questionnaire.id,
        questionTypes: this.questionTypes || [],
        mode: 'create'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.action === 'create') {
        this.createQuestion(result.data, result.options);
      }
    });
  }

  editQuestion(question: Question): void {
    if (!this.questionnaire?.id) {
      this.snackBar.open('No questionnaire selected', 'Close', { duration: 3000 });
      return;
    }
    
    // Ensure question types are loaded
    if (!this.questionTypes || this.questionTypes.length === 0) {
      this.loadQuestionTypes();
      this.snackBar.open('Loading question types...', 'Close', { duration: 2000 });
      return;
    }
    
    const dialogRef = this.dialog.open(QuestionDialogComponent, {
      width: '800px',
      maxHeight: '90vh',
      data: {
        question: question,
        questionnaireId: this.questionnaire.id,
        questionTypes: this.questionTypes || [],
        mode: 'edit'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.action === 'update') {
        this.updateQuestion(question.id, result.data, result.options);
      }
    });
  }

  deleteQuestion(question: Question): void {
    if (!this.questionnaire?.id) {
      this.snackBar.open('No questionnaire selected', 'Close', { duration: 3000 });
      return;
    }
    
    if (confirm(`Are you sure you want to delete the question "${question.questionText}"?`)) {
      this.questionService.deleteQuestion(this.questionnaire.id, question.id).subscribe({
        next: () => {
          this.loadQuestions(); // Refresh the questions list
          this.snackBar.open('Question deleted successfully', 'Close', { duration: 3000 });
        },
        error: (error) => {
          console.error('Error deleting question:', error);
          this.snackBar.open('Error deleting question', 'Close', { duration: 3000 });
        }
      });
    }
  }

  private createQuestion(questionData: any, options: any[]): void {
    // Include options in the question data
    const questionWithOptions = {
      ...questionData,
      options: options || []
    };

    this.questionService.createQuestion(questionWithOptions).subscribe({
      next: (createdQuestion) => {
        this.loadQuestions(); // Refresh the questions list
        this.snackBar.open('Question created successfully', 'Close', { duration: 3000 });
      },
      error: (error) => {
        console.error('Error creating question:', error);
        this.snackBar.open('Error creating question', 'Close', { duration: 3000 });
      }
    });
  }

  private updateQuestion(questionId: string, questionData: any, options: any[]): void {
    if (!this.questionnaire?.id) {
      this.snackBar.open('No questionnaire selected', 'Close', { duration: 3000 });
      return;
    }
    
    // Include options in the question data
    const questionWithOptions = {
      ...questionData,
      options: options || []
    };
    
    this.questionService.updateQuestion(this.questionnaire.id, questionId, questionWithOptions).subscribe({
      next: (updatedQuestion) => {
        this.loadQuestions(); // Refresh the questions list
        this.snackBar.open('Question updated successfully', 'Close', { duration: 3000 });
      },
      error: (error) => {
        console.error('Error updating question:', error);
        this.snackBar.open('Error updating question', 'Close', { duration: 3000 });
      }
    });
  }

  private createQuestionOptions(questionId: string, options: any[]): void {
    if (!this.questionnaire?.id) {
      this.snackBar.open('No questionnaire selected', 'Close', { duration: 3000 });
      return;
    }
    
    const optionPromises = options.map(option => 
      this.questionService.createQuestionOption(this.questionnaire.id, questionId, option)
    );

    Promise.all(optionPromises).then(() => {
      this.loadQuestions(); // Refresh the questions list
      this.snackBar.open('Question and options created successfully', 'Close', { duration: 3000 });
    }).catch(error => {
      console.error('Error creating question options:', error);
      this.snackBar.open('Question created but error creating options', 'Close', { duration: 3000 });
    });
  }

  private updateQuestionOptions(questionId: string, options: any[]): void {
    if (!this.questionnaire?.id) {
      this.snackBar.open('No questionnaire selected', 'Close', { duration: 3000 });
      return;
    }
    
    // For simplicity, we'll delete existing options and create new ones
    // In a production app, you'd want to handle updates more efficiently
    this.questionService.getQuestionOptions(this.questionnaire.id, questionId).subscribe({
      next: (response) => {
        if (response.statusCode === 200 && response.data) {
          const existingOptions = response.data;
          // Delete existing options
          const deletePromises = existingOptions.map(option => 
            this.questionService.deleteQuestionOption(this.questionnaire.id, questionId, option.id)
          );

          Promise.all(deletePromises).then(() => {
            // Create new options
            const createPromises = options.map(option => 
              this.questionService.createQuestionOption(this.questionnaire.id, questionId, option)
            );

            Promise.all(createPromises).then(() => {
              this.loadQuestions(); // Refresh the questions list
              this.snackBar.open('Question and options updated successfully', 'Close', { duration: 3000 });
            });
          });
        } else {
          this.snackBar.open('Error loading question options: ' + response.message, 'Close', { duration: 3000 });
        }
      },
      error: (error) => {
        this.snackBar.open('Error loading question options: ' + error.message, 'Close', { duration: 3000 });
      }
    });
  }

  getRequiredCount(): number {
    return this.questions.filter(q => q.isRequired).length;
  }

  getQuestionTypeName(typeId: string): string {
    const type = this.questionTypes.find(t => t.id === typeId);
    return type?.displayName || 'Unknown';
  }

  hasValidation(question: Question): boolean {
    return !!(question.minLength || question.maxLength || question.minValue || question.maxValue);
  }

  trackByQuestionId(index: number, question: Question): string {
    return question.id;
  }

  trackByOptionId(index: number, option: any): string {
    return option.id || index;
  }

  closeDialog(): void {
    if (this.dialogRef) {
      this.dialogRef.close();
    }
  }

  isDialogMode(): boolean {
    return !!this.dialogRef;
  }

  // Question Order Management
  toggleQuestionOrderManager(): void {
    this.showQuestionOrderManager = !this.showQuestionOrderManager;
    if (this.showQuestionOrderManager) {
      this.loadQuestionOrderItems();
    }
  }

  loadQuestionOrderItems(): void {
    this.questionOrderItems = this.questions.map(question => ({
      id: question.id,
      displayOrder: question.displayOrder,
      name: question.questionText,
      type: 'question' as const
    }));
  }

  onQuestionOrderChanged(orderItems: OrderItem[]): void {
    this.questionOrderItems = orderItems;
    
    // Update the questions array with new order
    this.questions = this.questions.map(question => {
      const orderItem = orderItems.find(item => item.id === question.id);
      if (orderItem) {
        return { ...question, displayOrder: orderItem.displayOrder };
      }
      return question;
    });

    // Sort questions by display order
    this.questions.sort((a, b) => a.displayOrder - b.displayOrder);
  }

  saveQuestionOrder(): void {
    if (!this.questionnaire?.id) {
      this.snackBar.open('No questionnaire selected', 'Close', { duration: 3000 });
      return;
    }

    this.questionService.updateQuestionOrder(this.questionnaire.id, this.questions).subscribe({
      next: (response) => {
        if (response.statusCode === 200 && response.data) {
          this.questions = response.data;
          this.snackBar.open('Question order updated successfully!', 'Close', { duration: 3000 });
          this.showQuestionOrderManager = false;
        } else {
          this.snackBar.open('Error updating question order: ' + response.message, 'Close', { duration: 5000 });
        }
      },
      error: (error) => {
        this.snackBar.open('Error updating question order: ' + error.message, 'Close', { duration: 5000 });
      }
    });
  }
} 