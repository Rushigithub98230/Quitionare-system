import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { QuestionService } from '../../../core/services/question.service';
import { UserQuestionResponseService } from '../../../core/services/user-question-response.service';
import { FileUploadService } from '../../../core/services/file-upload.service';
import { forkJoin, of } from 'rxjs';
import { switchMap, catchError } from 'rxjs/operators';
import { Observable } from 'rxjs';

import { CommonModule } from '@angular/common';
import { FormArray, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatStepperModule } from '@angular/material/stepper';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { MatSliderModule } from '@angular/material/slider';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, DateAdapter, NativeDateAdapter } from '@angular/material/core';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialogModule } from '@angular/material/dialog';

import { Question, QuestionType } from '../../../core/models/question.model';
import { Questionnaire } from '../../../core/models/questionnaire.model';

interface PreviewResponse {
  questionId: string;
  questionText: string;
  questionType: string;
  response: any;
  isRequired: boolean;
  isValid: boolean;
  validationMessage?: string;
  uploadedFileData?: {
    fileName: string;
    fileUrl: string;
    fileSize: number;
  };
}

@Component({
  selector: 'app-questionnaire-preview',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MatDialogModule,
    MatStepperModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatRadioModule,
    MatSliderModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatCardModule,
    MatChipsModule,
    MatProgressBarModule,
    MatDividerModule,
    MatProgressSpinnerModule
  ],
  providers: [
    { provide: DateAdapter, useClass: NativeDateAdapter }
  ],
  template: `
    <div class="preview-container">
      <div class="preview-header">
        <h2 mat-dialog-title>
          <mat-icon>{{ data.isAdminSolve ? 'assignment' : 'preview' }}</mat-icon>
          {{ data.isAdminSolve ? 'Solve as Admin' : 'Preview & Test' }}: {{ questionnaire?.title }}
        </h2>
        <button mat-icon-button (click)="closeDialog()" class="close-button">
          <mat-icon>close</mat-icon>
        </button>
      </div>

      <mat-dialog-content class="preview-content">
        <div class="questionnaire-info">
          <mat-card>
            <mat-card-content>
              <p><strong>Description:</strong> {{ questionnaire?.description || 'No description' }}</p>
              <p><strong>Category:</strong> {{ getCategoryName(questionnaire?.categoryId) }}</p>
              <p><strong>Total Questions:</strong> {{ questions.length }}</p>
              <p><strong>Required Questions:</strong> {{ getRequiredCount() }}</p>
            </mat-card-content>
          </mat-card>
        </div>

        <div *ngIf="loading" class="loading">
          <mat-spinner></mat-spinner>
          <p>Loading questionnaire...</p>
        </div>

        <div *ngIf="!loading && questions.length === 0" class="no-questions">
          <mat-icon>quiz</mat-icon>
          <p>No questions found for this questionnaire.</p>
        </div>

        <div *ngIf="!loading && questions.length > 0" class="stepper-container">
          <mat-stepper #stepper [linear]="false" class="preview-stepper">
            <mat-step *ngFor="let question of questions; let i = index" 
                      [label]="'Question ' + (i + 1)"
                      [state]="getStepState(i)">
              
              <div class="question-step">
                <div class="question-header">
                  <div class="question-number">
                    <span>{{ i + 1 }}</span>
                  </div>
                  <div class="question-content">
                    <h3>{{ question.questionText }}</h3>
                    <div class="question-meta">
                      <mat-chip-set>
                        <mat-chip [color]="question.isRequired ? 'warn' : 'primary'" selected>
                          {{ question.isRequired ? 'Required' : 'Optional' }}
                        </mat-chip>
                        <mat-chip color="accent" selected>
                          {{ getQuestionTypeName(question.questionTypeId) }} ({{ question.questionTypeName }})
                        </mat-chip>
                      </mat-chip-set>
                    </div>
                    <p *ngIf="question.helpText" class="help-text">{{ question.helpText }}</p>
                  </div>
                </div>

                <div class="question-form">
                  <form [formGroup]="getQuestionForm(i)">
                    <!-- Text Input -->
                    <mat-form-field *ngIf="question.questionTypeName === 'text'" appearance="outline" class="full-width">
                      <mat-label>{{ question.placeholder || 'Enter your answer' }}</mat-label>
                      <input matInput 
                             [formControlName]="'response'"
                             [placeholder]="question.placeholder || ''"
                             [maxlength]="question.maxLength && question.maxLength > 0 ? question.maxLength : null">
                      <mat-hint *ngIf="question.maxLength && question.maxLength > 0">
                        {{ getQuestionForm(i).get('response')?.value?.length || 0 }}/{{ question.maxLength }}
                      </mat-hint>
                    </mat-form-field>

                    <!-- Number Input -->
                    <mat-form-field *ngIf="question.questionTypeName === 'number'" appearance="outline" class="full-width">
                      <mat-label>{{ question.placeholder || 'Enter a number' }}</mat-label>
                      <input matInput 
                             type="number"
                             [formControlName]="'response'"
                             [placeholder]="question.placeholder || ''"
                             [min]="question.minValue && question.minValue > 0 ? question.minValue : null"
                             [max]="question.maxValue && question.maxValue > 0 ? question.maxValue : null">
                      <mat-hint *ngIf="(question.minValue && question.minValue > 0) || (question.maxValue && question.maxValue > 0)">
                        {{ question.minValue && question.minValue > 0 ? 'Min: ' + question.minValue : '' }}
                        {{ question.maxValue && question.maxValue > 0 ? 'Max: ' + question.maxValue : '' }}
                      </mat-hint>
                    </mat-form-field>

                    <!-- Email Input -->
                    <mat-form-field *ngIf="question.questionTypeName === 'email'" appearance="outline" class="full-width">
                      <mat-label>{{ question.placeholder || 'Enter email address' }}</mat-label>
                      <input matInput 
                             type="email"
                             [formControlName]="'response'"
                             [placeholder]="question.placeholder || ''">
                    </mat-form-field>

                                                              <!-- Date Input -->
                      <mat-form-field *ngIf="question.questionTypeName === 'date'" appearance="outline" class="full-width">
                        <mat-label>{{ question.placeholder || 'Select date' }}</mat-label>
                        <input matInput 
                               [matDatepicker]="picker"
                               [formControlName]="'response'"
                               [placeholder]="question.placeholder || ''"
                               readonly>
                        <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
                        <mat-datepicker #picker [touchUi]="false"></mat-datepicker>
                      </mat-form-field>
                    
                                         <!-- Debug info for date -->
                     <div *ngIf="question.questionTypeName === 'date'" style="color: red; font-size: 12px;">
                       Debug: questionTypeName = "{{ question.questionTypeName }}"
                     </div>

                     <!-- Debug info for file -->
                     <div *ngIf="question.questionTypeName === 'file'" style="color: red; font-size: 12px;">
                       Debug: questionTypeName = "{{ question.questionTypeName }}"
                     </div>

                    <!-- File Upload -->
                    <div *ngIf="question.questionTypeName === 'file'" class="file-upload-group">
                      <div class="file-input-container">
                        <label class="file-input-label">{{ question.placeholder || 'Choose file' }}</label>
                        <input type="file"
                               class="file-input"
                               [formControlName]="'response'"
                               (change)="onFileSelected($event, i)"
                               accept="*/*">
                      </div>
                      <div *ngIf="getQuestionForm(i).get('response')?.value" class="file-info">
                        <mat-icon>attach_file</mat-icon>
                        <span>{{ getSelectedFileName(i) }}</span>
                      </div>
                    </div>

                    <!-- Phone Input -->
                    <mat-form-field *ngIf="question.questionTypeName === 'phone'" appearance="outline" class="full-width">
                      <mat-label>{{ question.placeholder || 'Enter phone number' }}</mat-label>
                      <input matInput 
                             type="tel"
                             [formControlName]="'response'"
                             [placeholder]="question.placeholder || ''">
                    </mat-form-field>

                    <!-- Radio Buttons -->
                    <div *ngIf="question.questionTypeName === 'radio'" class="radio-group">
                      <mat-radio-group [formControlName]="'response'">
                        <mat-radio-button *ngFor="let option of question.options" 
                                         [value]="option.optionValue"
                                         class="radio-option">
                          {{ option.optionText }}
                        </mat-radio-button>
                      </mat-radio-group>
                    </div>

                                         <!-- Checkboxes -->
                     <div *ngIf="question.questionTypeName === 'checkbox'" class="checkbox-group">
                       <div *ngFor="let option of question.options" class="checkbox-option">
                         <mat-checkbox [value]="option.id"
                                      (change)="onCheckboxChange($event, i, option.id)">
                           {{ option.optionText }}
                         </mat-checkbox>
                       </div>
                     </div>

                    <!-- Dropdown/Select -->
                    <mat-form-field *ngIf="question.questionTypeName === 'select'" appearance="outline" class="full-width">
                      <mat-label>{{ question.placeholder || 'Select an option' }}</mat-label>
                      <mat-select [formControlName]="'response'">
                        <mat-option *ngFor="let option of question.options" [value]="option.id">
                          {{ option.optionText }}
                        </mat-option>
                      </mat-select>
                    </mat-form-field>
                    
                                         <!-- Debug info for select -->
                     <div *ngIf="question.questionTypeName === 'select'" style="color: red; font-size: 12px;">
                       Debug: questionTypeName = "{{ question.questionTypeName }}", options count = {{ question.options && question.options.length || 0 }}
                     </div>

                    <!-- Multi-Select -->
                    <mat-form-field *ngIf="question.questionTypeName === 'multiselect'" appearance="outline" class="full-width">
                      <mat-label>{{ question.placeholder || 'Select options' }}</mat-label>
                      <mat-select [formControlName]="'response'" multiple>
                        <mat-option *ngFor="let option of question.options" [value]="option.id">
                          {{ option.optionText }}
                        </mat-option>
                      </mat-select>
                    </mat-form-field>

                    <!-- Rating Scale -->
                    <div *ngIf="question.questionTypeName === 'rating'" class="rating-group">
                      <mat-form-field appearance="outline" class="full-width">
                        <mat-label>{{ question.placeholder || 'Select rating' }}</mat-label>
                        <mat-select [formControlName]="'response'">
                          <mat-option *ngFor="let i of getRatingOptions(question)" [value]="i">
                            {{ i }} {{ i === 1 ? 'star' : 'stars' }}
                          </mat-option>
                        </mat-select>
                      </mat-form-field>
                    </div>

                    <!-- Slider -->
                    <div *ngIf="question.questionTypeName === 'slider'" class="scale-group">
                      <mat-slider [formControlName]="'response'"
                                 [min]="question.minValue && question.minValue > 0 ? question.minValue : 1"
                                 [max]="question.maxValue && question.maxValue > 0 ? question.maxValue : 10"
                                 [step]="1"
                                 class="full-width">
                      </mat-slider>
                      <div class="scale-labels">
                        <span>{{ question.minValue && question.minValue > 0 ? question.minValue : 1 }}</span>
                        <span>{{ question.maxValue && question.maxValue > 0 ? question.maxValue : 10 }}</span>
                      </div>
                      <p class="scale-value">Selected: {{ getQuestionForm(i).get('response')?.value || 'None' }}</p>
                    </div>

                    <!-- Yes/No -->
                    <div *ngIf="question.questionTypeName === 'yes_no'" class="radio-group">
                      <mat-radio-group [formControlName]="'response'">
                        <mat-radio-button value="yes" class="radio-option">Yes</mat-radio-button>
                        <mat-radio-button value="no" class="radio-option">No</mat-radio-button>
                      </mat-radio-group>
                    </div>

                    <!-- Textarea -->
                    <mat-form-field *ngIf="question.questionTypeName === 'textarea'" appearance="outline" class="full-width">
                      <mat-label>{{ question.placeholder || 'Enter your answer' }}</mat-label>
                      <textarea matInput 
                                [formControlName]="'response'"
                                [placeholder]="question.placeholder || ''"
                                [maxlength]="question.maxLength && question.maxLength > 0 ? question.maxLength : null"
                                rows="4"></textarea>
                      <mat-hint *ngIf="question.maxLength && question.maxLength > 0">
                        {{ getQuestionForm(i).get('response')?.value?.length || 0 }}/{{ question.maxLength }}
                      </mat-hint>
                    </mat-form-field>

                    <!-- Validation Messages -->
                    <div *ngIf="getQuestionForm(i).get('response')?.invalid && getQuestionForm(i).get('response')?.touched" 
                         class="validation-error">
                      <mat-icon>error</mat-icon>
                      <span>{{ getValidationMessage(i) }}</span>
                    </div>
                  </form>
                </div>
              </div>

              <div class="step-actions">
                <button mat-button (click)="stepper.previous()" *ngIf="i > 0">
                  <mat-icon>arrow_back</mat-icon>
                  Previous
                </button>
                <button mat-raised-button 
                        color="primary" 
                        (click)="stepper.next()" 
                        *ngIf="i < questions.length - 1"
                        [disabled]="getQuestionForm(i).invalid">
                  <mat-icon>arrow_forward</mat-icon>
                  Next
                </button>
                <!-- Response Toggle for Database Saving -->
                <div *ngIf="i === questions.length - 1" class="response-toggle-section">
                  <mat-checkbox 
                    [(ngModel)]="saveToDatabase" 
                    color="primary"
                    class="save-toggle">
                    <mat-icon>save</mat-icon>
                    Save Response to Database
                  </mat-checkbox>
                  <p class="toggle-hint" *ngIf="saveToDatabase">
                    <mat-icon>info</mat-icon>
                    Your response will be saved to the database for review.
                  </p>
                </div>

                <button mat-raised-button 
                        color="accent" 
                        (click)="submitPreview()" 
                        *ngIf="i === questions.length - 1"
                        [disabled]="!isFormValid()">
                  <mat-icon>send</mat-icon>
                  {{ data.isAdminSolve ? 'Submit Admin Response' : (saveToDatabase ? 'Submit & Save' : 'Submit Preview') }}
                </button>
              </div>
            </mat-step>
          </mat-stepper>
        </div>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button (click)="closeDialog()">Cancel</button>
      </mat-dialog-actions>
    </div>
  `,
  styles: [`
    .preview-container {
      max-width: 800px;
      max-height: 90vh;
      overflow: hidden;
      display: flex;
      flex-direction: column;
    }

    .preview-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 16px 24px;
      background: #f5f5f5;
      border-bottom: 1px solid #e0e0e0;
    }

    .preview-header h2 {
      display: flex;
      align-items: center;
      gap: 8px;
      margin: 0;
    }

    .close-button {
      position: absolute;
      top: 8px;
      right: 8px;
    }

    .preview-content {
      flex: 1;
      overflow-y: auto;
      padding: 24px;
    }

    .questionnaire-info {
      margin-bottom: 24px;
    }

    .loading, .no-questions {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: 48px;
      text-align: center;
      color: #666;
    }

    .loading mat-spinner, .no-questions mat-icon {
      margin-bottom: 16px;
    }

    .no-questions mat-icon {
      font-size: 48px;
      width: 48px;
      height: 48px;
      color: #ccc;
    }

    .stepper-container {
      margin-top: 24px;
    }

    .preview-stepper {
      background: transparent;
    }

    .question-step {
      padding: 24px 0;
    }

    .question-header {
      display: flex;
      gap: 16px;
      margin-bottom: 24px;
    }

    .question-number {
      background: #3f51b5;
      color: white;
      border-radius: 50%;
      width: 40px;
      height: 40px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: bold;
      flex-shrink: 0;
    }

    .question-content h3 {
      margin: 0 0 12px 0;
      font-size: 18px;
      font-weight: 500;
    }

    .question-meta {
      margin-bottom: 12px;
    }

    .help-text {
      color: #666;
      font-style: italic;
      margin: 8px 0 0 0;
    }

    .question-form {
      margin-bottom: 24px;
    }

    .full-width {
      width: 100%;
    }

    .radio-group, .checkbox-group {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }

    .radio-option, .checkbox-option {
      padding: 8px 0;
    }

    .scale-group {
      padding: 16px 0;
    }

    .scale-labels {
      display: flex;
      justify-content: space-between;
      margin-top: 8px;
      color: #666;
    }

    .scale-value {
      text-align: center;
      margin-top: 8px;
      font-weight: 500;
      color: #3f51b5;
    }

    .file-upload-group {
      margin-bottom: 16px;
    }

    .file-info {
      display: flex;
      align-items: center;
      gap: 8px;
      margin-top: 8px;
      padding: 8px;
      background-color: #f5f5f5;
      border-radius: 4px;
      color: #666;
    }

    .file-info mat-icon {
      color: #3f51b5;
    }

    .file-input-container {
      margin-bottom: 16px;
    }

    .file-input-label {
      display: block;
      margin-bottom: 8px;
      font-weight: 500;
      color: rgba(0, 0, 0, 0.87);
    }

    .file-input {
      width: 100%;
      padding: 8px;
      border: 1px solid #ccc;
      border-radius: 4px;
      background-color: white;
      cursor: pointer;
    }

    .file-input:hover {
      border-color: #3f51b5;
    }

    /* Date picker specific styles */
    .mat-datepicker-content {
      z-index: 1000 !important;
    }

    .mat-datepicker-popup {
      z-index: 1000 !important;
    }

    .mat-calendar {
      z-index: 1000 !important;
    }

    /* Ensure date picker appears above dialog */
    .cdk-overlay-pane {
      z-index: 1001 !important;
    }

    .mat-datepicker-content {
      z-index: 1001 !important;
    }

    .rating-group {
      margin-bottom: 16px;
    }

    .validation-error {
      display: flex;
      align-items: center;
      gap: 8px;
      color: #f44336;
      margin-top: 8px;
      font-size: 12px;
    }

    .step-actions {
      display: flex;
      justify-content: space-between;
      margin-top: 24px;
      padding-top: 16px;
      border-top: 1px solid #e0e0e0;
    }

    mat-dialog-actions {
      padding: 16px 24px;
      background: #f5f5f5;
      border-top: 1px solid #e0e0e0;
    }

    .mat-mdc-dialog-content {
      max-height: 70vh;
    }

    .response-toggle-section {
      margin-bottom: 16px;
      padding: 16px;
      background-color: var(--surface-color, #f5f5f5);
      border-radius: 8px;
      border: 1px solid var(--border-color, #e0e0e0);
    }

    .save-toggle {
      display: flex;
      align-items: center;
      gap: 8px;
      font-weight: 500;
    }

    .toggle-hint {
      margin: 8px 0 0 0;
      font-size: 0.9rem;
      color: var(--text-secondary, #666);
      display: flex;
      align-items: center;
      gap: 4px;
    }
  `]
})
export class QuestionnairePreviewComponent implements OnInit {
  questionnaire?: Questionnaire;
  questions: Question[] = [];
  questionTypes: QuestionType[] = [];
  loading = false;
  questionForms: FormGroup[] = [];
  responses: PreviewResponse[] = [];
  saveToDatabase = false; // Toggle for saving responses to database

