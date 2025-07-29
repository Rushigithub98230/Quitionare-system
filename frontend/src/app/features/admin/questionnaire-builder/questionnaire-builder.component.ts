import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { MatSliderModule } from '@angular/material/slider';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialogRef } from '@angular/material/dialog';
import { CategoryService } from '../../../services/category.service';
import { QuestionnaireService } from '../../../services/questionnaire.service';

export interface QuestionType {
  id: number;
  name: string;
  displayName: string;
  hasOptions: boolean;
  hasValidation: boolean;
}

@Component({
  selector: 'app-questionnaire-builder',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatCheckboxModule,
    MatRadioModule,
    MatSliderModule,
    MatChipsModule
  ],
  template: `
    <div class="builder-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>{{ isEditMode ? 'Edit' : 'Create' }} Questionnaire</mat-card-title>
        </mat-card-header>
        
        <mat-card-content>
          <form [formGroup]="questionnaireForm" (ngSubmit)="onSubmit()">
            <!-- Basic Information -->
            <div class="form-section">
              <h3>Basic Information</h3>
              <div class="form-row">
                <mat-form-field appearance="outline" class="half-width">
                  <mat-label>Title</mat-label>
                  <input matInput formControlName="title" placeholder="Enter questionnaire title">
                  <mat-error *ngIf="questionnaireForm.get('title')?.hasError('required')">
                    Title is required
                  </mat-error>
                </mat-form-field>
                
                <mat-form-field appearance="outline" class="half-width">
                  <mat-label>Category</mat-label>
                  <mat-select formControlName="categoryId">
                    <mat-option *ngFor="let category of categories" [value]="category.id">
                      {{ category.name }}
                    </mat-option>
                  </mat-select>
                  <mat-error *ngIf="questionnaireForm.get('categoryId')?.hasError('required')">
                    Category is required
                  </mat-error>
                </mat-form-field>
              </div>
              
              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Description</mat-label>
                <textarea matInput formControlName="description" rows="3" placeholder="Enter questionnaire description"></textarea>
              </mat-form-field>
              
              <div class="form-row">
                <mat-form-field appearance="outline" class="half-width">
                  <mat-label>Version</mat-label>
                  <input matInput formControlName="version" placeholder="1.0">
                  <mat-error *ngIf="questionnaireForm.get('version')?.hasError('required')">
                    Version is required
                  </mat-error>
                </mat-form-field>
                
                <div class="checkbox-field">
                  <mat-checkbox formControlName="isActive">Active</mat-checkbox>
                </div>
              </div>
            </div>
            
            <!-- Questions -->
            <div class="form-section">
              <div class="section-header">
                <h3>Questions</h3>
                <button type="button" mat-raised-button color="accent" (click)="addQuestion()">
                  <mat-icon>add</mat-icon>
                  Add Question
                </button>
              </div>
              
              <div formArrayName="questions" class="questions-container">
                <div *ngFor="let question of questionsArray.controls; let i = index" 
                     [formGroupName]="i" class="question-card">
                  <div class="question-header">
                    <h4>Question {{ i + 1 }}</h4>
                    <button type="button" mat-icon-button (click)="removeQuestion(i)" color="warn">
                      <mat-icon>delete</mat-icon>
                    </button>
                  </div>
                  
                  <div class="form-row">
                    <mat-form-field appearance="outline" class="half-width">
                      <mat-label>Question Text</mat-label>
                      <input matInput formControlName="text" placeholder="Enter question text">
                      <mat-error *ngIf="question.get('text')?.hasError('required')">
                        Question text is required
                      </mat-error>
                    </mat-form-field>
                    
                    <mat-form-field appearance="outline" class="half-width">
                      <mat-label>Question Type</mat-label>
                      <mat-select formControlName="questionTypeId" (selectionChange)="onQuestionTypeChange(i)">
                        <mat-option *ngFor="let type of questionTypes" [value]="type.id">
                          {{ type.displayName }}
                        </mat-option>
                      </mat-select>
                      <mat-error *ngIf="question.get('questionTypeId')?.hasError('required')">
                        Question type is required
                      </mat-error>
                    </mat-form-field>
                  </div>
                  
              <div class="form-row">
                <mat-form-field appearance="outline" class="half-width">
                  <mat-label>Order</mat-label>
                  <input matInput type="number" formControlName="order" min="1">
                  <mat-error *ngIf="question.get('order')?.hasError('required')">
                    Order is required
                  </mat-error>
                </mat-form-field>
                
                <div class="checkbox-field">
                  <mat-checkbox formControlName="isRequired">Required</mat-checkbox>
                </div>
              </div>
              
              <!-- Options for multiple choice questions -->
              <div *ngIf="getQuestionType(i)?.hasOptions" class="options-section">
                <h5>Options</h5>
                <div formArrayName="options" class="options-container">
                  <div *ngFor="let option of getQuestionOptions(i).controls; let j = index" 
                       [formGroupName]="j" class="option-row">
                    <mat-form-field appearance="outline" class="option-input">
                      <mat-label>Option {{ j + 1 }}</mat-label>
                      <input matInput formControlName="text" placeholder="Enter option text">
                    </mat-form-field>
                    <button type="button" mat-icon-button (click)="removeOption(i, j)" color="warn">
                      <mat-icon>remove</mat-icon>
                    </button>
                  </div>
                  <button type="button" mat-button (click)="addOption(i)">
                    <mat-icon>add</mat-icon>
                    Add Option
                  </button>
                </div>
              </div>
              
              <!-- Validation rules -->
              <div *ngIf="getQuestionType(i)?.hasValidation" class="validation-section">
                <h5>Validation</h5>
                <div class="form-row">
                  <mat-form-field appearance="outline" class="half-width">
                    <mat-label>Min Length</mat-label>
                    <input matInput type="number" formControlName="minLength" min="0">
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline" class="half-width">
                    <mat-label>Max Length</mat-label>
                    <input matInput type="number" formControlName="maxLength" min="0">
                  </mat-form-field>
                </div>
                
                <div class="form-row">
                  <mat-form-field appearance="outline" class="half-width">
                    <mat-label>Min Value</mat-label>
                    <input matInput type="number" formControlName="minValue">
                  </mat-form-field>
                  
                  <mat-form-field appearance="outline" class="half-width">
                    <mat-label>Max Value</mat-label>
                    <input matInput type="number" formControlName="maxValue">
                  </mat-form-field>
                </div>
              </div>
            </div>
          </div>
          
          <!-- Submit Buttons -->
          <div class="form-actions">
            <button type="button" mat-button (click)="onCancel()">Cancel</button>
            <button type="submit" mat-raised-button color="primary" 
                    [disabled]="questionnaireForm.invalid || isLoading">
              {{ isLoading ? 'Saving...' : (isEditMode ? 'Update' : 'Create') }}
            </button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  </div>
  `,
  styles: [`
    .builder-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 20px;
    }
    
    .form-section {
      margin-bottom: 30px;
    }
    
    .form-section h3 {
      margin-bottom: 20px;
      color: #333;
      border-bottom: 2px solid #e0e0e0;
      padding-bottom: 10px;
    }
    
    .section-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
    }
    
    .form-row {
      display: flex;
      gap: 16px;
      margin-bottom: 16px;
    }
    
    .half-width {
      flex: 1;
    }
    
    .full-width {
      width: 100%;
    }
    
    .checkbox-field {
      display: flex;
      align-items: center;
      margin-top: 16px;
    }
    
    .questions-container {
      margin-top: 20px;
    }
    
    .question-card {
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      padding: 20px;
      margin-bottom: 20px;
      background: #fafafa;
    }
    
    .question-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
    }
    
    .question-header h4 {
      margin: 0;
      color: #333;
    }
    
    .options-section,
    .validation-section {
      margin-top: 20px;
      padding: 16px;
      background: white;
      border-radius: 4px;
      border: 1px solid #e0e0e0;
    }
    
    .options-section h5,
    .validation-section h5 {
      margin: 0 0 16px 0;
      color: #666;
    }
    
    .options-container {
      margin-top: 16px;
    }
    
    .option-row {
      display: flex;
      align-items: center;
      gap: 8px;
      margin-bottom: 8px;
    }
    
    .option-input {
      flex: 1;
    }
    
    .form-actions {
      display: flex;
      justify-content: flex-end;
      gap: 16px;
      margin-top: 30px;
      padding-top: 20px;
      border-top: 1px solid #e0e0e0;
    }
    
    @media (max-width: 600px) {
      .form-row {
        flex-direction: column;
        gap: 0;
      }
      
      .option-row {
        flex-direction: column;
        align-items: stretch;
      }
    }
  `]
})
export class QuestionnaireBuilderComponent implements OnInit {
  questionnaireForm: FormGroup;
  categories: any[] = [];
  questionTypes: QuestionType[] = [];
  isLoading = false;
  isEditMode = false;
  questionnaireId?: string;

