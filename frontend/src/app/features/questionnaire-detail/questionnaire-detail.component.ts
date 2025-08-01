import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { QuestionnaireService } from '../../services/questionnaire.service';
import { CategoryQuestionnaireTemplate, CategoryQuestion, QuestionResponse } from '../../models/questionnaire.model';

@Component({
  selector: 'app-questionnaire-detail',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatRadioModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressBarModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="questionnaire-container">
      <div class="header">
        <div class="breadcrumb">
          <button mat-button routerLink="/questionnaires">
            <mat-icon>arrow_back</mat-icon>
            Back to Questionnaires
          </button>
        </div>
        
        <div class="questionnaire-info" *ngIf="questionnaire">
          <h1>{{ questionnaire.title }}</h1>
          <p class="description">{{ questionnaire.description }}</p>
          
          <div class="meta-info">
            <span class="category">{{ questionnaire.categoryName }}</span>
            <span class="questions-count">{{ questionnaire.questions.length || 0 }} questions</span>
            <span class="mandatory" *ngIf="questionnaire.isMandatory">Mandatory</span>
          </div>
        </div>
      </div>

      <div class="progress-section" *ngIf="questionnaire">
        <mat-progress-bar 
          [value]="progressPercentage" 
          color="primary">
        </mat-progress-bar>
        <div class="progress-text">
          Question {{ currentQuestionIndex + 1 }} of {{ questionnaire.questions.length || 0 }}
        </div>
      </div>

      <div class="questionnaire-content" *ngIf="questionnaire && currentQuestion">
        <mat-card class="question-card">
          <mat-card-header>
            <mat-card-title>
              <span class="question-number">{{ currentQuestionIndex + 1 }}.</span>
              {{ currentQuestion.questionText }}
              <span class="required-indicator" *ngIf="currentQuestion.isRequired">*</span>
            </mat-card-title>
          </mat-card-header>
          
          <mat-card-content>
            <form [formGroup]="questionForm" (ngSubmit)="onSubmit()">
              <!-- Text Input -->
              <mat-form-field *ngIf="currentQuestion.questionTypeId === '66666666-6666-6666-6666-666666666666'" 
                             appearance="outline" class="full-width">
                <mat-label>Your answer</mat-label>
                <input matInput formControlName="textResponse" 
                       [placeholder]="'Enter your answer'"
                       [minlength]="currentQuestion.minLength || null"
                       [maxlength]="currentQuestion.maxLength || null">
                <mat-hint *ngIf="currentQuestion.minLength || currentQuestion.maxLength">
                  {{ currentQuestion.minLength || 0 }}-{{ currentQuestion.maxLength || 100 }} characters
                </mat-hint>
                <mat-error *ngIf="questionForm.get('textResponse')?.hasError('required')">
                  This field is required
                </mat-error>
                <mat-error *ngIf="questionForm.get('textResponse')?.hasError('minlength')">
                  Minimum {{ currentQuestion.minLength }} characters required
                </mat-error>
                <mat-error *ngIf="questionForm.get('textResponse')?.hasError('maxlength')">
                  Maximum {{ currentQuestion.maxLength }} characters allowed
                </mat-error>
              </mat-form-field>

              <!-- Number Input -->
              <mat-form-field *ngIf="currentQuestion.questionTypeId === 'cccccccc-cccc-cccc-cccc-cccccccccccc'" 
                             appearance="outline" class="full-width">
                <mat-label>Your answer</mat-label>
                <input matInput type="number" formControlName="numberResponse" 
                       [placeholder]="'Enter a number'"
                       [min]="currentQuestion.minValue || null"
                       [max]="currentQuestion.maxValue || null">
                <mat-hint *ngIf="currentQuestion.minValue || currentQuestion.maxValue">
                  Range: {{ currentQuestion.minValue || 0 }}-{{ currentQuestion.maxValue || 100 }}
                </mat-hint>
                <mat-error *ngIf="questionForm.get('numberResponse')?.hasError('required')">
                  This field is required
                </mat-error>
                <mat-error *ngIf="questionForm.get('numberResponse')?.hasError('min')">
                  Minimum value is {{ currentQuestion.minValue }}
                </mat-error>
                <mat-error *ngIf="questionForm.get('numberResponse')?.hasError('max')">
                  Maximum value is {{ currentQuestion.maxValue }}
                </mat-error>
              </mat-form-field>

              <!-- Date Input -->
              <mat-form-field *ngIf="currentQuestion.questionTypeId === 'dddddddd-dddd-dddd-dddd-dddddddddddd'" 
                             appearance="outline" class="full-width">
                <mat-label>Choose a date</mat-label>
                <input matInput [matDatepicker]="picker" formControlName="dateResponse">
                <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
                <mat-datepicker #picker></mat-datepicker>
                <mat-error *ngIf="questionForm.get('dateResponse')?.hasError('required')">
                  This field is required
                </mat-error>
              </mat-form-field>

              <!-- Email Input -->
              <mat-form-field *ngIf="currentQuestion.questionTypeId === '99999999-9999-9999-9999-999999999999'" 
                             appearance="outline" class="full-width">
                <mat-label>Email address</mat-label>
                <input matInput type="email" formControlName="textResponse" 
                       placeholder="Enter your email address">
                <mat-error *ngIf="questionForm.get('textResponse')?.hasError('required')">
                  This field is required
                </mat-error>
                <mat-error *ngIf="questionForm.get('textResponse')?.hasError('email')">
                  Please enter a valid email address
                </mat-error>
              </mat-form-field>

              <!-- Radio Buttons -->
              <div *ngIf="currentQuestion.questionTypeId === '33333333-3333-3333-3333-333333333333'" class="radio-group">
                <mat-radio-group formControlName="selectedOptionIds">
                  <mat-radio-button *ngFor="let option of currentQuestion.options" 
                                   [value]="option.id" class="radio-option">
                    {{ option.optionText }}
                  </mat-radio-button>
                </mat-radio-group>
                <mat-error *ngIf="questionForm.get('selectedOptionIds')?.hasError('required')">
                  Please select an option
                </mat-error>
              </div>

              <!-- Checkboxes -->
              <div *ngIf="currentQuestion.questionTypeId === '44444444-4444-4444-4444-444444444444'" class="checkbox-group">
                <mat-checkbox *ngFor="let option of currentQuestion.options" 
                             [value]="option.id"
                             (change)="onCheckboxChange($event, option.id)"
                             class="checkbox-option">
                  {{ option.optionText }}
                </mat-checkbox>
                <mat-error *ngIf="questionForm.get('selectedOptionIds')?.hasError('required')">
                  Please select at least one option
                </mat-error>
              </div>

              <!-- Select Dropdown -->
              <mat-form-field *ngIf="currentQuestion.questionTypeId === '55555555-5555-5555-5555-555555555555'" 
                             appearance="outline" class="full-width">
                <mat-label>Select an option</mat-label>
                <mat-select formControlName="selectedOptionIds">
                  <mat-option *ngFor="let option of currentQuestion.options" 
                             [value]="option.id">
                    {{ option.optionText }}
                  </mat-option>
                </mat-select>
                <mat-error *ngIf="questionForm.get('selectedOptionIds')?.hasError('required')">
                  Please select an option
                </mat-error>
              </mat-form-field>

              <div class="form-actions">
                <button mat-button type="button" 
                        [disabled]="currentQuestionIndex === 0"
                        (click)="previousQuestion()">
                  <mat-icon>arrow_back</mat-icon>
                  Previous
                </button>
                
                <button mat-raised-button color="primary" type="submit" 
                        [disabled]="questionForm.invalid || isLoading">
                  <mat-icon>arrow_forward</mat-icon>
                  {{ isLastQuestion ? 'Submit' : 'Next' }}
                </button>
              </div>
            </form>
          </mat-card-content>
        </mat-card>
      </div>

      <div class="loading-container" *ngIf="isLoading">
        <mat-spinner></mat-spinner>
        <p>Loading questionnaire...</p>
      </div>
    </div>
  `,
  styles: [`
    .questionnaire-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 20px;
    }

    .header {
      margin-bottom: 30px;
    }

    .breadcrumb {
      margin-bottom: 20px;
    }

    .questionnaire-info h1 {
      color: #333;
      margin-bottom: 8px;
      font-size: 2rem;
      font-weight: 300;
    }

    .description {
      color: #666;
      font-size: 1.1rem;
      margin-bottom: 16px;
    }

    .meta-info {
      display: flex;
      gap: 16px;
      flex-wrap: wrap;
    }

    .meta-info span {
      padding: 4px 12px;
      border-radius: 16px;
      font-size: 0.9rem;
      font-weight: 500;
    }

    .category {
      background-color: #e3f2fd;
      color: #1976d2;
    }

    .questions-count {
      background-color: #f3e5f5;
      color: #7b1fa2;
    }

    .mandatory {
      background-color: #ffebee;
      color: #d32f2f;
    }

    .progress-section {
      margin-bottom: 30px;
    }

    .progress-text {
      text-align: center;
      margin-top: 8px;
      color: #666;
      font-size: 0.9rem;
    }

    .question-card {
      border-radius: 12px;
      overflow: hidden;
    }

    .question-number {
      color: #1976d2;
      font-weight: 600;
      margin-right: 8px;
    }

    .required-indicator {
      color: #d32f2f;
      font-weight: 600;
    }

    .full-width {
      width: 100%;
      margin-bottom: 16px;
    }

    .radio-group, .checkbox-group {
      margin: 16px 0;
    }

    .radio-option, .checkbox-option {
      display: block;
      margin: 12px 0;
    }

    .form-actions {
      display: flex;
      justify-content: space-between;
      margin-top: 24px;
      padding-top: 16px;
      border-top: 1px solid #f0f0f0;
    }

    .loading-container {
      text-align: center;
      padding: 60px 20px;
    }

    .loading-container p {
      margin-top: 16px;
      color: #666;
    }

    @media (max-width: 768px) {
      .questionnaire-container {
        padding: 16px;
      }

      .questionnaire-info h1 {
        font-size: 1.5rem;
      }

      .meta-info {
        flex-direction: column;
        gap: 8px;
      }

      .form-actions {
        flex-direction: column;
        gap: 12px;
      }
    }
  `]
})
export class QuestionnaireDetailComponent implements OnInit {
  questionnaire?: CategoryQuestionnaireTemplate;
  currentQuestionIndex = 0;
  currentQuestion?: CategoryQuestion;
  questionForm: FormGroup;
  responses: QuestionResponse[] = [];
  isLoading = false;

  constructor(
    private questionnaireService: QuestionnaireService,
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private snackBar: MatSnackBar
  ) {
    this.questionForm = this.fb.group({});
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const id = params['id'];
      if (id) {
        this.loadQuestionnaire(id);
      }
    });
  }

  loadQuestionnaire(id: string): void {
    this.isLoading = true;
    this.questionnaireService.getQuestionnaireForResponse(id).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.questionnaire = response.data;
          this.initializeForm();
          this.createQuestionForm();
        } else {
          this.snackBar.open(response.message || 'Failed to load questionnaire', 'Close', { duration: 5000 });
        }
      },
      error: (error) => {
        this.snackBar.open('Error loading questionnaire', 'Close', { duration: 5000 });
        console.error('Error loading questionnaire:', error);
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  initializeForm(): void {
    this.currentQuestionIndex = 0;
    this.currentQuestion = this.questionnaire?.questions[0];
    this.createQuestionForm();
  }

  createQuestionForm(): void {
    if (!this.currentQuestion) return;

    const controls: any = {};

    switch (this.currentQuestion.questionTypeId) {
      case '66666666-6666-6666-6666-666666666666': // Text
        controls['textResponse'] = ['', this.currentQuestion.isRequired ? Validators.required : null];
        if (this.currentQuestion.minLength) {
          controls['textResponse'].push(Validators.minLength(this.currentQuestion.minLength));
        }
        if (this.currentQuestion.maxLength) {
          controls['textResponse'].push(Validators.maxLength(this.currentQuestion.maxLength));
        }
        break;

      case 'cccccccc-cccc-cccc-cccc-cccccccccccc': // Number
        controls['numberResponse'] = ['', this.currentQuestion.isRequired ? Validators.required : null];
        if (this.currentQuestion.minValue !== undefined) {
          controls['numberResponse'].push(Validators.min(this.currentQuestion.minValue));
        }
        if (this.currentQuestion.maxValue !== undefined) {
          controls['numberResponse'].push(Validators.max(this.currentQuestion.maxValue));
        }
        break;

      case 'dddddddd-dddd-dddd-dddd-dddddddddddd': // Date
        controls['dateResponse'] = ['', this.currentQuestion.isRequired ? Validators.required : null];
        break;

      case '99999999-9999-9999-9999-999999999999': // Email
        controls['textResponse'] = ['', [
          this.currentQuestion.isRequired ? Validators.required : null,
          Validators.email
        ].filter(Boolean)];
        break;

      case '33333333-3333-3333-3333-333333333333': // Radio
      case '55555555-5555-5555-5555-555555555555': // Select
        controls['selectedOptionIds'] = ['', this.currentQuestion.isRequired ? Validators.required : null];
        break;

      case '44444444-4444-4444-4444-444444444444': // Checkbox
        controls['selectedOptionIds'] = [[]];
        break;
    }

    this.questionForm = this.fb.group(controls);
    this.loadSavedResponse();
  }

  get progressPercentage(): number {
    if (!this.questionnaire?.questions.length) return 0;
    return ((this.currentQuestionIndex + 1) / this.questionnaire.questions.length) * 100;
  }

  get isLastQuestion(): boolean {
    return this.currentQuestionIndex === (this.questionnaire?.questions.length || 0) - 1;
  }

  nextQuestion(): void {
    this.saveCurrentResponse();
    
    if (this.currentQuestionIndex < (this.questionnaire?.questions.length || 0) - 1) {
      this.currentQuestionIndex++;
      this.currentQuestion = this.questionnaire?.questions[this.currentQuestionIndex];
      this.createQuestionForm();
    }
  }

  previousQuestion(): void {
    this.saveCurrentResponse();
    
    if (this.currentQuestionIndex > 0) {
      this.currentQuestionIndex--;
      this.currentQuestion = this.questionnaire?.questions[this.currentQuestionIndex];
      this.createQuestionForm();
    }
  }

  saveCurrentResponse(): void {
    if (!this.currentQuestion || this.questionForm.invalid) return;

    const formValue = this.questionForm.value;
    const response: QuestionResponse = {
      questionId: this.currentQuestion.id,
      selectedOptionIds: []
    };

    switch (this.currentQuestion.questionTypeId) {
      case '66666666-6666-6666-6666-666666666666': // Text
      case '99999999-9999-9999-9999-999999999999': // Email
        response.textResponse = formValue.textResponse;
        break;

      case 'cccccccc-cccc-cccc-cccc-cccccccccccc': // Number
        response.numberResponse = formValue.numberResponse;
        break;

      case 'dddddddd-dddd-dddd-dddd-dddddddddddd': // Date
        response.dateResponse = formValue.dateResponse;
        break;

      case '33333333-3333-3333-3333-333333333333': // Radio
      case '55555555-5555-5555-5555-555555555555': // Select
        response.selectedOptionIds = [formValue.selectedOptionIds];
        break;

      case '44444444-4444-4444-4444-444444444444': // Checkbox
        response.selectedOptionIds = formValue.selectedOptionIds || [];
        break;
    }

    // Update or add response
    const existingIndex = this.responses.findIndex(r => r.questionId === this.currentQuestion?.id);
    if (existingIndex >= 0) {
      this.responses[existingIndex] = response;
    } else {
      this.responses.push(response);
    }
  }

  loadSavedResponse(): void {
    if (!this.currentQuestion) return;

    const savedResponse = this.responses.find(r => r.questionId === this.currentQuestion?.id);
    if (savedResponse) {
      switch (this.currentQuestion.questionTypeId) {
        case '66666666-6666-6666-6666-666666666666': // Text
        case '99999999-9999-9999-9999-999999999999': // Email
          this.questionForm.patchValue({ textResponse: savedResponse.textResponse });
          break;

        case 'cccccccc-cccc-cccc-cccc-cccccccccccc': // Number
          this.questionForm.patchValue({ numberResponse: savedResponse.numberResponse });
          break;

        case 'dddddddd-dddd-dddd-dddd-dddddddddddd': // Date
          this.questionForm.patchValue({ dateResponse: savedResponse.dateResponse });
          break;

        case '33333333-3333-3333-3333-333333333333': // Radio
        case '55555555-5555-5555-5555-555555555555': // Select
          this.questionForm.patchValue({ selectedOptionIds: savedResponse.selectedOptionIds[0] });
          break;

        case '44444444-4444-4444-4444-444444444444': // Checkbox
          this.questionForm.patchValue({ selectedOptionIds: savedResponse.selectedOptionIds });
          break;
      }
    }
  }

  onCheckboxChange(event: any, optionId: string): void {
    const currentValues = this.questionForm.get('selectedOptionIds')?.value || [];
    
    if (event.checked) {
      if (!currentValues.includes(optionId)) {
        currentValues.push(optionId);
      }
    } else {
      const index = currentValues.indexOf(optionId);
      if (index > -1) {
        currentValues.splice(index, 1);
      }
    }
    
    this.questionForm.patchValue({ selectedOptionIds: currentValues });
  }

  submitQuestionnaire(): void {
    if (!this.questionnaire) return;

    this.saveCurrentResponse();

    const submitRequest = {
      questionnaireId: this.questionnaire.id,
      responses: this.responses
    };

    this.isLoading = true;
    this.questionnaireService.submitResponse(submitRequest).subscribe({
      next: (response) => {
        if (response.success) {
          this.snackBar.open('Questionnaire submitted successfully!', 'Close', { duration: 5000 });
          this.router.navigate(['/questionnaires']);
        } else {
          this.snackBar.open(response.message || 'Failed to submit questionnaire', 'Close', { duration: 5000 });
        }
      },
      error: (error) => {
        this.snackBar.open('Error submitting questionnaire', 'Close', { duration: 5000 });
        console.error('Error submitting questionnaire:', error);
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.questionForm.invalid) return;

    if (this.isLastQuestion) {
      this.submitQuestionnaire();
    } else {
      this.nextQuestion();
    }
  }
} 