  constructor(
    private dialogRef: MatDialogRef<QuestionnairePreviewComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private formBuilder: FormBuilder,
    private questionService: QuestionService,
    private responseService: UserQuestionResponseService,
    private snackBar: MatSnackBar,
    private fileUploadService: FileUploadService
  ) {
    this.questionnaire = data.questionnaire;
  }

  ngOnInit(): void {
    this.loadQuestionTypes();
    this.loadQuestions();
    
    // If this is an admin solve, automatically enable save to database
    if (this.data.isAdminSolve) {
      this.saveToDatabase = true;
      console.log('Admin solve mode enabled - saveToDatabase:', this.saveToDatabase);
    }
  }

  loadQuestionTypes(): void {
    this.questionService.getQuestionTypes().subscribe({
      next: (types) => {
        this.questionTypes = types || [];
      },
      error: (error) => {
        console.error('Error loading question types:', error);
        this.snackBar.open('Error loading question types', 'Close', { duration: 3000 });
        this.questionTypes = [];
      }
    });
  }

  loadQuestions(): void {
    if (!this.questionnaire?.id) {
      this.snackBar.open('No questionnaire selected', 'Close', { duration: 3000 });
      return;
    }

    this.loading = true;
    this.questionService.getQuestionsByQuestionnaireId(this.questionnaire.id).subscribe({
      next: (response: any) => {
        let questions: any[] = [];
        if (response && typeof response === 'object' && !Array.isArray(response)) {
          if (response.data && Array.isArray(response.data)) {
            questions = response.data;
          } else if (response.questions && Array.isArray(response.questions)) {
            questions = response.questions;
          } else {
            questions = [];
          }
        } else if (Array.isArray(response)) {
          questions = response;
        } else {
          questions = [];
        }
        
                 this.questions = questions.sort((a, b) => a.displayOrder - b.displayOrder);
         console.log('Loaded questions:', this.questions);
         
         // Debug: Check options for each question
         this.questions.forEach((question, index) => {
           console.log(`Question ${index + 1} (${question.questionTypeName}):`, {
             questionText: question.questionText,
             options: question.options,
             optionsCount: question.options ? question.options.length : 0
           });
         });
         
         this.initializeForms();
         this.loading = false;
      },
      error: (error) => {
        console.error('Error loading questions:', error);
        this.snackBar.open('Error loading questions', 'Close', { duration: 3000 });
        this.loading = false;
        this.questions = [];
      }
    });
  }

