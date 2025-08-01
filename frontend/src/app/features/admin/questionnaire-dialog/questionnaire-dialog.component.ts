import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormArray } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { QuestionnaireService } from '../../../services/questionnaire.service';
import { CategoryService } from '../../../services/category.service';
import { CategoryQuestionnaireTemplate, CreateCategoryQuestionnaireTemplateRequest, UpdateCategoryQuestionnaireTemplateRequest, CreateCategoryQuestionRequest, CreateQuestionOptionRequest } from '../../../models/questionnaire.model';
import { Category } from '../../../models/category.model';

export interface QuestionnaireDialogData {
  questionnaire?: CategoryQuestionnaireTemplate;
  category?: Category;
  isEdit: boolean;
}

export interface QuestionType {
  id: string;
  name: string;
  displayName: string;
  hasOptions: boolean;
  hasValidation: boolean;
}

@Component({
  selector: 'app-questionnaire-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatCheckboxModule,
    MatExpansionModule,
    MatCardModule,
    MatProgressSpinnerModule
  ],
  template: `
    <h2 mat-dialog-title>
      <mat-icon>{{ data.isEdit ? 'edit' : 'add' }}</mat-icon>
      {{ data.isEdit ? 'Edit Questionnaire Template' : 'Create New Questionnaire Template' }}
      <span *ngIf="data.category" class="category-info">for {{ data.category.name }}</span>
    </h2>
    
    <form [formGroup]="questionnaireForm" (ngSubmit)="onSubmit()">
      <mat-dialog-content>
        <div class="form-container">
          <!-- Basic Information -->
          <mat-expansion-panel expanded="true">
            <mat-expansion-panel-header>
              <mat-panel-title>
                <mat-icon>info</mat-icon>
                Basic Information
              </mat-panel-title>
            </mat-expansion-panel-header>
            
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Template Title</mat-label>
              <input matInput formControlName="title" placeholder="Enter template title">
              <mat-error *ngIf="questionnaireForm.get('title')?.hasError('required')">
                Title is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Description</mat-label>
              <textarea matInput formControlName="description" 
                        placeholder="Enter template description"
                        rows="3"></textarea>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width" *ngIf="!data.category">
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

            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Display Order</mat-label>
                <input matInput type="number" formControlName="displayOrder" placeholder="1">
                <mat-error *ngIf="questionnaireForm.get('displayOrder')?.hasError('required')">
                  Display order is required
                </mat-error>
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Version</mat-label>
                <input matInput type="number" formControlName="version" placeholder="1">
                <mat-error *ngIf="questionnaireForm.get('version')?.hasError('required')">
                  Version is required
                </mat-error>
              </mat-form-field>
            </div>

            <div class="form-row">
              <mat-checkbox formControlName="isActive" color="primary">
                Active Template
              </mat-checkbox>
              <mat-checkbox formControlName="isMandatory" color="primary">
                Mandatory Template
              </mat-checkbox>
            </div>
          </mat-expansion-panel>

          <!-- Questions Section -->
          <mat-expansion-panel expanded="true">
            <mat-expansion-panel-header>
              <mat-panel-title>
                <mat-icon>quiz</mat-icon>
                Questions ({{ questionsArray.length }})
              </mat-panel-title>
            </mat-expansion-panel-header>
            
            <div class="questions-container">
              <div formArrayName="questions" class="question-list">
                <div *ngFor="let questionGroup of questionsArray.controls; let i = index" 
                     [formGroupName]="i" 
                     class="question-container">
                  
                  <mat-card>
                    <mat-card-header>
                      <mat-card-title>Question {{ i + 1 }}</mat-card-title>
                      <mat-card-subtitle>
                        <button mat-icon-button color="warn" type="button" (click)="removeQuestion(i)">
                          <mat-icon>delete</mat-icon>
                        </button>
                      </mat-card-subtitle>
                    </mat-card-header>
                    
                    <mat-card-content>
                      <div class="question-form">
                        <mat-form-field appearance="outline" class="full-width">
                          <mat-label>Question Text</mat-label>
                          <textarea matInput formControlName="questionText" 
                                    placeholder="Enter your question"
                                    rows="2"></textarea>
                          <mat-error *ngIf="questionGroup.get('questionText')?.hasError('required')">
                            Question text is required
                          </mat-error>
                        </mat-form-field>

                        <div class="form-row">
                          <mat-form-field appearance="outline" class="half-width">
                            <mat-label>Question Type</mat-label>
                            <mat-select formControlName="questionTypeId">
                              <mat-option *ngFor="let type of questionTypes" [value]="type.id">
                                {{ type.displayName }}
                              </mat-option>
                            </mat-select>
                            <mat-error *ngIf="questionGroup.get('questionTypeId')?.hasError('required')">
                              Question type is required
                            </mat-error>
                          </mat-form-field>

                          <mat-form-field appearance="outline" class="half-width">
                            <mat-label>Display Order</mat-label>
                            <input matInput type="number" formControlName="displayOrder" placeholder="1">
                            <mat-error *ngIf="questionGroup.get('displayOrder')?.hasError('required')">
                              Display order is required
                            </mat-error>
                          </mat-form-field>
                        </div>

                        <div class="form-row">
                          <mat-checkbox formControlName="isRequired" color="primary">
                            Required Question
                          </mat-checkbox>
                        </div>

                        <!-- Text/Number validation fields -->
                        <div *ngIf="isTextType(questionGroup.get('questionTypeId')?.value)" class="validation-fields">
                          <div class="form-row">
                            <mat-form-field appearance="outline" class="half-width">
                              <mat-label>Min Length</mat-label>
                              <input matInput type="number" formControlName="minLength" placeholder="0">
                            </mat-form-field>
                            <mat-form-field appearance="outline" class="half-width">
                              <mat-label>Max Length</mat-label>
                              <input matInput type="number" formControlName="maxLength" placeholder="255">
                            </mat-form-field>
                          </div>
                        </div>

                        <div *ngIf="isNumberType(questionGroup.get('questionTypeId')?.value)" class="validation-fields">
                          <div class="form-row">
                            <mat-form-field appearance="outline" class="half-width">
                              <mat-label>Min Value</mat-label>
                              <input matInput type="number" formControlName="minValue" placeholder="0">
                            </mat-form-field>
                            <mat-form-field appearance="outline" class="half-width">
                              <mat-label>Max Value</mat-label>
                              <input matInput type="number" formControlName="maxValue" placeholder="100">
                            </mat-form-field>
                          </div>
                        </div>

                        <!-- Options for choice questions -->
                        <div *ngIf="isChoiceType(questionGroup.get('questionTypeId')?.value)" class="options-section">
                          <h4>Options</h4>
                          <div formArrayName="options" class="options-list">
                            <div *ngFor="let optionGroup of getOptionsArray(questionGroup).controls; let j = index" 
                                 [formGroupName]="j" 
                                 class="option-item">
                              <div class="form-row">
                                <mat-form-field appearance="outline" class="full-width">
                                  <mat-label>Option {{ j + 1 }}</mat-label>
                                  <input matInput formControlName="optionText" placeholder="Enter option text">
                                  <mat-error *ngIf="optionGroup.get('optionText')?.hasError('required')">
                                    Option text is required
                                  </mat-error>
                                </mat-form-field>
                                <button mat-icon-button color="warn" type="button" (click)="removeOption(questionGroup, j)">
                                  <mat-icon>delete</mat-icon>
                                </button>
                              </div>
                            </div>
                          </div>
                          <button mat-button type="button" (click)="addOption(questionGroup)">
                            <mat-icon>add</mat-icon>
                            Add Option
                          </button>
                        </div>
                      </div>
                    </mat-card-content>
                  </mat-card>
                </div>
              </div>
              
              <button mat-raised-button type="button" (click)="addQuestion()" class="add-question-btn">
                <mat-icon>add</mat-icon>
                Add Question
              </button>
            </div>
          </mat-expansion-panel>
        </div>
      </mat-dialog-content>
      
      <mat-dialog-actions align="end">
        <button mat-button type="button" (click)="onCancel()">
          Cancel
        </button>
        <button mat-raised-button color="primary" type="submit" [disabled]="isLoading">
          <mat-spinner *ngIf="isLoading" diameter="20"></mat-spinner>
          <mat-icon *ngIf="!isLoading">{{ data.isEdit ? 'update' : 'add' }}</mat-icon>
          {{ data.isEdit ? 'Update Template' : 'Create Template' }}
        </button>
      </mat-dialog-actions>
    </form>
  `,
  styles: [`
    .form-container {
      max-width: 100%;
    }
    
    .full-width {
      width: 100%;
    }
    
    .half-width {
      width: 48%;
    }
    
    .form-row {
      display: flex;
      gap: 16px;
      align-items: center;
    }
    
    .question-container {
      margin-bottom: 16px;
    }
    
    .question-form {
      padding: 16px;
    }
    
    .validation-fields {
      margin-top: 16px;
      padding: 16px;
      background-color: #f5f5f5;
      border-radius: 4px;
    }
    
    .options-section {
      margin-top: 16px;
    }
    
    .options-section h4 {
      margin: 0 0 16px 0;
      color: #333;
    }
    
    .option-item {
      margin-bottom: 8px;
    }
    
    .add-question-btn {
      margin-top: 16px;
      width: 100%;
    }
    
    .category-info {
      font-size: 14px;
      color: #666;
      margin-left: 8px;
    }
    
    @media (max-width: 768px) {
      .form-row {
        flex-direction: column;
        gap: 8px;
      }
      
      .half-width {
        width: 100%;
      }
    }
  `]
})
export class QuestionnaireDialogComponent implements OnInit {
  questionnaireForm: FormGroup;
  categories: Category[] = [];
  questionTypes: QuestionType[] = [];
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private questionnaireService: QuestionnaireService,
    private categoryService: CategoryService,
    private dialogRef: MatDialogRef<QuestionnaireDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: QuestionnaireDialogData,
    private snackBar: MatSnackBar
  ) {
    this.questionnaireForm = this.createForm();
    this.loadQuestionTypes();
  }

  ngOnInit(): void {
    this.loadCategories();
    if (this.data.isEdit && this.data.questionnaire) {
      this.loadQuestionnaireData();
    }
  }

  createForm(): FormGroup {
    return this.fb.group({
      title: ['', Validators.required],
      description: [''],
      categoryId: [this.data.category?.id || '', Validators.required],
      isActive: [true],
      isMandatory: [false],
      displayOrder: [1, Validators.required],
      version: [1, Validators.required],
      questions: this.fb.array([])
    });
  }

  loadQuestionTypes(): void {
    this.questionTypes = [
      { id: '66666666-6666-6666-6666-666666666666', name: 'Text', displayName: 'Text Input', hasOptions: false, hasValidation: true },
      { id: 'cccccccc-cccc-cccc-cccc-cccccccccccc', name: 'Number', displayName: 'Number Input', hasOptions: false, hasValidation: true },
      { id: 'dddddddd-dddd-dddd-dddd-dddddddddddd', name: 'Email', displayName: 'Email Input', hasOptions: false, hasValidation: true },
      { id: 'gggggggg-gggg-gggg-gggg-gggggggggggg', name: 'Phone', displayName: 'Phone Input', hasOptions: false, hasValidation: true },
      { id: 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', name: 'Date', displayName: 'Date Picker', hasOptions: false, hasValidation: false },
      { id: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', name: 'Radio', displayName: 'Single Choice', hasOptions: true, hasValidation: false },
      { id: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', name: 'Checkbox', displayName: 'Multiple Choice', hasOptions: true, hasValidation: false },
      { id: 'hhhhhhhh-hhhh-hhhh-hhhh-hhhhhhhhhhhh', name: 'Dropdown', displayName: 'Dropdown', hasOptions: true, hasValidation: false },
      { id: 'ffffffff-ffff-ffff-ffff-ffffffffffff', name: 'Rating', displayName: 'Rating Scale', hasOptions: false, hasValidation: false },
      { id: 'iiiiiiii-iiii-iiii-iiii-iiiiiiiiiiii', name: 'Slider', displayName: 'Slider', hasOptions: false, hasValidation: true },
      { id: 'jjjjjjjj-jjjj-jjjj-jjjj-jjjjjjjjjjjj', name: 'YesNo', displayName: 'Yes/No', hasOptions: false, hasValidation: false },
      { id: 'kkkkkkkk-kkkk-kkkk-kkkk-kkkkkkkkkkkk', name: 'File', displayName: 'File Upload', hasOptions: false, hasValidation: true },
      { id: 'llllllll-llll-llll-llll-llllllllllll', name: 'Image', displayName: 'Image Display', hasOptions: false, hasValidation: false },
      { id: 'mmmmmmmm-mmmm-mmmm-mmmm-mmmmmmmmmmmm', name: 'TextArea', displayName: 'Long Text', hasOptions: false, hasValidation: true }
    ];
  }

  loadCategories(): void {
    this.categoryService.getCategories().subscribe({
      next: (response) => {
        if (response.success) {
          this.categories = response.data;
        }
      },
      error: (error) => {
        this.snackBar.open('Error loading categories', 'Close', { duration: 3000 });
      }
    });
  }

  loadQuestionnaireData(): void {
    if (this.data.questionnaire) {
      this.questionnaireForm.patchValue({
        title: this.data.questionnaire.title,
        description: this.data.questionnaire.description,
        categoryId: this.data.questionnaire.categoryId,
        isActive: this.data.questionnaire.isActive,
        isMandatory: this.data.questionnaire.isMandatory,
        displayOrder: this.data.questionnaire.displayOrder,
        version: this.data.questionnaire.version
      });

      // Load questions
      if (this.data.questionnaire.questions) {
        this.data.questionnaire.questions.forEach(question => {
          this.addQuestion(question);
        });
      }
    }
  }

  get questionsArray(): FormArray {
    return this.questionnaireForm.get('questions') as FormArray;
  }

  addQuestion(question?: any): void {
    const questionGroup = this.fb.group({
      questionText: [question?.questionText || '', Validators.required],
      questionTypeId: [question?.questionTypeId || '', Validators.required],
      isRequired: [question?.isRequired || false],
      displayOrder: [question?.displayOrder || this.questionsArray.length + 1, Validators.required],
      minLength: [question?.minLength || null],
      maxLength: [question?.maxLength || null],
      minValue: [question?.minValue || null],
      maxValue: [question?.maxValue || null],
      options: this.fb.array([])
    });

    this.questionsArray.push(questionGroup);

    // Add options if question has them
    if (question?.options) {
      question.options.forEach((option: any) => {
        this.addOption(questionGroup, option);
      });
    }
  }

  removeQuestion(index: number): void {
    this.questionsArray.removeAt(index);
  }

  getOptionsArray(questionGroup: any): FormArray {
    return questionGroup.get('options') as FormArray;
  }

  addOption(questionGroup: any, option?: any): void {
    const optionGroup = this.fb.group({
      optionText: [option?.optionText || '', Validators.required],
      displayOrder: [option?.displayOrder || this.getOptionsArray(questionGroup).length + 1],
      value: [option?.value || ''],
      isCorrect: [option?.isCorrect || false]
    });

    this.getOptionsArray(questionGroup).push(optionGroup);
  }

  removeOption(questionGroup: any, index: number): void {
    this.getOptionsArray(questionGroup).removeAt(index);
  }

  isTextType(questionTypeId: string): boolean {
    return questionTypeId === '66666666-6666-6666-6666-666666666666' ||
           questionTypeId === 'dddddddd-dddd-dddd-dddd-dddddddddddd' ||
           questionTypeId === 'gggggggg-gggg-gggg-gggg-gggggggggggg' ||
           questionTypeId === 'mmmmmmmm-mmmm-mmmm-mmmm-mmmmmmmmmmmm';
  }

  isNumberType(questionTypeId: string): boolean {
    return questionTypeId === 'cccccccc-cccc-cccc-cccc-cccccccccccc' ||
           questionTypeId === 'ffffffff-ffff-ffff-ffff-ffffffffffff' ||
           questionTypeId === 'iiiiiiii-iiii-iiii-iiii-iiiiiiiiiiii';
  }

  isChoiceType(questionTypeId: string): boolean {
    return questionTypeId === 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa' ||
           questionTypeId === 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb' ||
           questionTypeId === 'hhhhhhhh-hhhh-hhhh-hhhh-hhhhhhhhhhhh';
  }

  onSubmit(): void {
    if (this.questionnaireForm.valid) {
      this.isLoading = true;
      const formValue = this.questionnaireForm.value;

      if (this.data.isEdit && this.data.questionnaire) {
        // Update existing template
        const updateData: UpdateCategoryQuestionnaireTemplateRequest = {
          title: formValue.title,
          description: formValue.description,
          categoryId: formValue.categoryId,
          isActive: formValue.isActive,
          isMandatory: formValue.isMandatory,
          displayOrder: formValue.displayOrder,
          version: formValue.version,
          questions: [] // Add empty questions array to match backend DTO
        };

        this.questionnaireService.updateQuestionnaireTemplate(this.data.questionnaire.id, updateData).subscribe({
          next: (response: any) => {
            if (response.success) {
              this.snackBar.open('Questionnaire template updated successfully!', 'Close', { duration: 3000 });
              this.dialogRef.close(response.data);
            } else {
              this.snackBar.open(response.message || 'Failed to update questionnaire template', 'Close', { duration: 5000 });
            }
          },
          error: (error: any) => {
            this.snackBar.open('Error updating questionnaire template', 'Close', { duration: 5000 });
            console.error('Error updating questionnaire template:', error);
          },
          complete: () => {
            this.isLoading = false;
          }
        });
      } else {
        // Create new template
        const createData: CreateCategoryQuestionnaireTemplateRequest = {
          title: formValue.title,
          description: formValue.description,
          categoryId: formValue.categoryId,
          isActive: formValue.isActive,
          isMandatory: formValue.isMandatory,
          displayOrder: formValue.displayOrder,
          version: formValue.version,
          createdBy: 'cccccccc-dddd-eeee-ffff-111111111111', // TODO: Get from auth service
          questions: [] // Add empty questions array to match backend DTO
        };

        this.questionnaireService.createQuestionnaireTemplate(createData).subscribe({
          next: (response: any) => {
            if (response.success) {
              this.snackBar.open('Questionnaire template created successfully!', 'Close', { duration: 3000 });
              this.dialogRef.close(response.data);
            } else {
              this.snackBar.open(response.message || 'Failed to create questionnaire template', 'Close', { duration: 5000 });
            }
          },
          error: (error: any) => {
            this.snackBar.open('Error creating questionnaire template', 'Close', { duration: 5000 });
            console.error('Error creating questionnaire template:', error);
          },
          complete: () => {
            this.isLoading = false;
          }
        });
      }
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
} 