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
  templateUrl: './questionnaire-dialog.component.html',
  styleUrls: ['./questionnaire-dialog.component.scss']
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