  initializeForms(): void {
    this.questionForms = this.questions.map(question => {
      const validators = [];
      
      if (question.isRequired) {
        validators.push(Validators.required);
      }

      // Add type-specific validators
      switch (question.questionTypeName) {
        case 'email':
          validators.push(Validators.email);
          break;
        case 'number':
          if (question.minValue !== undefined && question.minValue !== null && question.minValue > 0) {
            validators.push(Validators.min(question.minValue));
          }
          if (question.maxValue !== undefined && question.maxValue !== null && question.maxValue > 0) {
            validators.push(Validators.max(question.maxValue));
          }
          break;
        case 'text':
        case 'textarea':
        case 'phone':
          if (question.minLength !== undefined && question.minLength !== null && question.minLength > 0) {
            validators.push(Validators.minLength(question.minLength));
          }
          if (question.maxLength !== undefined && question.maxLength !== null && question.maxLength > 0) {
            validators.push(Validators.maxLength(question.maxLength));
          }
          break;
        case 'file':
          // File validation can be added here if needed
          break;
      }

      return this.formBuilder.group({
        response: ['', validators]
      });
    });
  }

  getQuestionForm(index: number): FormGroup {
    return this.questionForms[index];
  }

  getStepState(index: number): string {
    const form = this.getQuestionForm(index);
    if (form.valid && form.get('response')?.value) {
      return 'done';
    } else if (form.get('response')?.touched && form.invalid) {
      return 'error';
    }
    return 'number';
  }

