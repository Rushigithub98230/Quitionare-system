import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormArray } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSliderModule } from '@angular/material/slider';
import { MatChipsModule } from '@angular/material/chips';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { QuestionnaireService } from '../../../services/questionnaire.service';
import { CategoryService } from '../../../services/category.service';
import { QuestionTypeService } from '../../../services/question-type.service';
import { AuthService } from '../../../services/auth.service';
import { CategoryQuestionnaireTemplate, CategoryQuestion, QuestionType, CreateCategoryQuestionnaireTemplateRequest } from '../../../models/questionnaire.model';
import { Category } from '../../../models/category.model';

@Component({
  selector: 'app-questionnaire-builder',
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
    MatSliderModule,
    MatChipsModule,
    MatExpansionModule,
    MatListModule,
    MatDividerModule,
    MatDialogModule,
    MatMenuModule,
    MatTooltipModule,
    MatProgressBarModule,
    DragDropModule
  ],
  template: `
    <div class="builder-container">
      <!-- Header -->
      <div class="header-section">
        <mat-card class="header-card">
          <mat-card-content>
            <div class="header-content">
              <div class="header-info">
                <h1>Questionnaire Builder</h1>
                <p>Create and customize your questionnaire with different question types</p>
              </div>
              <div class="header-actions">
                <button mat-stroked-button (click)="previewQuestionnaire()">
                  <mat-icon>visibility</mat-icon>
                  Preview
                </button>
                <button mat-raised-button color="primary" (click)="saveQuestionnaire()" [disabled]="!builderForm.valid">
                  <mat-icon>save</mat-icon>
                  Save Questionnaire
                </button>
              </div>
            </div>
          </mat-card-content>
        </mat-card>
      </div>

      <div class="builder-content">
        <!-- Left Panel - Questionnaire Settings -->
        <div class="left-panel">
          <mat-card class="settings-card">
            <mat-card-header>
              <mat-card-title>
                <mat-icon>settings</mat-icon>
                Questionnaire Settings
              </mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <form [formGroup]="builderForm" class="settings-form">
                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Title</mat-label>
                  <input matInput formControlName="title" placeholder="Enter questionnaire title">
                  <mat-error *ngIf="builderForm.get('title')?.hasError('required')">
                    Title is required
                  </mat-error>
                </mat-form-field>

                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Description</mat-label>
                  <textarea matInput formControlName="description" rows="3" placeholder="Enter questionnaire description"></textarea>
                </mat-form-field>

                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Category</mat-label>
                  <mat-select formControlName="categoryId">
                    <mat-option *ngFor="let category of categories" [value]="category.id">
                      {{ category.name }}
                    </mat-option>
                  </mat-select>
                  <mat-error *ngIf="builderForm.get('categoryId')?.hasError('required')">
                    Category is required
                  </mat-error>
                </mat-form-field>

                <div class="form-row">
                  <mat-form-field appearance="outline">
                    <mat-label>Display Order</mat-label>
                    <input matInput type="number" formControlName="displayOrder" min="0">
                  </mat-form-field>

                  <mat-form-field appearance="outline">
                    <mat-label>Version</mat-label>
                    <input matInput type="number" formControlName="version" min="1">
                  </mat-form-field>
                </div>

                <div class="checkbox-row">
                  <mat-checkbox formControlName="isActive">
                    Active
                  </mat-checkbox>
                  <mat-checkbox formControlName="isMandatory">
                    Mandatory
                  </mat-checkbox>
                </div>
              </form>
            </mat-card-content>
          </mat-card>

          <!-- Question Types Panel -->
          <mat-card class="question-types-card">
            <mat-card-header>
              <mat-card-title>
                <mat-icon>quiz</mat-icon>
                Question Types
              </mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <div class="question-types-grid">
                <div *ngFor="let questionType of questionTypes" 
                     class="question-type-item"
                     (click)="addQuestion(questionType)"
                     [matTooltip]="questionType.displayName">
                  <mat-icon>{{ getQuestionTypeIcon(questionType.typeName) }}</mat-icon>
                  <span>{{ questionType.displayName }}</span>
                </div>
              </div>
            </mat-card-content>
          </mat-card>
        </div>

        <!-- Right Panel - Questions List -->
        <div class="right-panel">
          <mat-card class="questions-card">
            <mat-card-header>
              <mat-card-title>
                <mat-icon>list</mat-icon>
                Questions ({{ questionsArray.length }})
              </mat-card-title>
              <mat-card-subtitle>
                Drag and drop to reorder questions
              </mat-card-subtitle>
            </mat-card-header>
            <mat-card-content>
              <div class="questions-container" cdkDropList (cdkDropListDropped)="drop($event)">
                <div *ngFor="let question of questionsArray.controls; let i = index" 
                     class="question-item"
                     cdkDrag>
                  <mat-card class="question-card">
                    <mat-card-content>
                      <div class="question-header">
                        <div class="question-info">
                          <span class="question-number">{{ i + 1 }}</span>
                          <div class="question-details">
                            <h3>{{ question.get('questionText')?.value || 'Untitled Question' }}</h3>
                            <mat-chip-set>
                              <mat-chip size="small" color="primary">
                                {{ getQuestionTypeName(question.get('questionTypeId')?.value) }}
                              </mat-chip>
                              <mat-chip *ngIf="question.get('isRequired')?.value" size="small" color="warn">
                                Required
                              </mat-chip>
                            </mat-chip-set>
                          </div>
                        </div>
                        <div class="question-actions">
                          <button mat-icon-button [matMenuTriggerFor]="questionMenu">
                            <mat-icon>more_vert</mat-icon>
                          </button>
                          <mat-menu #questionMenu="matMenu">
                            <button mat-menu-item (click)="editQuestion(i)">
                              <mat-icon>edit</mat-icon>
                              Edit
                            </button>
                            <button mat-menu-item (click)="duplicateQuestion(i)">
                              <mat-icon>content_copy</mat-icon>
                              Duplicate
                            </button>
                            <mat-divider></mat-divider>
                            <button mat-menu-item (click)="deleteQuestion(i)" color="warn">
                              <mat-icon>delete</mat-icon>
                              Delete
                            </button>
                          </mat-menu>
                        </div>
                      </div>
                    </mat-card-content>
                  </mat-card>
                </div>

                <!-- Empty State -->
                <div *ngIf="questionsArray.length === 0" class="empty-state">
                  <mat-icon>quiz</mat-icon>
                  <h3>No Questions Added</h3>
                  <p>Start building your questionnaire by adding questions from the left panel</p>
                </div>
              </div>
            </mat-card-content>
          </mat-card>
        </div>
      </div>

      <!-- Question Editor Dialog -->
      <div *ngIf="editingQuestionIndex !== null" class="question-editor-overlay">
        <mat-card class="question-editor-card">
          <mat-card-header>
            <mat-card-title>
              <mat-icon>edit</mat-icon>
              {{ editingQuestionIndex === -1 ? 'Add Question' : 'Edit Question' }}
            </mat-card-title>
            <button mat-icon-button (click)="closeQuestionEditor()">
              <mat-icon>close</mat-icon>
            </button>
          </mat-card-header>
          <mat-card-content>
            <form [formGroup]="questionForm" class="question-form">
              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Question Text</mat-label>
                <textarea matInput formControlName="questionText" rows="3" placeholder="Enter your question"></textarea>
                <mat-error *ngIf="questionForm.get('questionText')?.hasError('required')">
                  Question text is required
                </mat-error>
              </mat-form-field>

              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Question Type</mat-label>
                <mat-select formControlName="questionTypeId">
                  <mat-option *ngFor="let type of questionTypes" [value]="type.id">
                    {{ type.displayName }}
                  </mat-option>
                </mat-select>
              </mat-form-field>

              <div class="form-row">
                <mat-form-field appearance="outline">
                  <mat-label>Display Order</mat-label>
                  <input matInput type="number" formControlName="displayOrder" min="1">
                </mat-form-field>

                <mat-form-field appearance="outline">
                  <mat-label>Section Name</mat-label>
                  <input matInput formControlName="sectionName" placeholder="Optional section name">
                </mat-form-field>
              </div>

              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Help Text</mat-label>
                <textarea matInput formControlName="helpText" rows="2" placeholder="Optional help text for this question"></textarea>
              </mat-form-field>

              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Placeholder</mat-label>
                <input matInput formControlName="placeholder" placeholder="Optional placeholder text">
              </mat-form-field>

              <!-- Validation Settings -->
              <mat-expansion-panel class="validation-panel">
                <mat-expansion-panel-header>
                  <mat-panel-title>
                    <mat-icon>settings</mat-icon>
                    Validation Settings
                  </mat-panel-title>
                </mat-expansion-panel-header>
                
                <div class="validation-settings">
                  <mat-checkbox formControlName="isRequired">
                    Required Question
                  </mat-checkbox>

                  <div class="form-row" *ngIf="isTextQuestion()">
                    <mat-form-field appearance="outline">
                      <mat-label>Min Length</mat-label>
                      <input matInput type="number" formControlName="minLength" min="0">
                    </mat-form-field>

                    <mat-form-field appearance="outline">
                      <mat-label>Max Length</mat-label>
                      <input matInput type="number" formControlName="maxLength" min="1">
                    </mat-form-field>
                  </div>

                  <div class="form-row" *ngIf="isNumberQuestion()">
                    <mat-form-field appearance="outline">
                      <mat-label>Min Value</mat-label>
                      <input matInput type="number" formControlName="minValue">
                    </mat-form-field>

                    <mat-form-field appearance="outline">
                      <mat-label>Max Value</mat-label>
                      <input matInput type="number" formControlName="maxValue">
                    </mat-form-field>
                  </div>
                </div>
              </mat-expansion-panel>

              <!-- Options for Multiple Choice Questions -->
              <mat-expansion-panel *ngIf="hasOptions()" class="options-panel">
                <mat-expansion-panel-header>
                  <mat-panel-title>
                    <mat-icon>list</mat-icon>
                    Options
                  </mat-panel-title>
                </mat-expansion-panel-header>
                
                <div formArrayName="options" class="options-container">
                  <div *ngFor="let option of optionsArray.controls; let i = index" 
                       [formGroupName]="i" 
                       class="option-item">
                    <mat-form-field appearance="outline">
                      <mat-label>Option {{ i + 1 }}</mat-label>
                      <input matInput formControlName="optionText" placeholder="Enter option text">
                    </mat-form-field>
                    <button mat-icon-button (click)="removeOption(i)" color="warn">
                      <mat-icon>delete</mat-icon>
                    </button>
                  </div>
                  <button mat-stroked-button (click)="addOption()" class="add-option-btn">
                    <mat-icon>add</mat-icon>
                    Add Option
                  </button>
                </div>
              </mat-expansion-panel>
            </form>
          </mat-card-content>
          <mat-card-actions>
            <button mat-button (click)="closeQuestionEditor()">Cancel</button>
            <button mat-raised-button color="primary" (click)="saveQuestion()" [disabled]="!questionForm.valid">
              {{ editingQuestionIndex === -1 ? 'Add Question' : 'Update Question' }}
            </button>
          </mat-card-actions>
        </mat-card>
      </div>
    </div>
  `,
  styles: [`
    .builder-container {
      max-width: 1400px;
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

    .header-info h1 {
      margin: 0 0 8px 0;
      font-size: 2rem;
      font-weight: 300;
    }

    .header-info p {
      margin: 0;
      opacity: 0.9;
    }

    .header-actions {
      display: flex;
      gap: 12px;
    }

    .builder-content {
      display: grid;
      grid-template-columns: 1fr 2fr;
      gap: 24px;
    }

    .settings-card, .question-types-card, .questions-card {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .settings-form {
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .full-width {
      width: 100%;
    }

    .form-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 16px;
    }

    .checkbox-row {
      display: flex;
      gap: 24px;
    }

    .question-types-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
      gap: 12px;
    }

    .question-type-item {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: 16px;
      border: 2px dashed #ddd;
      border-radius: 8px;
      cursor: pointer;
      transition: all 0.2s;
      text-align: center;
    }

    .question-type-item:hover {
      border-color: #3f51b5;
      background: #f5f5f5;
    }

    .question-type-item mat-icon {
      font-size: 2rem;
      margin-bottom: 8px;
      color: #3f51b5;
    }

    .question-type-item span {
      font-size: 0.9rem;
      font-weight: 500;
    }

    .questions-container {
      min-height: 400px;
    }

    .question-item {
      margin-bottom: 16px;
    }

    .question-card {
      border-radius: 8px;
      transition: box-shadow 0.2s;
    }

    .question-card:hover {
      box-shadow: 0 4px 12px rgba(0,0,0,0.15);
    }

    .question-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .question-info {
      display: flex;
      align-items: center;
      gap: 16px;
    }

    .question-number {
      width: 32px;
      height: 32px;
      border-radius: 50%;
      background: #3f51b5;
      color: white;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: 500;
    }

    .question-details h3 {
      margin: 0 0 8px 0;
      font-size: 1.1rem;
    }

    .empty-state {
      text-align: center;
      padding: 48px 24px;
      color: #666;
    }

    .empty-state mat-icon {
      font-size: 4rem;
      margin-bottom: 16px;
      color: #ddd;
    }

    .empty-state h3 {
      margin: 0 0 8px 0;
      color: #333;
    }

    .empty-state p {
      margin: 0;
      font-size: 0.9rem;
    }

    .question-editor-overlay {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: rgba(0,0,0,0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
      padding: 24px;
    }

    .question-editor-card {
      max-width: 600px;
      width: 100%;
      max-height: 90vh;
      overflow-y: auto;
    }

    .question-editor-card mat-card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .question-form {
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .validation-panel, .options-panel {
      margin-top: 16px;
    }

    .validation-settings {
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .options-container {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }

    .option-item {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .option-item mat-form-field {
      flex: 1;
    }

    .add-option-btn {
      align-self: flex-start;
    }

    .cdk-drag-preview {
      box-shadow: 0 4px 12px rgba(0,0,0,0.15);
    }

    .cdk-drag-placeholder {
      opacity: 0.3;
    }

    .cdk-drag-animating {
      transition: transform 250ms cubic-bezier(0, 0, 0.2, 1);
    }

    @media (max-width: 1024px) {
      .builder-content {
        grid-template-columns: 1fr;
      }
      
      .header-content {
        flex-direction: column;
        text-align: center;
      }
      
      .header-actions {
        margin-top: 16px;
      }
    }

    @media (max-width: 768px) {
      .builder-container {
        padding: 16px;
      }
      
      .form-row {
        grid-template-columns: 1fr;
      }
      
      .question-types-grid {
        grid-template-columns: repeat(2, 1fr);
      }
    }
  `]
})
export class QuestionnaireBuilderComponent implements OnInit {
  builderForm!: FormGroup;
  questionForm!: FormGroup;
  categories: Category[] = [];
  questionTypes: QuestionType[] = [];
  editingQuestionIndex: number | null = null;