  constructor(
    private fb: FormBuilder,
    private categoryService: CategoryService,
    private questionnaireService: QuestionnaireService,
    private snackBar: MatSnackBar,
    private dialogRef: MatDialogRef<QuestionnaireBuilderComponent>
  ) {
    this.questionnaireForm = this.createForm();
  }

  ngOnInit(): void {
    this.loadCategories();
    this.loadQuestionTypes();
  }

  createForm(): FormGroup {
    return this.fb.group({
      title: ['', [Validators.required]],
      description: [''],
      categoryId: ['', [Validators.required]],
      version: ['1.0', [Validators.required]],
      isActive: [true],
      questions: this.fb.array([])
    });
  }

  get questionsArray(): FormArray {
    return this.questionnaireForm.get('questions') as FormArray;
  }

  loadCategories(): void {
    this.categoryService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: (error) => {
        this.snackBar.open('Error loading categories', 'Close', { duration: 3000 });
      }
    });
  }

  loadQuestionTypes(): void {
    // This would typically come from the backend
    this.questionTypes = [
      { id: 1, name: 'Text', displayName: 'Text Input', hasOptions: false, hasValidation: true },
      { id: 2, name: 'Number', displayName: 'Number Input', hasOptions: false, hasValidation: true },
      { id: 3, name: 'Email', displayName: 'Email Input', hasOptions: false, hasValidation: true },
      { id: 4, name: 'Phone', displayName: 'Phone Input', hasOptions: false, hasValidation: true },
      { id: 5, name: 'Date', displayName: 'Date Picker', hasOptions: false, hasValidation: false },
      { id: 6, name: 'Radio', displayName: 'Single Choice', hasOptions: true, hasValidation: false },
      { id: 7, name: 'Checkbox', displayName: 'Multiple Choice', hasOptions: true, hasValidation: false },
      { id: 8, name: 'Dropdown', displayName: 'Dropdown', hasOptions: true, hasValidation: false },
      { id: 9, name: 'Rating', displayName: 'Rating', hasOptions: false, hasValidation: false },
      { id: 10, name: 'Slider', displayName: 'Slider', hasOptions: false, hasValidation: true },
      { id: 11, name: 'YesNo', displayName: 'Yes/No', hasOptions: false, hasValidation: false },
      { id: 12, name: 'File', displayName: 'File Upload', hasOptions: false, hasValidation: true },
      { id: 13, name: 'Image', displayName: 'Image Display', hasOptions: false, hasValidation: false },
      { id: 14, name: 'TextArea', displayName: 'Long Text', hasOptions: false, hasValidation: true }
    ];
  }

  addQuestion(): void {
    const question = this.fb.group({
      text: ['', [Validators.required]],
      questionTypeId: ['', [Validators.required]],
      order: [this.questionsArray.length + 1, [Validators.required]],
      isRequired: [false],
      minLength: [0],
      maxLength: [0],
      minValue: [null],
      maxValue: [null],
      options: this.fb.array([])
    });

    this.questionsArray.push(question);
  }

  removeQuestion(index: number): void {
    this.questionsArray.removeAt(index);
    // Update order numbers
    this.questionsArray.controls.forEach((control, i) => {
      control.patchValue({ order: i + 1 });
    });
  }

  getQuestionType(index: number): QuestionType | undefined {
    const questionTypeId = this.questionsArray.at(index).get('questionTypeId')?.value;
    return this.questionTypes.find(type => type.id === questionTypeId);
  }

  getQuestionOptions(index: number): FormArray {
    return this.questionsArray.at(index).get('options') as FormArray;
  }

  addOption(questionIndex: number): void {
    const options = this.getQuestionOptions(questionIndex);
    const option = this.fb.group({
      text: ['', [Validators.required]],
      order: [options.length + 1]
    });
    options.push(option);
  }

  removeOption(questionIndex: number, optionIndex: number): void {
    const options = this.getQuestionOptions(questionIndex);
    options.removeAt(optionIndex);
    // Update order numbers
    options.controls.forEach((control, i) => {
      control.patchValue({ order: i + 1 });
    });
  }

  onQuestionTypeChange(questionIndex: number): void {
    const question = this.questionsArray.at(questionIndex);
    const questionType = this.getQuestionType(questionIndex);
    
    // Clear options if question type doesn't support them
    if (!questionType?.hasOptions) {
      const options = question.get('options') as FormArray;
      options.clear();
    }
  }

  onSubmit(): void {
    if (this.questionnaireForm.valid) {
      this.isLoading = true;
      const formData = this.questionnaireForm.value;
      
      if (this.isEditMode && this.questionnaireId) {
        this.questionnaireService.updateQuestionnaire(this.questionnaireId, formData).subscribe({
          next: () => {
            this.isLoading = false;
            this.snackBar.open('Questionnaire updated successfully', 'Close', { duration: 3000 });
            this.dialogRef.close(true);
          },
          error: (error) => {
            this.isLoading = false;
            this.snackBar.open('Error updating questionnaire', 'Close', { duration: 3000 });
          }
        });
      } else {
        this.questionnaireService.createQuestionnaire(formData).subscribe({
          next: () => {
            this.isLoading = false;
            this.snackBar.open('Questionnaire created successfully', 'Close', { duration: 3000 });
            this.dialogRef.close(true);
          },
          error: (error) => {
            this.isLoading = false;
            this.snackBar.open('Error creating questionnaire', 'Close', { duration: 3000 });
          }
        });
      }
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  setEditMode(questionnaire: any): void {
    this.isEditMode = true;
    this.questionnaireId = questionnaire.id;
    this.questionnaireForm.patchValue({
      title: questionnaire.title,
      description: questionnaire.description,
      categoryId: questionnaire.categoryId,
      version: questionnaire.version,
      isActive: questionnaire.isActive
    });
    
    // Load questions
    if (questionnaire.questions) {
      questionnaire.questions.forEach((q: any) => {
        const question = this.fb.group({
          text: [q.text, [Validators.required]],
          questionTypeId: [q.questionTypeId, [Validators.required]],
          order: [q.order, [Validators.required]],
          isRequired: [q.isRequired],
          minLength: [q.minLength || 0],
          maxLength: [q.maxLength || 0],
          minValue: [q.minValue],
          maxValue: [q.maxValue],
          options: this.fb.array([])
        });
        
        // Load options
        if (q.options) {
          q.options.forEach((opt: any) => {
            const option = this.fb.group({
              text: [opt.text, [Validators.required]],
              order: [opt.order]
            });
            (question.get('options') as FormArray).push(option);
          });
        }
        
        this.questionsArray.push(question);
      });
    }
  }
} 