  getRequiredCount(): number {
    return this.questions.filter(q => q.isRequired).length;
  }

  getQuestionTypeName(typeId: string): string {
    const type = this.questionTypes.find(t => t.id === typeId);
    return type?.displayName || 'Unknown';
  }

  getCategoryName(categoryId?: string): string {
    // This would need to be implemented based on your category service
    return categoryId || 'Unknown Category';
  }

  getValidationMessage(index: number): string {
    const form = this.getQuestionForm(index);
    const control = form.get('response');
    const question = this.questions[index];

    if (control?.hasError('required')) {
      return 'This field is required.';
    }

    if (control?.hasError('email')) {
      return 'Please enter a valid email address.';
    }

    if (control?.hasError('minlength')) {
      return `Minimum length is ${question.minLength || 0} characters.`;
    }

    if (control?.hasError('maxlength')) {
      return `Maximum length is ${question.maxLength || 0} characters.`;
    }

    if (control?.hasError('min')) {
      return `Minimum value is ${question.minValue || 0}.`;
    }

    if (control?.hasError('max')) {
      return `Maximum value is ${question.maxValue || 0}.`;
    }

    return 'Invalid input.';
  }

  isFormValid(): boolean {
    return this.questionForms.every(form => form.valid);
  }

  submitPreview(): void {
    if (!this.isFormValid()) {
      this.snackBar.open('Please complete all required fields', 'Close', { duration: 3000 });
      return;
    }

    console.log('Submit preview called - saveToDatabase:', this.saveToDatabase);
    console.log('Data isAdminSolve:', this.data.isAdminSolve);

    // Collect all responses
    this.responses = this.questions.map((question, index) => {
      const form = this.getQuestionForm(index);
      const response = form.get('response')?.value;
      
      return {
        questionId: question.id,
        questionText: question.questionText,
        questionType: question.questionTypeName,
        response: response,
        isRequired: question.isRequired,
        isValid: form.valid
      };
    });

    console.log('Collected responses:', this.responses);

    // If save to database is enabled, save the response
    if (this.saveToDatabase) {
      console.log('Saving response to database...');
      this.saveResponseToDatabase();
    } else {
      console.log('Showing preview results directly...');
      // Show preview results directly
      this.showPreviewResults();
    }
  }