  constructor(
    private fb: FormBuilder,
    private questionnaireService: QuestionnaireService,
    private categoryService: CategoryService,
    private questionTypeService: QuestionTypeService,
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    private router: Router
  ) {
    this.initializeForms();
  }

  ngOnInit() {
    this.loadCategories();
    this.loadQuestionTypes();
  }

  initializeForms() {
    this.builderForm = this.fb.group({
      title: ['', Validators.required],
      description: [''],
      categoryId: ['', Validators.required],
      isActive: [true],
      isMandatory: [false],
      displayOrder: [0],
      version: [1]
    });

    this.questionForm = this.fb.group({
      questionText: ['', Validators.required],
      questionTypeId: ['', Validators.required],
      isRequired: [false],
      displayOrder: [1],
      sectionName: [''],
      helpText: [''],
      placeholder: [''],
      minLength: [null],
      maxLength: [null],
      minValue: [null],
      maxValue: [null],
      options: this.fb.array([])
    });
  }

  get questionsArray(): FormArray {
    return this.builderForm.get('questions') as FormArray;
  }

  get optionsArray(): FormArray {
    return this.questionForm.get('options') as FormArray;
  }

  loadCategories() {
    this.categoryService.getCategories().subscribe(response => {
      if (response.success) {
        this.categories = response.data;
      }
    });
  }

