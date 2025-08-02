import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormArray } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';

import { Question, CreateQuestionDto, UpdateQuestionDto, QuestionType, QuestionOption } from '../../../core/models/question.model';

export interface QuestionDialogData {
  question?: Question;
  questionnaireId: string;
  questionTypes: QuestionType[];
  mode: 'create' | 'edit';
}

@Component({
  selector: 'app-question-dialog',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatCheckboxModule, MatSelectModule, MatIconModule, MatChipsModule, MatDividerModule
  ],
  template: `
    <h2 mat-dialog-title>{{ data.mode === 'create' ? 'Create Question' : 'Edit Question' }}</h2>
    
    <form [formGroup]="questionForm" (ngSubmit)="onSubmit()">
      <mat-dialog-content>
        <div class="form-row">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Question Text</mat-label>
            <textarea matInput formControlName="questionText" placeholder="Enter your question" rows="3"></textarea>
            <mat-error *ngIf="questionForm.get('questionText')?.hasError('required')">
              Question text is required
            </mat-error>
          </mat-form-field>
        </div>

        <div class="form-row">
          <mat-form-field appearance="outline">
            <mat-label>Question Type</mat-label>
                         <mat-select formControlName="questionTypeId" (selectionChange)="onQuestionTypeChange()" [disabled]="data.questionTypes.length === 0">
               <mat-option *ngFor="let type of data.questionTypes" [value]="type.id">
                 {{ type.displayName }}
               </mat-option>
               <mat-option *ngIf="data.questionTypes.length === 0" disabled>
                 Loading question types...
               </mat-option>
             </mat-select>
            <mat-error *ngIf="questionForm.get('questionTypeId')?.hasError('required')">
              Question type is required
            </mat-error>
          </mat-form-field>

        </div>

        <div class="form-row">
          <mat-form-field appearance="outline">
            <mat-label>Section Name (Optional)</mat-label>
            <input matInput formControlName="sectionName" placeholder="e.g., Personal Information">
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Help Text (Optional)</mat-label>
            <input matInput formControlName="helpText" placeholder="Additional guidance for users">
          </mat-form-field>
        </div>

        <div class="form-row">
          <mat-form-field appearance="outline">
            <mat-label>Placeholder (Optional)</mat-label>
            <input matInput formControlName="placeholder" placeholder="Placeholder text">
          </mat-form-field>

          <mat-checkbox formControlName="isRequired">Required Question</mat-checkbox>
        </div>

        <!-- Validation Settings -->
        <mat-divider></mat-divider>
        <div class="validation-header">
          <h4>Validation Settings</h4>
          <button type="button" mat-button color="primary" (click)="toggleValidationSettings()">
            <mat-icon>{{ showValidationSettings ? 'visibility_off' : 'visibility' }}</mat-icon>
            {{ showValidationSettings ? 'Hide' : 'Show' }} Validation
          </button>
        </div>
        
        <div *ngIf="showValidationSettings" class="validation-fields">
          <div class="form-row">
            <mat-form-field appearance="outline">
              <mat-label>Minimum Length</mat-label>
              <input matInput type="number" formControlName="minLength" placeholder="0">
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Maximum Length</mat-label>
              <input matInput type="number" formControlName="maxLength" placeholder="255">
            </mat-form-field>
          </div>

          <div class="form-row">
            <mat-form-field appearance="outline">
              <mat-label>Minimum Value</mat-label>
              <input matInput type="number" formControlName="minValue" placeholder="0">
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Maximum Value</mat-label>
              <input matInput type="number" formControlName="maxValue" placeholder="100">
            </mat-form-field>
          </div>
        </div>

        <!-- Question Options for Multiple Choice Questions -->
        <div *ngIf="showOptionsSection()">
          <mat-divider></mat-divider>
          <div class="options-header">
            <h4>Question Options</h4>
            <button type="button" mat-mini-fab color="primary" (click)="addOption()">
              <mat-icon>add</mat-icon>
            </button>
          </div>

          <div formArrayName="options">
            <div *ngFor="let option of optionsArray.controls; let i = index" [formGroupName]="i" class="option-row">
              <mat-form-field appearance="outline">
                <mat-label>Option {{ i + 1 }}</mat-label>
                <input matInput formControlName="optionText" placeholder="Enter option text">
                <mat-error *ngIf="option.get('optionText')?.hasError('required')">
                  Option text is required
                </mat-error>
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>Value</mat-label>
                <input matInput formControlName="optionValue" placeholder="Option value">
                <mat-error *ngIf="option.get('optionValue')?.hasError('required')">
                  Option value is required
                </mat-error>
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>Order</mat-label>
                <input matInput type="number" formControlName="displayOrder" placeholder="{{ i }}">
              </mat-form-field>

              <mat-checkbox formControlName="isCorrect">Correct Answer</mat-checkbox>
              <mat-checkbox formControlName="isActive">Active</mat-checkbox>

              <button type="button" mat-icon-button color="warn" (click)="removeOption(i)" [disabled]="optionsArray.length <= 1">
                <mat-icon>delete</mat-icon>
              </button>
            </div>
          </div>
        </div>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button type="button" (click)="onCancel()">Cancel</button>
        <button mat-raised-button color="primary" type="submit" [disabled]="questionForm.invalid || loading">
          <mat-icon *ngIf="loading">hourglass_empty</mat-icon>
          {{ data.mode === 'create' ? 'Create' : 'Update' }}
        </button>
      </mat-dialog-actions>
    </form>
  `,
  styles: [`
    .form-row {
      display: flex;
      gap: 16px;
      margin-bottom: 16px;
      align-items: center;
    }
    
    .full-width {
      width: 100%;
    }
    
    .validation-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
    }
    
    .validation-fields {
      margin-top: 16px;
    }
    
    .options-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
    }
    
    .option-row {
      display: flex;
      gap: 12px;
      margin-bottom: 12px;
      align-items: center;
      padding: 12px;
      border: 1px solid #e0e0e0;
      border-radius: 4px;
      background-color: #fafafa;
    }
    
    mat-form-field {
      flex: 1;
    }
    
    mat-dialog-content {
      max-height: 70vh;
      overflow-y: auto;
    }
    
    h4 {
      margin: 16px 0 8px 0;
      color: #333;
    }
    
    mat-divider {
      margin: 16px 0;
    }
  `]
})
export class QuestionDialogComponent implements OnInit {
  questionForm: FormGroup;
  loading = false;
  showValidationSettings = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<QuestionDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: QuestionDialogData
  ) {
    // Ensure questionTypes is always an array
    if (!this.data.questionTypes || !Array.isArray(this.data.questionTypes)) {
      this.data.questionTypes = [];
    }
    
    this.questionForm = this.fb.group({
      questionText: ['', [Validators.required, Validators.maxLength(1000)]],
      questionTypeId: ['', [Validators.required]],
      isRequired: [false],
      sectionName: [''],
      helpText: [''],
      placeholder: [''],
      minLength: [null],
      maxLength: [null],
      minValue: [null],
      maxValue: [null],
      imageUrl: [''],
      imageAltText: [''],
      validationRules: [''],
      conditionalLogic: [''],
      settings: [''],
      options: this.fb.array([])
    });
  }

  ngOnInit(): void {
    if (this.data.mode === 'edit' && this.data.question) {
      this.questionForm.patchValue({
        questionText: this.data.question.questionText,
        questionTypeId: this.data.question.questionTypeId,
        isRequired: this.data.question.isRequired,
        sectionName: this.data.question.sectionName,
        helpText: this.data.question.helpText,
        placeholder: this.data.question.placeholder,
        minLength: this.data.question.minLength,
        maxLength: this.data.question.maxLength,
        minValue: this.data.question.minValue,
        maxValue: this.data.question.maxValue,
        imageUrl: this.data.question.imageUrl,
        imageAltText: this.data.question.imageAltText,
        validationRules: this.data.question.validationRules,
        conditionalLogic: this.data.question.conditionalLogic,
        settings: this.data.question.settings
      });

      // Load existing options
      if (this.data.question.options && this.data.question.options.length > 0) {
        this.data.question.options.forEach(option => {
          this.addOption(option);
        });
      }

      // Show validation settings if the question has validation values
      if (this.data.question.minLength || this.data.question.maxLength || 
          this.data.question.minValue || this.data.question.maxValue) {
        this.showValidationSettings = true;
      }
    } else {
      // Add a default option for new questions
      this.addOption();
    }
  }

  get optionsArray(): FormArray {
    return this.questionForm.get('options') as FormArray;
  }

  addOption(option?: QuestionOption): void {
    const optionGroup = this.fb.group({
      optionText: [option?.optionText || '', [Validators.required]],
      optionValue: [option?.optionValue || '', [Validators.required]],
      displayOrder: [option?.displayOrder || this.optionsArray.length],
      isCorrect: [option?.isCorrect || false],
      isActive: [option?.isActive !== false]
    });
    this.optionsArray.push(optionGroup);
  }

  removeOption(index: number): void {
    if (this.optionsArray.length > 1) {
      this.optionsArray.removeAt(index);
    }
  }

  showOptionsSection(): boolean {
    const questionTypeId = this.questionForm.get('questionTypeId')?.value;
    if (!questionTypeId || !this.data.questionTypes || this.data.questionTypes.length === 0) return false;
    
    const questionType = this.data.questionTypes.find(t => t.id === questionTypeId);
    return questionType?.typeName?.toLowerCase().includes('radio') || 
           questionType?.typeName?.toLowerCase().includes('checkbox') || 
           questionType?.typeName?.toLowerCase().includes('select') ||
           questionType?.typeName?.toLowerCase().includes('multiselect') || false;
  }

  onQuestionTypeChange(): void {
    // Reset options when question type changes
    if (!this.showOptionsSection()) {
      this.optionsArray.clear();
    } else if (this.optionsArray.length === 0) {
      this.addOption();
    }
  }

  toggleValidationSettings(): void {
    this.showValidationSettings = !this.showValidationSettings;
  }

  onSubmit(): void {
    if (this.questionForm.valid) {
      this.loading = true;
      const formValue = this.questionForm.value;

      if (this.data.mode === 'create') {
        const createDto: CreateQuestionDto = {
          questionnaireId: this.data.questionnaireId,
          questionText: formValue.questionText,
          questionTypeId: formValue.questionTypeId,
          isRequired: formValue.isRequired,
          sectionName: formValue.sectionName,
          helpText: formValue.helpText,
          placeholder: formValue.placeholder,
          minLength: formValue.minLength,
          maxLength: formValue.maxLength,
          minValue: formValue.minValue,
          maxValue: formValue.maxValue,
          imageUrl: formValue.imageUrl,
          imageAltText: formValue.imageAltText,
          validationRules: formValue.validationRules,
          conditionalLogic: formValue.conditionalLogic,
          settings: formValue.settings
        };
        this.dialogRef.close({ action: 'create', data: createDto, options: formValue.options });
      } else {
        const updateDto: UpdateQuestionDto = {
          questionText: formValue.questionText,
          questionTypeId: formValue.questionTypeId,
          isRequired: formValue.isRequired,
          sectionName: formValue.sectionName,
          helpText: formValue.helpText,
          placeholder: formValue.placeholder,
          minLength: formValue.minLength,
          maxLength: formValue.maxLength,
          minValue: formValue.minValue,
          maxValue: formValue.maxValue,
          imageUrl: formValue.imageUrl,
          imageAltText: formValue.imageAltText,
          validationRules: formValue.validationRules,
          conditionalLogic: formValue.conditionalLogic,
          settings: formValue.settings
        };
        this.dialogRef.close({ action: 'update', data: updateDto, options: formValue.options });
      }
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
} 