  saveResponseToDatabase(): void {
    console.log('saveResponseToDatabase called');
    console.log('Questionnaire ID:', this.questionnaire?.id);
    
    if (!this.questionnaire?.id) {
      this.snackBar.open('Error: No questionnaire ID', 'Close', { duration: 3000 });
      return;
    }

    // Show loading message
    this.snackBar.open('Processing responses...', 'Close', { duration: 2000 });

    // First, upload any files
    const fileUploads: Observable<any>[] = [];
    const fileResponses: any[] = [];

    this.responses.forEach((response, index) => {
      const form = this.getQuestionForm(index);
      const responseValue = form.get('response')?.value;

      if (response.questionType === 'file' && responseValue && typeof responseValue === 'object' && responseValue.file) {
        fileUploads.push(
          this.fileUploadService.uploadFile(responseValue.file).pipe(
            catchError(error => {
              console.error('File upload error:', error);
              this.snackBar.open(`Error uploading file: ${responseValue.fileName}`, 'Close', { duration: 3000 });
              return of(null);
            })
          )
        );
        fileResponses.push({ index, response });
      }
    });

    // If there are files to upload, upload them first
    if (fileUploads.length > 0) {
      console.log('Uploading files before saving response...');
      forkJoin(fileUploads).pipe(
        switchMap(uploadResults => {
          console.log('File upload results:', uploadResults);
          // Update file responses with uploaded URLs
          uploadResults.forEach((result, i) => {
            if (result && result.success) {
              const { index, response } = fileResponses[i];
              // Store the uploaded file data in the response object instead of form control
              this.responses[index].uploadedFileData = {
                fileName: result.data.fileName,
                fileUrl: result.data.fileUrl,
                fileSize: result.data.fileSize
              };
            }
          });

          // Now save all responses
          console.log('Files uploaded, now saving responses...');
          return this.saveResponsesToBackend();
        })
      ).subscribe({
        next: (result) => {
          console.log('Response saved successfully:', result);
          this.snackBar.open('Response saved successfully!', 'Close', { duration: 3000 });
          this.showPreviewResults();
        },
        error: (error) => {
          console.error('Error saving response:', error);
          this.snackBar.open('Error saving response to database', 'Close', { duration: 3000 });
          this.showPreviewResults();
        }
      });
    } else {
      // No files to upload, save responses directly
      console.log('No files to upload, saving responses directly...');
      this.saveResponsesToBackend().subscribe({
        next: (result) => {
          console.log('Response saved successfully:', result);
          this.snackBar.open('Response saved successfully!', 'Close', { duration: 3000 });
          this.showPreviewResults();
        },
        error: (error) => {
          console.error('Error saving response:', error);
          this.snackBar.open('Error saving response to database', 'Close', { duration: 3000 });
          this.showPreviewResults();
        }
      });
    }
  }

