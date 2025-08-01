import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatStepperModule } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatRadioModule } from '@angular/material/radio';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSliderModule } from '@angular/material/slider';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { Subject, takeUntil } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { QuestionnaireService } from '../../../services/questionnaire.service';
import { ResponseService } from '../../../services/response.service';
import { QuestionnaireTemplate, Question, QuestionType } from '../../../models/questionnaire.model';
import { QuestionResponse } from '../../../models/questionnaire.model';

@Component({
  selector: 'app-questionnaire-stepper',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatStepperModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressBarModule,
    MatChipsModule,
    MatRadioModule,
    MatCheckboxModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSliderModule,
    MatDialogModule
  ],
  template: `
    <div class="stepper-container">
      <!-- Header -->
      <div class="header-section">
        <mat-card class="header-card">
          <mat-card-content>
            <div class="header-content">
              <div class="questionnaire-info">
                <h1>{{ questionnaire?.title }}</h1>
                <p class="description">{{ questionnaire?.description }}</p>
                <div class="meta-info">
                  <mat-chip-set>
                    <mat-chip color="primary" selected>
                      <mat-icon>category</mat-icon>
                      {{ questionnaire?.categoryName }}
                    </mat-chip>
                    <mat-chip color="accent" selected>
                      <mat-icon>quiz</mat-icon>
                      {{ questionnaire?.questionCount }} questions
                    </mat-chip>
                  </mat-chip-set>
                </div>
              </div>
              <div class="progress-section">
                <div class="progress-info">
                  <span>Progress</span>
                  <span>{{ currentStep + 1 }} / {{ totalSteps }}</span>
                </div>
                <mat-progress-bar 
                  [value]="progressPercentage" 
                  color="primary"
                  mode="determinate">
                </mat-progress-bar>
              </div>
            </div>
          </mat-card-content>
        </mat-card>
      </div>

      <!-- Stepper -->
      <mat-stepper 
        #stepper 
        [linear]="false" 
        class="questionnaire-stepper"
        (selectionChange)="onStepChange($event)">
        
        <mat-step 
          *ngFor="let question of questionnaire?.questions; let i = index" 
          [stepControl]="questionForms[i]"
          [completed]="isStepCompleted(i)">
          
          <ng-template matStepLabel>
            <div class="step-label">
              <span class="step-number">{{ i + 1 }}</span>
              <span class="step-title">{{ question.questionText | slice:0:30 }}{{ question.questionText.length > 30 ? '...' : '' }}</span>
            </div>
          </ng-template>

          <div class="question-container">
            <mat-card class="question-card">
              <mat-card-content>
                <!-- Question Header -->
                <div class="question-header">
                  <h2 class="question-text">{{ question.questionText }}</h2>
                  <div class="question-meta">
                    <mat-chip *ngIf="question.isRequired" color="warn" size="small">
                      <mat-icon>priority_high</mat-icon>
                      Required
                    </mat-chip>
                    <mat-chip color="primary" size="small">
                      <mat-icon>quiz</mat-icon>
                      {{ question.questionTypeName }}
                    </mat-chip>
                  </div>
                  <p *ngIf="question.helpText" class="help-text">
                    <mat-icon>help</mat-icon>
                    {{ question.helpText }}
                  </p>
                </div>

                <!-- Question Form -->
                <form [formGroup]="questionForms[i]" class="question-form">
                  
                  <!-- Text Input -->
                  <mat-form-field *ngIf="question.questionTypeName === 'Text Input'" appearance="outline" class="full-width">
                    <mat-label>{{ question.placeholder || 'Enter your answer' }}</mat-label>
                    <input matInput 
                           formControlName="textResponse"
                           [placeholder]="question.placeholder || ''"
                           [maxlength]="question.maxLength || null">
                    <mat-hint *ngIf="question.maxLength">
                      {{ questionForms[i].get('textResponse')?.value?.length || 0 }} / {{ question.maxLength }}
                    </mat-hint>
                  </mat-form-field>

                  <!-- Text Area -->
                  <mat-form-field *ngIf="question.questionTypeName === 'Text Area'" appearance="outline" class="full-width">
                    <mat-label>{{ question.placeholder || 'Enter your detailed answer' }}</mat-label>
                    <textarea matInput 
                              formControlName="textResponse"
                              [placeholder]="question.placeholder || ''"
                              [maxlength]="question.maxLength || null"
                              rows="4"></textarea>
                    <mat-hint *ngIf="question.maxLength">
                      {{ questionForms[i].get('textResponse')?.value?.length || 0 }} / {{ question.maxLength }}
                    </mat-hint>
                  </mat-form-field>

                  <!-- Number Input -->
                  <mat-form-field *ngIf="question.questionTypeName === 'Number'" appearance="outline" class="full-width">
                    <mat-label>{{ question.placeholder || 'Enter a number' }}</mat-label>
                    <input matInput 
                           type="number"
                           formControlName="numberResponse"
                           [placeholder]="question.placeholder || ''"
                           [min]="question.minValue || null"
                           [max]="question.maxValue || null">
                  </mat-form-field>

                  <!-- Date Input -->
                  <mat-form-field *ngIf="question.questionTypeName === 'Date'" appearance="outline" class="full-width">
                    <mat-label>{{ question.placeholder || 'Select a date' }}</mat-label>
                    <input matInput 
                           [matDatepicker]="picker"
                           formControlName="dateResponse"
                           [placeholder]="question.placeholder || ''">
                    <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
                    <mat-datepicker #picker></mat-datepicker>
                  </mat-form-field>

                  <!-- Email Input -->
                  <mat-form-field *ngIf="question.questionTypeName === 'Email'" appearance="outline" class="full-width">
                    <mat-label>{{ question.placeholder || 'Enter your email' }}</mat-label>
                    <input matInput 
                           type="email"
                           formControlName="textResponse"
                           [placeholder]="question.placeholder || ''">
                  </mat-form-field>

                  <!-- Phone Input -->
                  <mat-form-field *ngIf="question.questionTypeName === 'Phone'" appearance="outline" class="full-width">
                    <mat-label>{{ question.placeholder || 'Enter your phone number' }}</mat-label>
                    <input matInput 
                           type="tel"
                           formControlName="textResponse"
                           [placeholder]="question.placeholder || ''">
                  </mat-form-field>

                  <!-- Radio Buttons -->
                  <div *ngIf="question.questionTypeName === 'Radio Button'" class="radio-group">
                    <mat-radio-group formControlName="selectedOptionIds">
                      <mat-radio-button 
                        *ngFor="let option of question.options" 
                        [value]="option.id"
                        class="radio-option">
                        {{ option.optionText }}
                      </mat-radio-button>
                    </mat-radio-group>
                  </div>

                  <!-- Checkboxes -->
                  <div *ngIf="question.questionTypeName === 'Checkbox'" class="checkbox-group">
                    <mat-checkbox 
                      *ngFor="let option of question.options"
                      [value]="option.id"
                      (change)="onCheckboxChange(i, option.id, $event.checked)"
                      class="checkbox-option">
                      {{ option.optionText }}
                    </mat-checkbox>
                  </div>

                  <!-- Dropdown -->
                  <mat-form-field *ngIf="question.questionTypeName === 'Dropdown'" appearance="outline" class="full-width">
                    <mat-label>{{ question.placeholder || 'Select an option' }}</mat-label>
                    <mat-select formControlName="selectedOptionIds">
                      <mat-option *ngFor="let option of question.options" [value]="option.id">
                        {{ option.optionText }}
                      </mat-option>
                    </mat-select>
                  </mat-form-field>

                  <!-- Multi-Select -->
                  <mat-form-field *ngIf="question.questionTypeName === 'Multi-Select'" appearance="outline" class="full-width">
                    <mat-label>{{ question.placeholder || 'Select options' }}</mat-label>
                    <mat-select formControlName="selectedOptionIds" multiple>
                      <mat-option *ngFor="let option of question.options" [value]="option.id">
                        {{ option.optionText }}
                      </mat-option>
                    </mat-select>
                  </mat-form-field>

                  <!-- Rating Scale -->
                  <div *ngIf="question.questionTypeName === 'Rating Scale'" class="rating-group">
                    <label class="rating-label">{{ question.placeholder || 'Rate from 1 to 5' }}</label>
                    <mat-slider 
                      formControlName="numberResponse"
                      [min]="1" 
                      [max]="5" 
                      [step]="1"
                      class="rating-slider">
                    </mat-slider>
                    <div class="rating-labels">
                      <span>1</span>
                      <span>2</span>
                      <span>3</span>
                      <span>4</span>
                      <span>5</span>
                    </div>
                  </div>

                  <!-- Yes/No -->
                  <div *ngIf="question.questionTypeName === 'Yes/No'" class="yes-no-group">
                    <mat-radio-group formControlName="booleanResponse">
                      <mat-radio-button value="true" class="yes-no-option">
                        <mat-icon>thumb_up</mat-icon>
                        Yes
                      </mat-radio-button>
                      <mat-radio-button value="false" class="yes-no-option">
                        <mat-icon>thumb_down</mat-icon>
                        No
                      </mat-radio-button>
                    </mat-radio-group>
                  </div>

                </form>
              </mat-card-content>
            </mat-card>
          </div>

          <!-- Navigation Buttons -->
          <div class="navigation-buttons">
            <button mat-button 
                    (click)="previousStep()" 
                    [disabled]="i === 0">
              <mat-icon>arrow_back</mat-icon>
              Previous
            </button>
            
            <button mat-raised-button 
                    color="primary" 
                    (click)="nextStep()" 
                    [disabled]="!questionForms[i].valid">
              <mat-icon>arrow_forward</mat-icon>
              {{ i === totalSteps - 1 ? 'Submit' : 'Next' }}
            </button>
          </div>
        </mat-step>
      </mat-stepper>
    </div>
  `,
  styles: [`
    .stepper-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 24px;
    }

    .header-section {
      margin-bottom: 24px;
    }

    .header-card {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }

    .header-content {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 24px 0;
    }

    .questionnaire-info h1 {
      margin: 0 0 8px 0;
      font-size: 1.8rem;
      font-weight: 300;
    }

    .description {
      margin: 0 0 16px 0;
      opacity: 0.9;
    }

    .meta-info {
      margin-bottom: 16px;
    }

    .progress-section {
      text-align: right;
      min-width: 200px;
    }

    .progress-info {
      display: flex;
      justify-content: space-between;
      margin-bottom: 8px;
      font-size: 0.9rem;
    }

    .questionnaire-stepper {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      padding: 24px;
    }

    .step-label {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .step-number {
      width: 24px;
      height: 24px;
      border-radius: 50%;
      background: #3f51b5;
      color: white;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 0.8rem;
      font-weight: 500;
    }

    .step-title {
      font-size: 0.9rem;
    }

    .question-container {
      margin: 24px 0;
    }

    .question-card {
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .question-header {
      margin-bottom: 24px;
    }

    .question-text {
      margin: 0 0 16px 0;
      font-size: 1.3rem;
      font-weight: 500;
      color: #333;
    }

    .question-meta {
      display: flex;
      gap: 8px;
      margin-bottom: 12px;
    }

    .help-text {
      display: flex;
      align-items: center;
      gap: 8px;
      color: #666;
      font-style: italic;
      margin: 0;
    }

    .question-form {
      margin-top: 24px;
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
      margin-bottom: 8px;
    }

    .yes-no-group {
      display: flex;
      gap: 24px;
    }

    .yes-no-option {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .rating-group {
      margin: 24px 0;
    }

    .rating-label {
      display: block;
      margin-bottom: 16px;
      font-weight: 500;
    }

    .rating-slider {
      width: 100%;
      margin-bottom: 8px;
    }

    .rating-labels {
      display: flex;
      justify-content: space-between;
      font-size: 0.8rem;
      color: #666;
    }

    .navigation-buttons {
      display: flex;
      justify-content: space-between;
      margin-top: 32px;
      padding-top: 24px;
      border-top: 1px solid #eee;
    }

    @media (max-width: 768px) {
      .stepper-container {
        padding: 16px;
      }
      
      .header-content {
        flex-direction: column;
        text-align: center;
      }
      
      .progress-section {
        margin-top: 16px;
        text-align: center;
      }
      
      .yes-no-group {
        flex-direction: column;
        gap: 12px;
      }
    }
  `]
})
export class QuestionnaireStepperComponent implements OnInit, OnDestroy {
  questionnaire: QuestionnaireTemplate | null = null;
  questionForms: FormGroup[] = [];
  currentStep = 0;
  totalSteps = 0;
  progressPercentage = 0;
  private destroy$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private questionnaireService: QuestionnaireService,
    private responseService: ResponseService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit() {
    const questionnaireId = this.route.snapshot.paramMap.get('id');
    if (questionnaireId) {
      this.loadQuestionnaire(questionnaireId);
    }
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadQuestionnaire(id: string) {
    console.log('Loading questionnaire with ID:', id);
    console.log('Auth token:', localStorage.getItem('auth_token'));
    console.log('API URL:', environment.apiUrl);
    
    this.questionnaireService.getQuestionnaireById(id).subscribe({
      next: (response: any) => {
        console.log('Questionnaire loaded successfully:', response);
        if (response.success) {
          this.questionnaire = response.data;
          this.totalSteps = this.questionnaire?.questions?.length || 0;
          console.log('Questionnaire data:', this.questionnaire);
          console.log('Total steps:', this.totalSteps);
          this.initializeForms();
        } else {
          console.error('Failed to load questionnaire:', response.message);
          this.snackBar.open('Failed to load questionnaire: ' + response.message, 'Close', { duration: 3000 });
          this.router.navigate(['/questionnaires']);
        }
      },
      error: (error: any) => {
        console.error('Error loading questionnaire:', error);
        console.error('Error details:', {
          status: error.status,
          statusText: error.statusText,
          message: error.message,
          error: error.error
        });
        this.snackBar.open('Error loading questionnaire: ' + (error.message || 'Unknown error'), 'Close', { duration: 3000 });
        this.router.navigate(['/questionnaires']);
      }
    });
  }

  initializeForms() {
    if (!this.questionnaire?.questions) return;

    this.questionForms = this.questionnaire.questions.map((question: any) => {
      const formGroup = this.fb.group({
        textResponse: [''],
        numberResponse: [null],
        dateResponse: [null],
        booleanResponse: [null],
        selectedOptionIds: question.questionTypeName === 'Checkbox' ? [[]] : ['']
      });

      // Add validators for required questions
      if (question.isRequired) {
        const control = this.getMainControl(question, formGroup);
        if (control) {
          control.setValidators([Validators.required]);
          control.updateValueAndValidity();
        }
      }

      return formGroup;
    });
  }

  getMainControl(question: Question, formGroup: FormGroup) {
    switch (question.questionTypeName) {
      case 'Text Input':
      case 'Text Area':
      case 'Email':
      case 'Phone':
        return formGroup.get('textResponse');
      case 'Number':
        return formGroup.get('numberResponse');
      case 'Date':
        return formGroup.get('dateResponse');
      case 'Yes/No':
        return formGroup.get('booleanResponse');
      case 'Radio Button':
      case 'Dropdown':
      case 'Multi-Select':
      case 'Checkbox':
        return formGroup.get('selectedOptionIds');
      default:
        return formGroup.get('textResponse');
    }
  }

  onStepChange(event: any) {
    this.currentStep = event.selectedIndex;
    this.updateProgress();
  }

  updateProgress() {
    this.progressPercentage = ((this.currentStep + 1) / this.totalSteps) * 100;
  }

  isStepCompleted(index: number): boolean {
    const form = this.questionForms[index];
    return form && form.valid && form.dirty;
  }

  onCheckboxChange(questionIndex: number, optionId: string, checked: boolean) {
    const form = this.questionForms[questionIndex];
    const currentValues = form.get('selectedOptionIds')?.value || [];
    
    if (checked) {
      form.get('selectedOptionIds')?.setValue([...currentValues, optionId]);
    } else {
      form.get('selectedOptionIds')?.setValue(currentValues.filter((id: string) => id !== optionId));
    }
  }

  previousStep() {
    if (this.currentStep > 0) {
      this.currentStep--;
      this.updateProgress();
    }
  }

  nextStep() {
    if (this.currentStep < this.totalSteps - 1) {
      this.currentStep++;
      this.updateProgress();
    } else {
      this.submitQuestionnaire();
    }
  }

  submitQuestionnaire() {
    if (!this.questionnaire) return;

    const responses: QuestionResponse[] = this.questionForms.map((form, index) => {
      const question = this.questionnaire!.questions[index];
      const formValue = form.value;

      return {
        questionId: question.id,
        textResponse: formValue.textResponse,
        numberResponse: formValue.numberResponse,
        dateResponse: formValue.dateResponse ? new Date(formValue.dateResponse).toISOString() : undefined,
        selectedOptionIds: Array.isArray(formValue.selectedOptionIds) ? formValue.selectedOptionIds : [formValue.selectedOptionIds],
        booleanResponse: formValue.booleanResponse
      };
    });

    const submitRequest = {
      questionnaireId: this.questionnaire.id,
      responses: responses
    };

    this.responseService.submitResponse(submitRequest).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.snackBar.open('Questionnaire submitted successfully!', 'Close', { duration: 3000 });
          this.router.navigate(['/dashboard']);
        } else {
          this.snackBar.open(response.message || 'Failed to submit questionnaire', 'Close', { duration: 3000 });
        }
      },
      error: (error: any) => {
        this.snackBar.open('Error submitting questionnaire', 'Close', { duration: 3000 });
      }
    });
  }
} 