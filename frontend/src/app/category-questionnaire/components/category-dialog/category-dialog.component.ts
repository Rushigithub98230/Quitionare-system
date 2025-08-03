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

import { Category, CreateCategoryDto, UpdateCategoryDto } from '../../models/category.model';
import { CategoryService } from '../../services/category.service';

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
          <mat-form-field appearance="outline">
            <mat-label>Name</mat-label>
            <input matInput formControlName="name" placeholder="Enter category name">
            <mat-error *ngIf="categoryForm.get('name')?.hasError('required')">
              Name is required
            </mat-error>
            <mat-error *ngIf="categoryForm.get('name')?.hasError('maxlength')">
              Name must be less than 100 characters
            </mat-error>
            <mat-error *ngIf="categoryForm.get('name')?.hasError('duplicate')">
              A category with this name already exists
            </mat-error>
            <mat-error *ngIf="categoryForm.get('name')?.hasError('duplicateInactive')">
              This category already exists and is currently inactive
            </mat-error>
            <mat-hint *ngIf="nameChecking" class="checking-hint">
              <mat-icon>hourglass_empty</mat-icon> Checking name availability...
            </mat-hint>
            <mat-hint *ngIf="!nameChecking && categoryForm.get('name')?.valid && !categoryForm.get('name')?.hasError('duplicate')" class="available-hint">
              <mat-icon>check_circle</mat-icon> Name is available
            </mat-hint>
          </mat-form-field>
        </div>

        <div class="form-row">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Description</mat-label>
            <textarea matInput formControlName="description" placeholder="Enter category description" rows="3"></textarea>
            <mat-error *ngIf="categoryForm.get('description')?.hasError('maxlength')">
              Description must be less than 500 characters
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
            <mat-error *ngIf="categoryForm.get('color')?.hasError('maxlength')">
              Color must be less than 100 characters
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
            <mat-error *ngIf="categoryForm.get('consultationDescription')?.hasError('maxlength')">
              Consultation description must be less than 500 characters
            </mat-error>
          </mat-form-field>
        </div>

        <div class="form-row">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Features</mat-label>
            <textarea matInput formControlName="features" placeholder="Enter features (comma separated)" rows="2"></textarea>
            <mat-error *ngIf="categoryForm.get('features')?.hasError('maxlength')">
              Features must be less than 1000 characters
            </mat-error>
          </mat-form-field>
        </div>

        <div class="form-row">
          <mat-form-field appearance="outline">
            <mat-label>Icon</mat-label>
            <input matInput formControlName="icon" placeholder="Enter icon name">
            <mat-error *ngIf="categoryForm.get('icon')?.hasError('maxlength')">
              Icon must be less than 100 characters
            </mat-error>
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
        <button mat-raised-button color="primary" type="submit" 
                [disabled]="categoryForm.invalid || loading || categoryForm.get('name')?.hasError('duplicate') || categoryForm.get('name')?.hasError('duplicateInactive')">
          {{ loading ? 'Saving...' : (data.mode === 'create' ? 'Create' : 'Update') }}
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
export class CategoryDialogComponent implements OnInit {
  categoryForm: FormGroup;
  loading = false;
  nameChecking = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<CategoryDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CategoryDialogData,
    private categoryService: CategoryService,
    private snackBar: MatSnackBar
  ) {
    this.categoryForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]],
      color: ['#007bff', [Validators.pattern(/^#[0-9A-F]{6}$/i), Validators.maxLength(100)]],
      basePrice: [0, [Validators.min(0)]],
      oneTimeConsultationDurationMinutes: [30, [Validators.min(1)]],
      consultationDescription: ['', [Validators.maxLength(500)]],
      features: ['', [Validators.maxLength(1000)]],
      icon: ['', [Validators.maxLength(100)]],
      isActive: [true],
      allowsFollowUpMessaging: [false],
      allowsMedicationDelivery: [false],
      isMostPopular: [false],
      isTrending: [false],
      requiresQuestionnaireAssessment: [false]
    });

    // Set up real-time name validation
    this.setupNameValidation();
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

  private setupNameValidation(): void {
    const nameControl = this.categoryForm.get('name');
    if (nameControl) {
      nameControl.valueChanges.pipe(
        debounceTime(500), // Wait 500ms after user stops typing
        distinctUntilChanged(), // Only emit if value has changed
        switchMap(name => {
          if (!name || name.length < 2) {
            this.nameChecking = false;
            return of(null);
          }
          
          console.log('Checking name:', name);
          this.nameChecking = true;
          return this.categoryService.checkNameExists(name).pipe(
            catchError((error) => {
              console.error('Error checking name:', error);
              this.nameChecking = false;
              return of(null);
            })
          );
        })
      ).subscribe(response => {
        this.nameChecking = false;
        
        console.log('Name validation response:', response);
        
        if (response && response.statusCode === 200 && response.data) {
          const exists = response.data.exists;
          const existsActive = response.data.existsActive;
          const existsInactive = response.data.existsInactive;
          
          console.log('Name exists:', exists, 'Active:', existsActive, 'Inactive:', existsInactive, 'Mode:', this.data.mode);
          
          if (exists && this.data.mode === 'create') {
            // For create mode, show error if name exists (active or inactive)
            console.log('Setting duplicate error for create mode');
            if (existsInactive) {
              // Special case: inactive category exists
              nameControl.setErrors({ ...nameControl.errors, duplicateInactive: true });
            } else {
              nameControl.setErrors({ ...nameControl.errors, duplicate: true });
            }
          } else if (exists && this.data.mode === 'edit' && this.data.category) {
            // For edit mode, only show error if name exists and it's not the current category
            if (nameControl.value !== this.data.category.name) {
              console.log('Setting duplicate error for edit mode - different name');
              if (existsInactive) {
                nameControl.setErrors({ ...nameControl.errors, duplicateInactive: true });
              } else {
                nameControl.setErrors({ ...nameControl.errors, duplicate: true });
              }
            } else {
              // Clear duplicate error if it's the same name
              console.log('Clearing duplicate error - same name in edit mode');
              const errors = { ...nameControl.errors };
              delete errors['duplicate'];
              delete errors['duplicateInactive'];
              nameControl.setErrors(Object.keys(errors).length > 0 ? errors : null);
            }
          } else {
            // Clear duplicate error if name doesn't exist
            console.log('Clearing duplicate error - name available');
            const errors = { ...nameControl.errors };
            delete errors['duplicate'];
            delete errors['duplicateInactive'];
            nameControl.setErrors(Object.keys(errors).length > 0 ? errors : null);
          }
        } else {
          console.log('Invalid response or error:', response);
        }
      });

      // Also add immediate validation on blur
      nameControl.valueChanges.subscribe(name => {
        if (name && name.length >= 2) {
          // Force a validation check when user stops typing
          setTimeout(() => {
            if (nameControl.value === name) {
              this.categoryService.checkNameExists(name).subscribe(response => {
                if (response && response.statusCode === 200 && response.data) {
                  const exists = response.data.exists;
                  const existsActive = response.data.existsActive;
                  const existsInactive = response.data.existsInactive;
                  if (exists && this.data.mode === 'create') {
                    if (existsInactive) {
                      nameControl.setErrors({ ...nameControl.errors, duplicateInactive: true });
                    } else {
                      nameControl.setErrors({ ...nameControl.errors, duplicate: true });
                    }
                  }
                }
              });
            }
          }, 1000);
        }
      });
    }
  }

  onSubmit(): void {
    if (this.categoryForm.valid && !this.categoryForm.get('name')?.hasError('duplicate') && !this.categoryForm.get('name')?.hasError('duplicateInactive')) {
      // Do a final validation check before submission
      const name = this.categoryForm.get('name')?.value;
      if (name && this.data.mode === 'create') {
        this.categoryService.checkNameExists(name).subscribe(response => {
          if (response && response.statusCode === 200 && response.data) {
            const exists = response.data.exists;
            const existsActive = response.data.existsActive;
            const existsInactive = response.data.existsInactive;
            if (exists) {
              this.snackBar.open('This category name already exists. Please choose a different name.', 'Close', { duration: 5000 });
              return;
            } else {
              // Proceed with submission
              this.submitForm();
            }
          } else {
            // Proceed with submission if validation fails
            this.submitForm();
          }
        });
      } else {
        // For edit mode or if no name, proceed directly
        this.submitForm();
      }
    } else if (this.categoryForm.get('name')?.hasError('duplicate') || this.categoryForm.get('name')?.hasError('duplicateInactive')) {
      const errorMessage = this.categoryForm.get('name')?.hasError('duplicateInactive') 
        ? 'This category already exists and is currently inactive. Please choose a different name.' 
        : 'Please choose a different category name. This name already exists.';
      this.snackBar.open(errorMessage, 'Close', { duration: 5000 });
    }
  }

  private submitForm(): void {
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

  onCancel(): void {
    this.dialogRef.close();
  }
} 