  private saveResponsesToBackend(): Observable<any> {
    // Prepare response data for database
    const responses: any[] = this.responses.map(response => {
      const form = this.getQuestionForm(this.responses.indexOf(response));
      const responseValue = form.get('response')?.value;

      // Handle file responses specially
      if (response.questionType === 'file') {
        // Use uploaded file data if available, otherwise use form value
        const fileData = response.uploadedFileData || responseValue;
        return {
          questionnaireId: this.questionnaire!.id,
          questionId: response.questionId,
          response: fileData
        };
      } else {
        return {
          questionnaireId: this.questionnaire!.id,
          questionId: response.questionId,
          response: responseValue
        };
      }
    });

    console.log('Saving responses to backend:', responses);
    return this.responseService.saveResponses(responses);
  }

  showPreviewResults(): void {
    // Open results dialog
    this.dialogRef.close({
      action: 'preview-complete',
      responses: this.responses,
      questionnaire: this.questionnaire
    });
  }

  onFileSelected(event: any, questionIndex: number): void {
    const file = event.target.files[0];
    if (file) {
      const form = this.getQuestionForm(questionIndex);
      // Store file information as an object to avoid browser security restrictions
      form.get('response')?.setValue({
        file: file,
        fileName: file.name,
        fileSize: file.size,
        fileType: file.type
      });
    }
  }

