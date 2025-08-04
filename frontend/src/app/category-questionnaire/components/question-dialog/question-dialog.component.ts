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

import { Question, CreateQuestionDto, UpdateQuestionDto, QuestionType, QuestionOption } from '../../models/question.model';

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
  templateUrl: './question-dialog.component.html',
  styleUrls: ['./question-dialog.component.scss']
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
    // Calculate the next available display order
    const existingOrders = this.optionsArray.controls.map(control => control.get('displayOrder')?.value || 0);
    const nextOrder = existingOrders.length > 0 ? Math.max(...existingOrders) + 1 : 1;
    
    const optionGroup = this.fb.group({
      optionText: [option?.optionText || '', [Validators.required]],
      optionValue: [option?.optionValue || '', [Validators.required]],
      displayOrder: [option?.displayOrder || nextOrder],
      isCorrect: [option?.isCorrect || false],
      isActive: [option?.isActive !== false]
    });
    this.optionsArray.push(optionGroup);
    
    // Add comprehensive validation
    this.validateAll();
  }

  removeOption(index: number): void {
    if (this.optionsArray.length > 1) {
      this.optionsArray.removeAt(index);
      this.validateAll();
    }
  }

  showOptionsSection(): boolean {
    const questionTypeId = this.questionForm.get('questionTypeId')?.value;
    if (!questionTypeId || !this.data.questionTypes || this.data.questionTypes.length === 0) return false;
    
    const questionType = this.data.questionTypes.find(t => t.id === questionTypeId);
    if (!questionType) return false;
    
    const typeName = questionType.typeName?.toLowerCase() || '';
    
    // Show options for question types that need options
    return typeName.includes('radio') || 
           typeName.includes('checkbox') || 
           typeName.includes('select') ||
           typeName.includes('multiselect') || 
           typeName.includes('choice') ||
           typeName.includes('yes/no') ||
           typeName.includes('boolean') ||
           typeName.includes('yes or no') || false;
  }

  onQuestionTypeChange(): void {
    const questionTypeId = this.questionForm.get('questionTypeId')?.value;
    const questionType = this.data.questionTypes.find(t => t.id === questionTypeId);
    const typeName = questionType?.typeName?.toLowerCase() || '';

    // Clear options when switching to question types that don't need options
    if (!this.showOptionsSection()) {
      this.optionsArray.clear();
    } else {
      // For Yes/No questions, automatically add the fixed options
      if (typeName.includes('yes/no') || typeName.includes('boolean') || typeName.includes('yes or no')) {
        this.optionsArray.clear();
        // Add "Yes" option
        this.addOption({
          id: '', // Will be set by backend
          questionId: '', // Will be set by backend
          optionText: 'Yes',
          optionValue: 'yes',
          displayOrder: 1,
          isCorrect: false,
          isActive: true,
          createdDate: new Date().toISOString(),
          updatedDate: new Date().toISOString()
        });
        // Add "No" option
        this.addOption({
          id: '', // Will be set by backend
          questionId: '', // Will be set by backend
          optionText: 'No',
          optionValue: 'no',
          displayOrder: 2,
          isCorrect: false,
          isActive: true,
          createdDate: new Date().toISOString(),
          updatedDate: new Date().toISOString()
        });
      } else if (this.optionsArray.length === 0) {
        // Add a default option for other question types that need options
        this.addOption();
      } else {
        // Validate existing options when question type changes
        this.validateAll();
      }
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

  validateOptionUniqueness(): void {
    const options = this.optionsArray.controls;
    const optionValues = options.map(control => control.get('optionValue')?.value?.toLowerCase().trim()).filter(v => v);
    const uniqueValues = [...new Set(optionValues)];
    
    if (optionValues.length !== uniqueValues.length) {
      // Mark all options as invalid
      options.forEach(control => {
        const optionValueControl = control.get('optionValue');
        if (optionValueControl) {
          optionValueControl.setErrors({ duplicate: true });
        }
      });
    } else {
      // Clear errors
      options.forEach(control => {
        const optionValueControl = control.get('optionValue');
        if (optionValueControl && optionValueControl.hasError('duplicate')) {
          optionValueControl.setErrors(null);
        }
      });
    }

    // Validate radio question has only one correct answer
    const questionTypeId = this.questionForm.get('questionTypeId')?.value;
    if (questionTypeId) {
      const questionType = this.data.questionTypes.find(t => t.id === questionTypeId);
      if (questionType?.typeName?.toLowerCase() === 'radio') {
        const correctOptions = options.filter(control => control.get('isCorrect')?.value).length;
        if (correctOptions > 1) {
          options.forEach(control => {
            const isCorrectControl = control.get('isCorrect');
            if (isCorrectControl && isCorrectControl.value) {
              isCorrectControl.setErrors({ multipleCorrect: true });
            }
          });
        } else {
          options.forEach(control => {
            const isCorrectControl = control.get('isCorrect');
            if (isCorrectControl && isCorrectControl.hasError('multipleCorrect')) {
              isCorrectControl.setErrors(null);
            }
          });
        }
      }
    }
  }

  validateOptionOrders(): void {
    const options = this.optionsArray.controls;
    const totalOptions = options.length;
    
    if (totalOptions === 0) return;
    
    // Get all display orders
    const displayOrders = options.map(control => control.get('displayOrder')?.value || 0);
    const providedOrders = displayOrders.filter(order => order > 0);
    const uniqueOrders = [...new Set(providedOrders)];
    
    // Check for duplicate orders
    if (providedOrders.length !== uniqueOrders.length) {
      const duplicateOrders = providedOrders.filter((order, index) => providedOrders.indexOf(order) !== index);
      options.forEach(control => {
        const displayOrderControl = control.get('displayOrder');
        if (displayOrderControl && duplicateOrders.includes(displayOrderControl.value)) {
          displayOrderControl.setErrors({ duplicate: true });
        }
      });
      return;
    }
    
    // Check if orders are within valid range (1 to totalOptions)
    const invalidOrders = providedOrders.filter(order => order < 1 || order > totalOptions);
    if (invalidOrders.length > 0) {
      options.forEach(control => {
        const displayOrderControl = control.get('displayOrder');
        if (displayOrderControl && invalidOrders.includes(displayOrderControl.value)) {
          displayOrderControl.setErrors({ invalidRange: true });
        }
      });
      return;
    }
    
    // Check if all orders from 1 to totalOptions are provided
    const expectedOrders = Array.from({length: totalOptions}, (_, i) => i + 1);
    const missingOrders = expectedOrders.filter(order => !providedOrders.includes(order));
    if (missingOrders.length > 0) {
      options.forEach(control => {
        const displayOrderControl = control.get('displayOrder');
        if (displayOrderControl && displayOrderControl.value === 0) {
          displayOrderControl.setErrors({ missing: true });
        }
      });
      return;
    }
    
    // Clear all errors if validation passes
    options.forEach(control => {
      const displayOrderControl = control.get('displayOrder');
      if (displayOrderControl) {
        displayOrderControl.setErrors(null);
      }
    });
  }

  validateQuestionText(): void {
    const questionTextControl = this.questionForm.get('questionText');
    if (questionTextControl) {
      const value = questionTextControl.value;
      if (value && value.length > 1000) {
        questionTextControl.setErrors({ maxLength: true });
      } else if (questionTextControl.hasError('maxLength')) {
        questionTextControl.setErrors(null);
      }
    }
  }

  validateMinMaxValues(): void {
    const minValueControl = this.questionForm.get('minValue');
    const maxValueControl = this.questionForm.get('maxValue');
    
    if (minValueControl && maxValueControl) {
      const minValue = minValueControl.value;
      const maxValue = maxValueControl.value;
      
      if (minValue !== null && maxValue !== null && minValue > maxValue) {
        minValueControl.setErrors({ invalidRange: true });
        maxValueControl.setErrors({ invalidRange: true });
      } else {
        if (minValueControl.hasError('invalidRange')) {
          minValueControl.setErrors(null);
        }
        if (maxValueControl.hasError('invalidRange')) {
          maxValueControl.setErrors(null);
        }
      }
    }
  }

  validateMinMaxLength(): void {
    const minLengthControl = this.questionForm.get('minLength');
    const maxLengthControl = this.questionForm.get('maxLength');
    
    if (minLengthControl && maxLengthControl) {
      const minLength = minLengthControl.value;
      const maxLength = maxLengthControl.value;
      
      if (minLength !== null && maxLength !== null && minLength > maxLength) {
        minLengthControl.setErrors({ invalidRange: true });
        maxLengthControl.setErrors({ invalidRange: true });
      } else {
        if (minLengthControl.hasError('invalidRange')) {
          minLengthControl.setErrors(null);
        }
        if (maxLengthControl.hasError('invalidRange')) {
          maxLengthControl.setErrors(null);
        }
      }
    }
  }

  validateOptionTextLength(): void {
    const options = this.optionsArray.controls;
    options.forEach(control => {
      const optionTextControl = control.get('optionText');
      if (optionTextControl) {
        const value = optionTextControl.value;
        if (value && value.length > 500) {
          optionTextControl.setErrors({ maxLength: true });
        } else if (optionTextControl.hasError('maxLength')) {
          optionTextControl.setErrors(null);
        }
      }
    });
  }

  validateQuestionType(): void {
    const questionTypeId = this.questionForm.get('questionTypeId')?.value;
    if (questionTypeId) {
      const questionType = this.data.questionTypes.find(t => t.id === questionTypeId);
      if (questionType && !questionType.isActive) {
        this.questionForm.get('questionTypeId')?.setErrors({ inactiveType: true });
      } else if (this.questionForm.get('questionTypeId')?.hasError('inactiveType')) {
        this.questionForm.get('questionTypeId')?.setErrors(null);
      }
    }
  }

  validateAll(): void {
    this.validateQuestionText();
    this.validateMinMaxValues();
    this.validateMinMaxLength();
    this.validateOptionTextLength();
    this.validateQuestionType();
    this.validateOptionUniqueness();
    this.validateOptionOrders();
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  isYesNoQuestion(): boolean {
    const questionTypeId = this.questionForm.get('questionTypeId')?.value;
    if (!questionTypeId) return false;

    const questionType = this.data.questionTypes.find(t => t.id === questionTypeId);
    if (!questionType) return false;

    const typeName = questionType.typeName?.toLowerCase() || '';
    return typeName.includes('yes/no') || typeName.includes('boolean') || typeName.includes('yes or no');
  }
} 