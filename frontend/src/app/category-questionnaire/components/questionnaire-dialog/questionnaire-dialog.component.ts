import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { debounceTime, distinctUntilChanged, switchMap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

import { Questionnaire, CreateQuestionnaireDto, UpdateQuestionnaireDto } from '../../models/questionnaire.model';
import { Category } from '../../models/category.model';
import { QuestionnaireService } from '../../services/questionnaire.service';

export interface QuestionnaireDialogData {
  questionnaire?: Questionnaire;
  categories: Category[];
  mode: 'create' | 'edit';
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
    MatCheckboxModule,
    MatSelectModule,
    MatIconModule
  ],
  template: `
    <h2 mat-dialog-title>{{ data.mode === 'create' ? 'Create Questionnaire' : 'Edit Questionnaire' }}</h2>
    
    <form [formGroup]="questionnaireForm" (ngSubmit)="onSubmit()">
      <mat-dialog-content>
        <div class="form-row">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Questionnaire Title</mat-label>
            <input matInput formControlName="title" placeholder="Enter questionnaire title">
            <mat-error *ngIf="questionnaireForm.get('title')?.hasError('required')">
              Title is required
            </mat-error>
            <mat-error *ngIf="questionnaireForm.get('title')?.hasError('maxlength')">
              Title must be less than 255 characters
            </mat-error>
            <mat-error *ngIf="questionnaireForm.get('title')?.hasError('duplicate')">
              A questionnaire with this title already exists
            </mat-error>
            <mat-hint *ngIf="titleChecking" class="checking-hint">
              <mat-icon>hourglass_empty</mat-icon> Checking title availability...
            </mat-hint>
            <mat-hint *ngIf="!titleChecking && questionnaireForm.get('title')?.valid && !questionnaireForm.get('title')?.hasError('duplicate')" class="available-hint">
              <mat-icon>check_circle</mat-icon> Title is available
            </mat-hint>
          </mat-form-field>
        </div>

        <div class="form-row">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Description</mat-label>
            <textarea matInput formControlName="description" placeholder="Enter questionnaire description" rows="3"></textarea>
            <mat-error *ngIf="questionnaireForm.get('description')?.hasError('maxlength')">
              Description must be less than 500 characters
            </mat-error>
          </mat-form-field>
        </div>

        <div class="form-row">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Category</mat-label>
            <mat-select formControlName="categoryId" placeholder="Select a category">
              <mat-option *ngFor="let category of data.categories" [value]="category.id">
                {{ category.name }}
              </mat-option>
            </mat-select>
            <mat-error *ngIf="questionnaireForm.get('categoryId')?.hasError('required')">
              Category is required
            </mat-error>
          </mat-form-field>
        </div>

        <div class="form-row">
          <mat-form-field appearance="outline">
            <mat-label>Display Order</mat-label>
            <input matInput type="number" formControlName="displayOrder" placeholder="0">
            <mat-error *ngIf="questionnaireForm.get('displayOrder')?.hasError('required')">
              Display order is required
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Version</mat-label>
            <input matInput type="number" formControlName="version" placeholder="1">
            <mat-error *ngIf="questionnaireForm.get('version')?.hasError('min')">
              Version must be at least 1
            </mat-error>
          </mat-form-field>
        </div>

        <div class="checkbox-row">
          <mat-checkbox formControlName="isActive">Active</mat-checkbox>
          <mat-checkbox formControlName="isMandatory">Mandatory</mat-checkbox>
        </div>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button type="button" (click)="onCancel()">Cancel</button>
        <button mat-raised-button color="primary" type="submit" [disabled]="questionnaireForm.invalid || loading || questionnaireForm.get('title')?.hasError('duplicate')">
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
    }
    
    .full-width {
      width: 100%;
    }
    
    .checkbox-row {
      display: flex;
      gap: 16px;
      margin-bottom: 16px;
    }
    
    mat-form-field {
      flex: 1;
    }
    
    mat-dialog-content {
      max-height: 70vh;
      overflow-y: auto;
    }

    .checking-hint {
      color: #ff9800;
      font-size: 12px;
    }

    .checking-hint mat-icon {
      font-size: 16px;
      width: 16px;
      height: 16px;
      vertical-align: middle;
    }

    .available-hint {
      color: #4caf50;
      font-size: 12px;
    }

    .available-hint mat-icon {
      font-size: 16px;
      width: 16px;
      height: 16px;
      vertical-align: middle;
    }
  `]
})
export class QuestionnaireDialogComponent implements OnInit {
  questionnaireForm: FormGroup;
  loading = false;
  titleChecking = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<QuestionnaireDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: QuestionnaireDialogData,
    private questionnaireService: QuestionnaireService,
    private snackBar: MatSnackBar
  ) {
    this.questionnaireForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(255)]],
      description: ['', [Validators.maxLength(500)]],
      categoryId: ['', [Validators.required]],
      displayOrder: [0, [Validators.required, Validators.min(0)]],
      version: [1, [Validators.min(1)]],
      isActive: [true],
      isMandatory: [false]
    });

    // Set up real-time title validation
    this.setupTitleValidation();
  }

  ngOnInit(): void {
    if (this.data.mode === 'edit' && this.data.questionnaire) {
      this.questionnaireForm.patchValue({
        title: this.data.questionnaire.title,
        description: this.data.questionnaire.description,
        categoryId: this.data.questionnaire.categoryId,
        displayOrder: this.data.questionnaire.displayOrder,
        version: this.data.questionnaire.version,
        isActive: this.data.questionnaire.isActive,
        isMandatory: this.data.questionnaire.isMandatory
      });
    }

    // Add custom validation for category uniqueness
    this.questionnaireForm.get('categoryId')?.valueChanges.subscribe(categoryId => {
      if (categoryId && this.data.mode === 'create') {
        this.validateCategoryUniqueness(categoryId);
      }
    });
  }

  private setupTitleValidation(): void {
    const titleControl = this.questionnaireForm.get('title');
    if (titleControl) {
      titleControl.valueChanges.pipe(
        debounceTime(500), // Wait 500ms after user stops typing
        distinctUntilChanged(), // Only emit if value has changed
        switchMap(title => {
          if (!title || title.length < 2) {
            this.titleChecking = false;
            return of(null);
          }
          
          this.titleChecking = true;
          return this.questionnaireService.checkTitleExists(title).pipe(
            catchError(() => {
              this.titleChecking = false;
              return of(null);
            })
          );
        })
      ).subscribe(response => {
        this.titleChecking = false;
        
        if (response && response.statusCode === 200 && response.data) {
          const exists = response.data.exists;
          
          if (exists && this.data.mode === 'create') {
            // For create mode, show error if title exists
            titleControl.setErrors({ ...titleControl.errors, duplicate: true });
          } else if (exists && this.data.mode === 'edit' && this.data.questionnaire) {
            // For edit mode, only show error if title exists and it's not the current questionnaire
            if (titleControl.value !== this.data.questionnaire.title) {
              titleControl.setErrors({ ...titleControl.errors, duplicate: true });
            } else {
              // Clear duplicate error if it's the same title
              const errors = { ...titleControl.errors };
              delete errors['duplicate'];
              titleControl.setErrors(Object.keys(errors).length > 0 ? errors : null);
            }
          } else {
            // Clear duplicate error if title doesn't exist
            const errors = { ...titleControl.errors };
            delete errors['duplicate'];
            titleControl.setErrors(Object.keys(errors).length > 0 ? errors : null);
          }
        }
      });
    }
  }

  validateCategoryUniqueness(categoryId: string): void {
    // This validation will be handled by the backend, but we can add frontend validation
    // to provide immediate feedback if we have access to existing questionnaires
    const categoryControl = this.questionnaireForm.get('categoryId');
    if (categoryControl) {
      // For now, we'll rely on backend validation
      // In a real application, you might want to fetch existing questionnaires
      // and validate here as well
    }
  }

  onSubmit(): void {
    if (this.questionnaireForm.valid && !this.questionnaireForm.get('title')?.hasError('duplicate')) {
      this.loading = true;
      const formValue = this.questionnaireForm.value;

      if (this.data.mode === 'create') {
        const createDto: CreateQuestionnaireDto = {
          title: formValue.title,
          description: formValue.description,
          categoryId: formValue.categoryId,
          displayOrder: formValue.displayOrder,
          version: formValue.version,
          isActive: formValue.isActive,
          isMandatory: formValue.isMandatory
          // TODO: Re-enable authentication for production
          // createdBy: 1 // Hardcoded admin user ID
        };
        this.dialogRef.close({ action: 'create', data: createDto });
      } else {
        const updateDto: UpdateQuestionnaireDto = {
          title: formValue.title,
          description: formValue.description,
          categoryId: formValue.categoryId,
          displayOrder: formValue.displayOrder,
          version: formValue.version,
          isActive: formValue.isActive,
          isMandatory: formValue.isMandatory
        };
        this.dialogRef.close({ action: 'update', data: updateDto });
      }
    } else if (this.questionnaireForm.get('title')?.hasError('duplicate')) {
      this.snackBar.open('Please choose a different questionnaire title. This title already exists.', 'Close', { duration: 5000 });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
} 