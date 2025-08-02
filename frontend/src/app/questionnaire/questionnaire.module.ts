import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatMenuModule } from '@angular/material/menu';
import { MatBadgeModule } from '@angular/material/badge';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { DragDropModule } from '@angular/cdk/drag-drop';

// Components
import { QuestionListComponent } from './components/question-list/question-list.component';
import { OrderManagerComponent } from './components/order-manager/order-manager.component';
import { QuestionnairePreviewComponent } from './components/questionnaire-preview/questionnaire-preview.component';
import { PreviewResultsComponent } from './components/preview-results/preview-results.component';

// Dialogs
import { QuestionDialogComponent } from './dialogs/question-dialog/question-dialog.component';
import { QuestionnaireDialogComponent } from './dialogs/questionnaire-dialog/questionnaire-dialog.component';

// Services
import { QuestionService } from './services/question.service';
import { QuestionnaireService } from './services/questionnaire.service';
import { OrderService } from './services/order.service';

// Models
export * from './models/question.model';
export * from './models/questionnaire.model';

// Services
export { QuestionService } from './services/question.service';
export { QuestionnaireService } from './services/questionnaire.service';
export { OrderService } from './services/order.service';

// Components
export { QuestionListComponent } from './components/question-list/question-list.component';
export { OrderManagerComponent } from './components/order-manager/order-manager.component';
export { QuestionnairePreviewComponent } from './components/questionnaire-preview/questionnaire-preview.component';
export { PreviewResultsComponent } from './components/preview-results/preview-results.component';

// Dialogs
export { QuestionDialogComponent } from './dialogs/question-dialog/question-dialog.component';
export { QuestionnaireDialogComponent } from './dialogs/questionnaire-dialog/questionnaire-dialog.component';

@NgModule({
  declarations: [
    QuestionListComponent,
    OrderManagerComponent,
    QuestionDialogComponent,
    QuestionnaireDialogComponent,
    QuestionnairePreviewComponent,
    PreviewResultsComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatExpansionModule,
    MatListModule,
    MatDividerModule,
    MatTooltipModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatMenuModule,
    MatBadgeModule,
    MatTabsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    DragDropModule
  ],
  exports: [
    // Components
    QuestionListComponent,
    OrderManagerComponent,
    QuestionnairePreviewComponent,
    PreviewResultsComponent,
    
    // Dialogs
    QuestionDialogComponent,
    QuestionnaireDialogComponent,
    
    // Services
    QuestionService,
    QuestionnaireService,
    OrderService,
    
    // Models
    // (exported via export * statements above)
  ],
  providers: [
    QuestionService,
    QuestionnaireService,
    OrderService
  ]
})
export class QuestionnaireModule { } 