  loadQuestionTypes() {
    this.questionTypeService.getQuestionTypes().subscribe(response => {
      if (response.success) {
        this.questionTypes = response.data;
      }
    });
  }

  getQuestionTypeIcon(typeName: string): string {
    const iconMap: { [key: string]: string } = {
      'text': 'short_text',
      'textarea': 'subject',
      'radio': 'radio_button_checked',
      'checkbox': 'check_box',
      'select': 'arrow_drop_down',
      'multiselect': 'list',
      'number': 'dialpad',
      'date': 'calendar_today',
      'email': 'email',
      'phone': 'phone',
      'file': 'attach_file',
      'rating': 'star',
      'slider': 'tune',
      'yes_no': 'help'
    };
    return iconMap[typeName] || 'quiz';
  }

  getQuestionTypeName(typeId: string): string {
    const type = this.questionTypes.find(t => t.id === typeId);
    return type?.displayName || 'Unknown';
  }

  addQuestion(questionType: QuestionType) {
    this.editingQuestionIndex = -1;
    this.questionForm.patchValue({
      questionTypeId: questionType.id,
      displayOrder: this.questionsArray.length + 1
    });
    this.clearOptions();
  }

  editQuestion(index: number) {
    this.editingQuestionIndex = index;
    const question = this.questionsArray.at(index).value;
    this.questionForm.patchValue(question);
    this.loadOptions(question.options || []);
  }

