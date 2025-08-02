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

import { Category, CreateCategoryDto, UpdateCategoryDto } from '../../../core/models/category.model';

export interface CategoryDialogData {
  category?: Category;
  mode: 'create' | 'edit';
}

@Component({
  selector: 'app-category-dialog',
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
    <h2 mat-dialog-title>{{ data.mode === 'create' ? 'Create Category' : 'Edit Category' }}</h2>
    
    <form [formGroup]="categoryForm" (ngSubmit)="onSubmit()">
      <mat-dialog-content>
        <div class="form-row">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Category Name</mat-label>
            <input matInput formControlName="name" placeholder="Enter category name">
            <mat-error *ngIf="categoryForm.get('name')?.hasError('required')">
              Category name is required
            </mat-error>
            <mat-error *ngIf="categoryForm.get('name')?.hasError('maxlength')">
              Category name must be less than 255 characters
            </mat-error>
          </mat-form-field>
        </div>

        <div class="form-row">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Description</mat-label>
            <textarea matInput formControlName="description" placeholder="Enter category description" rows="3"></textarea>
            <mat-error *ngIf="categoryForm.get('description')?.hasError('maxlength')">
              Description must be less than 1000 characters
            </mat-error>
          </mat-form-field>
        </div>

        <div class="form-row">
          <mat-form-field appearance="outline">
            <mat-label>Color</mat-label>
            <input matInput formControlName="color" placeholder="#007bff">
            <mat-error *ngIf="categoryForm.get('color')?.hasError('pattern')">
              Please enter a valid hex color
            </mat-error>
          </mat-form-field>
        </div>

        <div class="form-row">
          <mat-form-field appearance="outline">
            <mat-label>Base Price</mat-label>
            <input matInput type="number" formControlName="basePrice" placeholder="0.00" step="0.01">
            <mat-error *ngIf="categoryForm.get('basePrice')?.hasError('min')">
              Base price must be at least 0
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Consultation Duration (minutes)</mat-label>
            <input matInput type="number" formControlName="oneTimeConsultationDurationMinutes" placeholder="30">
            <mat-error *ngIf="categoryForm.get('oneTimeConsultationDurationMinutes')?.hasError('min')">
              Duration must be at least 1 minute
            </mat-error>
          </mat-form-field>
        </div>

        <div class="form-row">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Consultation Description</mat-label>
            <textarea matInput formControlName="consultationDescription" placeholder="Enter consultation description" rows="2"></textarea>
          </mat-form-field>
        </div>

        <div class="form-row">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Features</mat-label>
            <textarea matInput formControlName="features" placeholder="Enter features (comma separated)" rows="2"></textarea>
          </mat-form-field>
        </div>

        <div class="form-row">
          <mat-form-field appearance="outline">
            <mat-label>Icon</mat-label>
            <input matInput formControlName="icon" placeholder="Enter icon name">
          </mat-form-field>
        </div>

        <div class="checkbox-row">
          <mat-checkbox formControlName="isActive">Active</mat-checkbox>
          <mat-checkbox formControlName="allowsFollowUpMessaging">Allows Follow-up Messaging</mat-checkbox>
          <mat-checkbox formControlName="allowsMedicationDelivery">Allows Medication Delivery</mat-checkbox>
          <mat-checkbox formControlName="isMostPopular">Most Popular</mat-checkbox>
          <mat-checkbox formControlName="isTrending">Trending</mat-checkbox>
          <mat-checkbox formControlName="requiresQuestionnaireAssessment">Requires Questionnaire Assessment</mat-checkbox>
        </div>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button type="button" (click)="onCancel()">Cancel</button>
        <button mat-raised-button color="primary" type="submit" [disabled]="categoryForm.invalid || loading">
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
      flex-wrap: wrap;
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
export class CategoryDialogComponent implements OnInit {
  categoryForm: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<CategoryDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CategoryDialogData
  ) {
    this.categoryForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(255)]],
      description: ['', [Validators.maxLength(1000)]],
      color: ['#007bff', [Validators.pattern(/^#[0-9A-F]{6}$/i)]],
      basePrice: [0, [Validators.min(0)]],
      oneTimeConsultationDurationMinutes: [30, [Validators.min(1)]],
      consultationDescription: [''],
      features: [''],
      icon: [''],
      isActive: [true],
      allowsFollowUpMessaging: [false],
      allowsMedicationDelivery: [false],
      isMostPopular: [false],
      isTrending: [false],
      requiresQuestionnaireAssessment: [false]
    });
  }

  ngOnInit(): void {
    if (this.data.mode === 'edit' && this.data.category) {
      this.categoryForm.patchValue({
        name: this.data.category.name,
        description: this.data.category.description,
        color: this.data.category.color,
        basePrice: this.data.category.basePrice,
        oneTimeConsultationDurationMinutes: this.data.category.oneTimeConsultationDurationMinutes,
        consultationDescription: this.data.category.consultationDescription,
        features: this.data.category.features,
        icon: this.data.category.icon,
        isActive: this.data.category.isActive,
        allowsFollowUpMessaging: this.data.category.allowsFollowUpMessaging,
        allowsMedicationDelivery: this.data.category.allowsMedicationDelivery,
        isMostPopular: this.data.category.isMostPopular,
        isTrending: this.data.category.isTrending,
        requiresQuestionnaireAssessment: this.data.category.requiresQuestionnaireAssessment
      });
    }
  }

  onSubmit(): void {
    if (this.categoryForm.valid) {
      this.loading = true;
      const formValue = this.categoryForm.value;

      if (this.data.mode === 'create') {
        const createDto: CreateCategoryDto = {
          name: formValue.name,
          description: formValue.description,
          color: formValue.color,
          basePrice: formValue.basePrice,
          oneTimeConsultationDurationMinutes: formValue.oneTimeConsultationDurationMinutes,
          consultationDescription: formValue.consultationDescription,
          features: formValue.features,
          icon: formValue.icon,
          isActive: formValue.isActive,
          allowsFollowUpMessaging: formValue.allowsFollowUpMessaging,
          allowsMedicationDelivery: formValue.allowsMedicationDelivery,
          isMostPopular: formValue.isMostPopular,
          isTrending: formValue.isTrending,
          requiresQuestionnaireAssessment: formValue.requiresQuestionnaireAssessment
        };
        this.dialogRef.close({ action: 'create', data: createDto });
      } else {
        const updateDto: UpdateCategoryDto = {
          name: formValue.name,
          description: formValue.description,
          displayOrder: formValue.displayOrder,
          color: formValue.color,
          basePrice: formValue.basePrice,
          oneTimeConsultationDurationMinutes: formValue.oneTimeConsultationDurationMinutes,
          consultationDescription: formValue.consultationDescription,
          features: formValue.features,
          icon: formValue.icon,
          isActive: formValue.isActive,
          allowsFollowUpMessaging: formValue.allowsFollowUpMessaging,
          allowsMedicationDelivery: formValue.allowsMedicationDelivery,
          isMostPopular: formValue.isMostPopular,
          isTrending: formValue.isTrending,
          requiresQuestionnaireAssessment: formValue.requiresQuestionnaireAssessment
        };
        this.dialogRef.close({ action: 'update', data: updateDto });
      }
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
} 