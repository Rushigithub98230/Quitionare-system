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

import { Questionnaire, CreateQuestionnaireDto, UpdateQuestionnaireDto } from '../../../core/models/questionnaire.model';
import { Category } from '../../../core/models/category.model';

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
          </mat-form-field>
        </div>

        <div class="form-row">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Description</mat-label>
            <textarea matInput formControlName="description" placeholder="Enter questionnaire description" rows="3"></textarea>
            <mat-error *ngIf="questionnaireForm.get('description')?.hasError('maxlength')">
              Description must be less than 1000 characters
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
        <button mat-raised-button color="primary" type="submit" [disabled]="questionnaireForm.invalid || loading">
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
  `]
})
export class QuestionnaireDialogComponent implements OnInit {
  questionnaireForm: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<QuestionnaireDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: QuestionnaireDialogData
  ) {
    this.questionnaireForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(255)]],
      description: ['', [Validators.maxLength(1000)]],
      categoryId: ['', [Validators.required]],
      displayOrder: [0, [Validators.required, Validators.min(0)]],
      version: [1, [Validators.min(1)]],
      isActive: [true],
      isMandatory: [false]
    });
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
  }

  onSubmit(): void {
    if (this.questionnaireForm.valid) {
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
          // createdBy: '00000000-0000-0000-0000-000000000001' // Hardcoded admin user ID
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
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
} 