  duplicateQuestion(index: number) {
    const question = this.questionsArray.at(index).value;
    const newQuestion = { ...question, displayOrder: this.questionsArray.length + 1 };
    this.questionsArray.push(this.fb.group(newQuestion));
  }

  deleteQuestion(index: number) {
    this.questionsArray.removeAt(index);
    this.updateQuestionNumbers();
  }

  updateQuestionNumbers() {
    for (let i = 0; i < this.questionsArray.length; i++) {
      this.questionsArray.at(i).patchValue({ displayOrder: i + 1 });
    }
  }

  saveQuestion() {
    if (this.questionForm.valid) {
      const questionData = this.questionForm.value;
      
      if (this.editingQuestionIndex === -1) {
        // Add new question
        this.questionsArray.push(this.fb.group(questionData));
      } else if (this.editingQuestionIndex !== null) {
        // Update existing question
        this.questionsArray.at(this.editingQuestionIndex).patchValue(questionData);
      }
      
      this.closeQuestionEditor();
      this.updateQuestionNumbers();
    }
  }

  closeQuestionEditor() {
    this.editingQuestionIndex = null;
    this.questionForm.reset();
    this.clearOptions();
  }

  clearOptions() {
    while (this.optionsArray.length !== 0) {
      this.optionsArray.removeAt(0);
    }
  }

