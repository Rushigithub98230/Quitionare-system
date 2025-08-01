import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CategoryService } from '../../../services/category.service';
import { Category, CreateCategoryRequest, UpdateCategoryRequest } from '../../../models/category.model';

export interface CategoryDialogData {
  category?: Category;
  isEdit: boolean;
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
    MatIconModule,
    MatSelectModule,
    MatCheckboxModule,
    MatProgressSpinnerModule
  ],
  template: `
    <h2 mat-dialog-title>
      <mat-icon>{{ data.isEdit ? 'edit' : 'add' }}</mat-icon>
      {{ data.isEdit ? 'Edit Category' : 'Create New Category' }}
    </h2>
    
    <form [formGroup]="categoryForm" (ngSubmit)="onSubmit()">
      <mat-dialog-content>
        <div class="form-container">
          <!-- Basic Information -->
          <div class="form-section">
            <h3>Basic Information</h3>
            
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Category Name</mat-label>
              <input matInput formControlName="name" placeholder="Enter category name">
              <mat-error *ngIf="categoryForm.get('name')?.hasError('required')">
                Name is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Description</mat-label>
              <textarea matInput formControlName="description" 
                        placeholder="Enter category description"
                        rows="3"></textarea>
            </mat-form-field>

            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Icon</mat-label>
                <input matInput formControlName="icon" placeholder="e.g., medical_services">
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Color</mat-label>
                <input matInput formControlName="color" placeholder="e.g., #3f51b5">
              </mat-form-field>
            </div>

            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Display Order</mat-label>
                <input matInput type="number" formControlName="displayOrder" placeholder="1">
                <mat-error *ngIf="categoryForm.get('displayOrder')?.hasError('required')">
                  Display order is required
                </mat-error>
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Base Price</mat-label>
                <input matInput type="number" formControlName="basePrice" placeholder="0.00" step="0.01">
                <mat-error *ngIf="categoryForm.get('basePrice')?.hasError('required')">
                  Base price is required
                </mat-error>
              </mat-form-field>
            </div>
          </div>

          <!-- Features -->
          <div class="form-section">
            <h3>Features</h3>
            
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Features (comma-separated)</mat-label>
              <textarea matInput formControlName="features" 
                        placeholder="e.g., Video Consultation, Prescription, Follow-up"
                        rows="2"></textarea>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Consultation Description</mat-label>
              <textarea matInput formControlName="consultationDescription" 
                        placeholder="Describe what this consultation includes"
                        rows="3"></textarea>
            </mat-form-field>

            <div class="form-row">
              <mat-form-field appearance="outline" class="half-width">
                <mat-label>Consultation Duration (minutes)</mat-label>
                <input matInput type="number" formControlName="oneTimeConsultationDurationMinutes" placeholder="60">
              </mat-form-field>
            </div>
          </div>

          <!-- Settings -->
          <div class="form-section">
            <h3>Settings</h3>
            
            <div class="checkbox-group">
              <mat-checkbox formControlName="isActive" color="primary">
                Active Category
              </mat-checkbox>
              
              <mat-checkbox formControlName="requiresQuestionnaireAssessment" color="primary">
                Requires Questionnaire Assessment
              </mat-checkbox>
              
              <mat-checkbox formControlName="allowsMedicationDelivery" color="primary">
                Allows Medication Delivery
              </mat-checkbox>
              
              <mat-checkbox formControlName="allowsFollowUpMessaging" color="primary">
                Allows Follow-up Messaging
              </mat-checkbox>
            </div>
          </div>

          <!-- Marketing -->
          <div class="form-section">
            <h3>Marketing</h3>
            
            <div class="checkbox-group">
              <mat-checkbox formControlName="isMostPopular" color="primary">
                Mark as Most Popular
              </mat-checkbox>
              
              <mat-checkbox formControlName="isTrending" color="primary">
                Mark as Trending
              </mat-checkbox>
            </div>
          </div>
        </div>
      </mat-dialog-content>
      
      <mat-dialog-actions align="end">
        <button mat-button type="button" (click)="onCancel()">
          <mat-icon>cancel</mat-icon>
          Cancel
        </button>
        <button mat-raised-button color="primary" type="submit" [disabled]="isLoading">
          <mat-spinner *ngIf="isLoading" diameter="20"></mat-spinner>
          <mat-icon *ngIf="!isLoading">{{ data.isEdit ? 'update' : 'add' }}</mat-icon>
          {{ data.isEdit ? 'Update' : 'Create' }}
        </button>
      </mat-dialog-actions>
    </form>
  `,
  styles: [`
    .form-container {
      max-width: 800px;
      margin: 0 auto;
    }
    
    .form-section {
      margin-bottom: 24px;
      padding: 16px;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      background-color: #fafafa;
    }
    
    .form-section h3 {
      margin: 0 0 16px 0;
      color: #333;
      font-size: 18px;
      font-weight: 500;
    }
    
    .form-row {
      display: flex;
      gap: 16px;
      align-items: flex-start;
    }
    
    .half-width {
      flex: 1;
    }
    
    .full-width {
      width: 100%;
    }
    
    .checkbox-group {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }
    
    mat-form-field {
      margin-bottom: 16px;
    }
    
    @media (max-width: 768px) {
      .form-row {
        flex-direction: column;
      }
      
      .half-width {
        width: 100%;
      }
    }
  `]
})
export class CategoryDialogComponent implements OnInit {
  categoryForm: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private categoryService: CategoryService,
    private dialogRef: MatDialogRef<CategoryDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CategoryDialogData,
    private snackBar: MatSnackBar
  ) {
    this.categoryForm = this.createForm();
  }

  ngOnInit(): void {
    if (this.data.isEdit && this.data.category) {
      this.loadCategoryData();
    }
  }

  createForm(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]],
      icon: ['', [Validators.maxLength(100)]],
      color: ['', [Validators.maxLength(100)]],
      displayOrder: [1, [Validators.required, Validators.min(1)]],
      features: ['', [Validators.maxLength(1000)]],
      consultationDescription: ['', [Validators.maxLength(500)]],
      basePrice: [0, [Validators.required, Validators.min(0)]],
      requiresQuestionnaireAssessment: [true],
      allowsMedicationDelivery: [true],
      allowsFollowUpMessaging: [true],
      oneTimeConsultationDurationMinutes: [60, [Validators.min(15), Validators.max(180)]],
      isActive: [true],
      isMostPopular: [false],
      isTrending: [false]
    });
  }

  loadCategoryData(): void {
    const category = this.data.category!;
    this.categoryForm.patchValue({
      name: category.name,
      description: category.description || '',
      icon: category.icon || '',
      color: category.color || '',
      displayOrder: category.displayOrder,
      features: category.features || '',
      consultationDescription: category.consultationDescription || '',
      basePrice: category.basePrice,
      requiresQuestionnaireAssessment: category.requiresQuestionnaireAssessment,
      allowsMedicationDelivery: category.allowsMedicationDelivery,
      allowsFollowUpMessaging: category.allowsFollowUpMessaging,
      oneTimeConsultationDurationMinutes: category.oneTimeConsultationDurationMinutes,
      isActive: category.isActive,
      isMostPopular: category.isMostPopular,
      isTrending: category.isTrending
    });
  }

  onSubmit(): void {
    if (this.categoryForm.valid) {
      this.isLoading = true;
      const formData = this.categoryForm.value;

      if (this.data.isEdit && this.data.category) {
        // Update existing category
        const updateData: UpdateCategoryRequest = {
          name: formData.name,
          description: formData.description,
          icon: formData.icon,
          color: formData.color,
          isActive: formData.isActive,
          displayOrder: formData.displayOrder,
          features: formData.features,
          consultationDescription: formData.consultationDescription,
          basePrice: formData.basePrice,
          requiresQuestionnaireAssessment: formData.requiresQuestionnaireAssessment,
          allowsMedicationDelivery: formData.allowsMedicationDelivery,
          allowsFollowUpMessaging: formData.allowsFollowUpMessaging,
          oneTimeConsultationDurationMinutes: formData.oneTimeConsultationDurationMinutes,
          isMostPopular: formData.isMostPopular,
          isTrending: formData.isTrending
        };

        this.categoryService.updateCategory(this.data.category.id, updateData).subscribe({
          next: (response) => {
            if (response.success) {
              this.snackBar.open('Category updated successfully!', 'Close', { duration: 3000 });
              this.dialogRef.close(response.data);
            } else {
              this.snackBar.open(response.message || 'Failed to update category', 'Close', { duration: 5000 });
            }
          },
          error: (error) => {
            this.snackBar.open('Error updating category', 'Close', { duration: 5000 });
            console.error('Error updating category:', error);
          },
          complete: () => {
            this.isLoading = false;
          }
        });
      } else {
        // Create new category
        const createData: CreateCategoryRequest = {
          name: formData.name,
          description: formData.description,
          icon: formData.icon,
          color: formData.color,
          displayOrder: formData.displayOrder,
          features: formData.features,
          consultationDescription: formData.consultationDescription,
          basePrice: formData.basePrice,
          requiresQuestionnaireAssessment: formData.requiresQuestionnaireAssessment,
          allowsMedicationDelivery: formData.allowsMedicationDelivery,
          allowsFollowUpMessaging: formData.allowsFollowUpMessaging,
          oneTimeConsultationDurationMinutes: formData.oneTimeConsultationDurationMinutes,
          isActive: formData.isActive,
          isMostPopular: formData.isMostPopular,
          isTrending: formData.isTrending
        };

        this.categoryService.createCategory(createData).subscribe({
          next: (response) => {
            if (response.success) {
              this.snackBar.open('Category created successfully!', 'Close', { duration: 3000 });
              this.dialogRef.close(response.data);
            } else {
              this.snackBar.open(response.message || 'Failed to create category', 'Close', { duration: 5000 });
            }
          },
          error: (error) => {
            this.snackBar.open('Error creating category', 'Close', { duration: 5000 });
            console.error('Error creating category:', error);
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