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
  templateUrl: './question-list.component.html',
  styleUrls: ['./question-list.component.scss']
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