  loadOptions(options: any[]) {
    this.clearOptions();
    options.forEach(option => {
      this.optionsArray.push(this.fb.group({
        optionText: [option.optionText, Validators.required],
        displayOrder: [option.displayOrder || 1]
      }));
    });
  }

  addOption() {
    this.optionsArray.push(this.fb.group({
      optionText: ['', Validators.required],
      displayOrder: [this.optionsArray.length + 1]
    }));
  }

  removeOption(index: number) {
    this.optionsArray.removeAt(index);
  }

  hasOptions(): boolean {
    const questionTypeId = this.questionForm.get('questionTypeId')?.value;
    const questionType = this.questionTypes.find(t => t.id === questionTypeId);
    return questionType?.hasOptions || false;
  }

  isTextQuestion(): boolean {
    const questionTypeId = this.questionForm.get('questionTypeId')?.value;
    const questionType = this.questionTypes.find(t => t.id === questionTypeId);
    return ['text', 'textarea', 'email', 'phone'].includes(questionType?.typeName || '');
  }

  isNumberQuestion(): boolean {
    const questionTypeId = this.questionForm.get('questionTypeId')?.value;
    const questionType = this.questionTypes.find(t => t.id === questionTypeId);
    return questionType?.typeName === 'number';
  }

  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(this.questionsArray.controls, event.previousIndex, event.currentIndex);
    this.updateQuestionNumbers();
  }

  previewQuestionnaire() {
    // TODO: Implement questionnaire preview
    this.snackBar.open('Preview functionality coming soon!', 'Close', { duration: 3000 });
  }

  saveQuestionnaire() {
    if (this.builderForm.valid && this.questionsArray.length > 0) {
      // Get current user ID from auth service
      const currentUser = this.authService.getCurrentUser();
      if (!currentUser) {
        this.snackBar.open('Please log in to create questionnaires', 'Close', { duration: 3000 });
        return;
      }
      
      const questionnaireData: CreateCategoryQuestionnaireTemplateRequest = {
        ...this.builderForm.value,
        createdBy: currentUser.id,
        questions: this.questionsArray.value.map((question: any) => ({
          ...question,
          questionnaireId: '', // Will be set by backend
          options: question.options || []
        }))
      };

      this.questionnaireService.createQuestionnaireTemplate(questionnaireData).subscribe({
        next: (response: any) => {
          if (response.success) {
            this.snackBar.open('Questionnaire saved successfully!', 'Close', { duration: 3000 });
            this.router.navigate(['/admin/questionnaires']);
          } else {
            this.snackBar.open(response.message || 'Failed to save questionnaire', 'Close', { duration: 5000 });
          }
        },
        error: (error: any) => {
          console.error('Error saving questionnaire:', error);
          let errorMessage = 'Error saving questionnaire';
          
          // Handle specific backend validation errors
          if (error.error?.message) {
            errorMessage = error.error.message;
          } else if (error.message) {
            errorMessage = error.message;
          }
          
          this.snackBar.open(errorMessage, 'Close', { duration: 5000 });
        }
      });
    } else {
      this.snackBar.open('Please fill in all required fields and add at least one question', 'Close', { duration: 3000 });
    }
  }
} 