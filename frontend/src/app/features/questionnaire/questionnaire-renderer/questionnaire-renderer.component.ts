import { Component, Input, OnInit } from '@angular/core';
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
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar } from '@angular/material/snack-bar';

export interface Question {
  id: string;
  text: string;
  questionTypeId: number;
  order: number;
  isRequired: boolean;
  minLength?: number;
  maxLength?: number;
  minValue?: number;
  maxValue?: number;
  options?: QuestionOption[];
}

export interface QuestionOption {
  id: string;
  text: string;
  order: number;
}

export interface Questionnaire {
  id: string;
  title: string;
  description: string;
  categoryId: string;
  categoryName: string;
  version: string;
  isActive: boolean;
  questions: Question[];
}

@Component({
  selector: 'app-questionnaire-renderer',
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
    MatChipsModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  template: `
    <div class="renderer-container" *ngIf="questionnaire">
      <mat-card>
        <mat-card-header>
          <mat-card-title>{{ questionnaire.title }}</mat-card-title>
          <mat-card-subtitle>{{ questionnaire.description }}</mat-card-subtitle>
        </mat-card-header>
        
        <mat-card-content>
          <form [formGroup]="responseForm" (ngSubmit)="onSubmit()">
            <div class="question-container" *ngFor="let question of sortedQuestions; let i = index">
              <div class="question-header">
                <h3>Question {{ i + 1 }}</h3>
                <span class="required-indicator" *ngIf="question.isRequired">*</span>
              </div>
              
              <p class="question-text">{{ question.text }}</p>
              
              <!-- Text Input -->
              <div *ngIf="getQuestionTypeName(question.questionTypeId) === 'text'" class="question-input">
                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Your answer</mat-label>
                  <input matInput [formControlName]="'question_' + question.id" 
                         [placeholder]="question.isRequired ? 'Required' : 'Optional'"
                         [maxlength]="question.maxLength">
                  <mat-hint *ngIf="question.maxLength">Max {{ question.maxLength }} characters</mat-hint>
                  <mat-error *ngIf="getQuestionControl(question.id)?.hasError('required')">
                    This question is required
                  </mat-error>
                  <mat-error *ngIf="getQuestionControl(question.id)?.hasError('minlength')">
                    Minimum {{ question.minLength }} characters required
                  </mat-error>
                </mat-form-field>
              </div>
              
              <!-- Number Input -->
              <div *ngIf="getQuestionTypeName(question.questionTypeId) === 'number'" class="question-input">
                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Your answer</mat-label>
                  <input matInput type="number" [formControlName]="'question_' + question.id"
                         [placeholder]="question.isRequired ? 'Required' : 'Optional'"
                         [min]="question.minValue" [max]="question.maxValue">
                  <mat-error *ngIf="getQuestionControl(question.id)?.hasError('required')">
                    This question is required
                  </mat-error>
                  <mat-error *ngIf="getQuestionControl(question.id)?.hasError('min')">
                    Minimum value is {{ question.minValue }}
                  </mat-error>
                  <mat-error *ngIf="getQuestionControl(question.id)?.hasError('max')">
                    Maximum value is {{ question.maxValue }}
                  </mat-error>
                </mat-form-field>
              </div>
              
              <!-- Email Input -->
              <div *ngIf="getQuestionTypeName(question.questionTypeId) === 'email'" class="question-input">
                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Email address</mat-label>
                  <input matInput type="email" [formControlName]="'question_' + question.id"
                         placeholder="Enter your email">
                  <mat-error *ngIf="getQuestionControl(question.id)?.hasError('required')">
                    Email is required
                  </mat-error>
                  <mat-error *ngIf="getQuestionControl(question.id)?.hasError('email')">
                    Please enter a valid email address
                  </mat-error>
                </mat-form-field>
              </div>
              
              <!-- Phone Input -->
              <div *ngIf="getQuestionTypeName(question.questionTypeId) === 'phone'" class="question-input">
                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Phone number</mat-label>
                  <input matInput type="tel" [formControlName]="'question_' + question.id"
                         placeholder="Enter your phone number">
                  <mat-error *ngIf="getQuestionControl(question.id)?.hasError('required')">
                    Phone number is required
                  </mat-error>
                </mat-form-field>
              </div>
              
              <!-- Date Picker -->
              <div *ngIf="getQuestionTypeName(question.questionTypeId) === 'date'" class="question-input">
                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Choose a date</mat-label>
                  <input matInput [matDatepicker]="picker" [formControlName]="'question_' + question.id">
                  <mat-datepicker-toggle matIconSuffix [for]="picker"></mat-datepicker-toggle>
                  <mat-datepicker #picker></mat-datepicker>
                  <mat-error *ngIf="getQuestionControl(question.id)?.hasError('required')">
                    Date is required
                  </mat-error>
                </mat-form-field>
              </div>
              
              <!-- Radio Buttons (Single Choice) -->
              <div *ngIf="getQuestionTypeName(question.questionTypeId) === 'radio'" class="question-input">
                <mat-radio-group [formControlName]="'question_' + question.id" class="radio-group">
                  <mat-radio-button *ngFor="let option of question.options" 
                                   [value]="option.id" class="radio-button">
                    {{ option.text }}
                  </mat-radio-button>
                </mat-radio-group>
                <mat-error *ngIf="getQuestionControl(question.id)?.hasError('required')">
                  Please select an option
                </mat-error>
              </div>
              
              <!-- Checkboxes (Multiple Choice) -->
              <div *ngIf="getQuestionTypeName(question.questionTypeId) === 'checkbox'" class="question-input">
                <div [formGroupName]="'question_' + question.id" class="checkbox-group">
                  <mat-checkbox *ngFor="let option of question.options" 
                               [formControlName]="option.id" class="checkbox-option">
                    {{ option.text }}
                  </mat-checkbox>
                </div>
                <mat-error *ngIf="getQuestionControl(question.id)?.hasError('required')">
                  Please select at least one option
                </mat-error>
              </div>
              
              <!-- Dropdown -->
              <div *ngIf="getQuestionTypeName(question.questionTypeId) === 'select'" class="question-input">
                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Select an option</mat-label>
                  <mat-select [formControlName]="'question_' + question.id">
                    <mat-option *ngFor="let option of question.options" [value]="option.id">
                      {{ option.text }}
                    </mat-option>
                  </mat-select>
                  <mat-error *ngIf="getQuestionControl(question.id)?.hasError('required')">
                    Please select an option
                  </mat-error>
                </mat-form-field>
              </div>
              
              <!-- Rating -->
              <div *ngIf="getQuestionTypeName(question.questionTypeId) === 'rating'" class="question-input">
                <div class="rating-container">
                  <span class="rating-label">Rate from 1 to 5:</span>
                  <div class="rating-stars">
                    <button type="button" *ngFor="let star of [1,2,3,4,5]" 
                            (click)="setRating(question.id, star)"
                            [class.selected]="getQuestionControl(question.id)?.value === star"
                            class="star-button">
                      <mat-icon>{{ getQuestionControl(question.id)?.value >= star ? 'star' : 'star_border' }}</mat-icon>
                    </button>
                  </div>
                </div>
              </div>
              
              <!-- Slider -->
              <div *ngIf="getQuestionTypeName(question.questionTypeId) === 'slider'" class="question-input">
                <div class="slider-container">
                  <span class="slider-label">Value: {{ getQuestionControl(question.id)?.value || 0 }}</span>
                  <mat-slider [formControlName]="'question_' + question.id"
                             [min]="question.minValue || 0"
                             [max]="question.maxValue || 100"
                             [step]="1"
                             class="full-width">
                  </mat-slider>
                </div>
              </div>
              
              <!-- Yes/No -->
              <div *ngIf="getQuestionTypeName(question.questionTypeId) === 'yes_no'" class="question-input">
                <mat-radio-group [formControlName]="'question_' + question.id" class="radio-group">
                  <mat-radio-button value="true" class="radio-button">Yes</mat-radio-button>
                  <mat-radio-button value="false" class="radio-button">No</mat-radio-button>
                </mat-radio-group>
                <mat-error *ngIf="getQuestionControl(question.id)?.hasError('required')">
                  Please select Yes or No
                </mat-error>
              </div>
              
              <!-- File Upload -->
              <div *ngIf="getQuestionTypeName(question.questionTypeId) === 'file'" class="question-input">
                <div class="file-upload-container">
                  <input type="file" (change)="onFileSelected($event, question.id)" 
                         [accept]="getFileAcceptTypes()" style="display: none;" #fileInput>
                  <button type="button" mat-stroked-button (click)="fileInput.click()">
                    <mat-icon>upload</mat-icon>
                    Choose File
                  </button>
                  <span *ngIf="selectedFiles[question.id]" class="file-name">
                    {{ selectedFiles[question.id].name }}
                  </span>
                </div>
              </div>
              
              <!-- Long Text -->
              <div *ngIf="getQuestionTypeName(question.questionTypeId) === 'textarea'" class="question-input">
                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Your detailed answer</mat-label>
                  <textarea matInput [formControlName]="'question_' + question.id" 
                           rows="4" [placeholder]="question.isRequired ? 'Required' : 'Optional'"
                           [maxlength]="question.maxLength"></textarea>
                  <mat-hint *ngIf="question.maxLength">Max {{ question.maxLength }} characters</mat-hint>
                  <mat-error *ngIf="getQuestionControl(question.id)?.hasError('required')">
                    This question is required
                  </mat-error>
                  <mat-error *ngIf="getQuestionControl(question.id)?.hasError('minlength')">
                    Minimum {{ question.minLength }} characters required
                  </mat-error>
                </mat-form-field>
              </div>
            </div>
            
            <!-- Submit Button -->
            <div class="form-actions">
              <button type="submit" mat-raised-button color="primary" 
                      [disabled]="responseForm.invalid || isLoading" class="submit-button">
                {{ isLoading ? 'Submitting...' : 'Submit Questionnaire' }}
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .renderer-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 20px;
    }
    
    .question-container {
      margin-bottom: 40px;
      padding: 20px;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      background: #fafafa;
    }
    
    .question-header {
      display: flex;
      align-items: center;
      margin-bottom: 16px;
    }
    
    .question-header h3 {
      margin: 0;
      color: #333;
    }
    
    .required-indicator {
      color: #f44336;
      font-weight: bold;
      margin-left: 8px;
      font-size: 1.2em;
    }
    
    .question-text {
      font-size: 1.1em;
      color: #555;
      margin-bottom: 20px;
      line-height: 1.5;
    }
    
    .question-input {
      margin-top: 16px;
    }
    
    .full-width {
      width: 100%;
    }
    
    .radio-group {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }
    
    .radio-button {
      margin-bottom: 8px;
    }
    
    .checkbox-group {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }
    
    .checkbox-option {
      margin-bottom: 8px;
    }
    
    .rating-container {
      display: flex;
      align-items: center;
      gap: 16px;
    }
    
    .rating-label {
      font-weight: 500;
      color: #333;
    }
    
    .rating-stars {
      display: flex;
      gap: 4px;
    }
    
    .star-button {
      background: none;
      border: none;
      cursor: pointer;
      padding: 4px;
      border-radius: 4px;
      transition: background-color 0.2s;
    }
    
    .star-button:hover {
      background-color: #f0f0f0;
    }
    
    .star-button.selected {
      background-color: #e3f2fd;
    }
    
    .slider-container {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }
    
    .slider-label {
      font-weight: 500;
      color: #333;
    }
    
    .file-upload-container {
      display: flex;
      align-items: center;
      gap: 16px;
    }
    
    .file-name {
      color: #666;
      font-style: italic;
    }
    
    .form-actions {
      display: flex;
      justify-content: center;
      margin-top: 40px;
      padding-top: 20px;
      border-top: 1px solid #e0e0e0;
    }
    
    .submit-button {
      min-width: 200px;
      height: 48px;
      font-size: 1.1em;
    }
    
    @media (max-width: 600px) {
      .renderer-container {
        padding: 10px;
      }
      
      .question-container {
        padding: 16px;
      }
      
      .rating-container {
        flex-direction: column;
        align-items: flex-start;
      }
      
      .file-upload-container {
        flex-direction: column;
        align-items: flex-start;
      }
    }
  `]
})
export class QuestionnaireRendererComponent implements OnInit {
  @Input() questionnaire?: Questionnaire;
  