  getSelectedFileName(questionIndex: number): string {
    const form = this.getQuestionForm(questionIndex);
    const fileData = form.get('response')?.value;
    if (fileData && typeof fileData === 'object' && fileData.fileName) {
      return fileData.fileName;
    } else if (fileData && fileData.name) {
      return fileData.name; // Fallback for old format
    }
    return '';
  }

  getRatingOptions(question: Question): number[] {
    const maxRating = question.maxValue && question.maxValue > 0 ? question.maxValue : 5;
    const minRating = question.minValue && question.minValue > 0 ? question.minValue : 1;
    const options: number[] = [];
    for (let i = minRating; i <= maxRating; i++) {
      options.push(i);
    }
    return options;
  }

  onCheckboxChange(event: any, questionIndex: number, optionId: string): void {
    const form = this.getQuestionForm(questionIndex);
    let currentValue = form.get('response')?.value || [];
    
    if (!Array.isArray(currentValue)) {
      currentValue = [];
    }
    
    if (event.checked) {
      if (!currentValue.includes(optionId)) {
        currentValue.push(optionId);
      }
    } else {
      currentValue = currentValue.filter((value: string) => value !== optionId);
    }
    
    form.get('response')?.setValue(currentValue);
  }

  closeDialog(): void {
    this.dialogRef.close();
  }
} 