  responseForm: FormGroup;
  selectedFiles: { [key: string]: File } = {};
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private snackBar: MatSnackBar
  ) {
    this.responseForm = this.fb.group({});
  }

  ngOnInit(): void {
    if (this.questionnaire) {
      this.createForm();
    }
  }

  get sortedQuestions(): Question[] {
    return this.questionnaire?.questions?.sort((a, b) => a.order - b.order) || [];
  }

  createForm(): void {
    const formGroup: { [key: string]: any } = {};
    
    this.sortedQuestions.forEach(question => {
      const validators = [];
      
      if (question.isRequired) {
        validators.push(Validators.required);
      }
      
      const questionTypeName = this.getQuestionTypeName(question.questionTypeId.toString());
      
      if (questionTypeName === 'text' || questionTypeName === 'textarea') {
        if (question.minLength) {
          validators.push(Validators.minLength(question.minLength));
        }
        if (question.maxLength) {
          validators.push(Validators.maxLength(question.maxLength));
        }
      } else if (questionTypeName === 'number') {
        if (question.minValue !== undefined) {
          validators.push(Validators.min(question.minValue));
        }
        if (question.maxValue !== undefined) {
          validators.push(Validators.max(question.maxValue));
        }
      } else if (questionTypeName === 'email') {
        validators.push(Validators.email);
      } else if (questionTypeName === 'checkbox') {
        formGroup[`question_${question.id}`] = this.fb.group({});
        question.options?.forEach(option => {
          formGroup[`question_${question.id}`].addControl(option.id, this.fb.control(false));
        });
        return;
      }
      
      formGroup[`question_${question.id}`] = this.fb.control('', validators);
    });
    
    this.responseForm = this.fb.group(formGroup);
  }

  getQuestionTypeName(questionTypeId: string): string {
    // This is a simple mapping - in a real app, you'd want to fetch question types from backend
    const questionTypeMap: { [key: string]: string } = {
      '11111111-1111-1111-1111-111111111111': 'text',
      '22222222-2222-2222-2222-222222222222': 'textarea',
      '33333333-3333-3333-3333-333333333333': 'radio',
      '44444444-4444-4444-4444-444444444444': 'checkbox',
      '55555555-5555-5555-5555-555555555555': 'select',
      '66666666-6666-6666-6666-666666666666': 'multiselect',
      '77777777-7777-7777-7777-777777777777': 'number',
      '88888888-8888-8888-8888-888888888888': 'date',
      '99999999-9999-9999-9999-999999999999': 'email',
      'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa': 'phone',
      'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb': 'file',
      'cccccccc-cccc-cccc-cccc-cccccccccccc': 'rating',
      'dddddddd-dddd-dddd-dddd-dddddddddddd': 'slider',
      'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee': 'yes_no'
    };
    return questionTypeMap[questionTypeId] || 'text';
  }

  getQuestionControl(questionId: string): any {
    return this.responseForm.get(`question_${questionId}`);
  }

  setRating(questionId: string, rating: number): void {
    this.getQuestionControl(questionId).setValue(rating);
  }

  onFileSelected(event: any, questionId: string): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFiles[questionId] = file;
      this.getQuestionControl(questionId).setValue(file.name);
    }
  }

  getFileAcceptTypes(): string {
    return '.pdf,.doc,.docx,.txt,.jpg,.jpeg,.png';
  }

  onSubmit(): void {
    if (this.responseForm.valid && this.questionnaire) {
      this.isLoading = true;
      
      const formData = new FormData();
      formData.append('questionnaireId', this.questionnaire.id);
      
      // Add form responses
      Object.keys(this.responseForm.value).forEach(key => {
        const value = this.responseForm.value[key];
        if (value !== null && value !== undefined && value !== '') {
          formData.append(key, value);
        }
      });
      
      // Add files
      Object.keys(this.selectedFiles).forEach(questionId => {
        formData.append(`file_${questionId}`, this.selectedFiles[questionId]);
      });
      
      // TODO: Send to backend service
      console.log('Form data:', formData);
      
      setTimeout(() => {
        this.isLoading = false;
        this.snackBar.open('Questionnaire submitted successfully!', 'Close', { duration: 3000 });
      }, 2000);
    }
  }
} 