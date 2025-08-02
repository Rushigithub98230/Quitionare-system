# Questionnaire Module

A complete, self-contained questionnaire system for Angular applications. This module provides all the functionality needed to manage questionnaires, questions, and their ordering.

## Features

- ✅ **Complete Questionnaire Management**: Create, read, update, delete questionnaires
- ✅ **Question Management**: Add, edit, delete questions with various types
- ✅ **Order Management**: Drag-and-drop reordering with validation
- ✅ **Category Support**: Organize questionnaires by categories
- ✅ **Question Types**: Multiple choice, single choice, text, number, boolean
- ✅ **Validation**: Real-time order validation and feedback
- ✅ **Responsive Design**: Works on desktop and mobile
- ✅ **Material Design**: Uses Angular Material components

## Quick Start

### 1. Install Dependencies

```bash
npm install @angular/material @angular/cdk
```

### 2. Import the Module

```typescript
import { QuestionnaireModule } from './questionnaire';

@NgModule({
  imports: [
    QuestionnaireModule
  ]
})
export class AppModule { }
```

### 3. Use Components

```typescript
import { QuestionListComponent, QuestionnaireService } from './questionnaire';

@Component({
  selector: 'app-my-component',
  template: `
    <app-question-list [questionnaire]="questionnaire"></app-question-list>
  `
})
export class MyComponent {
  constructor(private questionnaireService: QuestionnaireService) {}
}
```

## Module Structure

```
questionnaire/
├── components/
│   ├── question-list/          # Main question management component
│   └── order-manager/          # Drag-and-drop order management
├── dialogs/
│   ├── question-dialog/        # Add/edit question dialog
│   └── questionnaire-dialog/   # Add/edit questionnaire dialog
├── models/
│   ├── question.model.ts       # Question interfaces and types
│   └── questionnaire.model.ts  # Questionnaire interfaces and types
├── services/
│   ├── question.service.ts     # Question API service
│   ├── questionnaire.service.ts # Questionnaire API service
│   └── order.service.ts        # Order management service
├── utils/
│   └── (utility functions)
├── questionnaire.module.ts      # Main module file
├── index.ts                    # Public exports
└── README.md                   # This file
```

## Components

### QuestionListComponent

Main component for managing questions within a questionnaire.

```typescript
<app-question-list 
  [questionnaire]="questionnaire"
  (questionAdded)="onQuestionAdded($event)"
  (questionUpdated)="onQuestionUpdated($event)"
  (questionDeleted)="onQuestionDeleted($event)">
</app-question-list>
```

**Features:**
- Display questions in order
- Add new questions
- Edit existing questions
- Delete questions
- Order management with drag-and-drop
- Question type support

### OrderManagerComponent

Reusable component for managing item ordering.

```typescript
<app-order-manager
  [items]="orderItems"
  [title]="'Question Order'"
  (orderChanged)="onOrderChanged($event)">
</app-order-manager>
```

**Features:**
- Drag-and-drop reordering
- Visual order indicators
- Validation feedback
- Manual controls (move up/down, to top/bottom)

## Services

### QuestionnaireService

Handles questionnaire CRUD operations.

```typescript
constructor(private questionnaireService: QuestionnaireService) {}

// Get all questionnaires
this.questionnaireService.getAll().subscribe(questionnaires => {
  console.log(questionnaires);
});

// Create questionnaire
this.questionnaireService.create(questionnaireData).subscribe(result => {
  console.log('Questionnaire created:', result);
});

// Update questionnaire order
this.questionnaireService.updateOrder(questionnaires).subscribe(result => {
  console.log('Order updated:', result);
});
```

### QuestionService

Handles question CRUD operations.

```typescript
constructor(private questionService: QuestionService) {}

// Get questions for a questionnaire
this.questionService.getQuestionsByQuestionnaireId(questionnaireId).subscribe(questions => {
  console.log(questions);
});

// Update question order
this.questionService.updateQuestionOrder(questionnaireId, questions).subscribe(result => {
  console.log('Question order updated:', result);
});
```

### OrderService

Provides order validation and management utilities.

```typescript
constructor(private orderService: OrderService) {}

// Validate order
const validation = this.orderService.validateOrder(items);
if (!validation.isValid) {
  console.log('Order errors:', validation.errors);
}

// Reorder items
const reorderedItems = this.orderService.reorderItems(items);
```

## Models

### Question

```typescript
interface Question {
  id: string;
  questionText: string;
  questionTypeId: string;
  isRequired: boolean;
  displayOrder: number;
  options?: QuestionOption[];
  validationRules?: ValidationRule[];
}
```

### Questionnaire

```typescript
interface Questionnaire {
  id: string;
  title: string;
  description?: string;
  categoryId: string;
  isActive: boolean;
  isMandatory: boolean;
  displayOrder: number;
  questions?: Question[];
}
```

## Configuration

### Basic Configuration

```typescript
import { createQuestionnaireConfig } from './questionnaire';

const config = createQuestionnaireConfig({
  apiUrl: 'https://api.example.com',
  enableOrderManagement: true,
  enableDragDrop: true,
  enableValidation: true
});
```

### Feature Flags

```typescript
import { createFeatureFlags } from './questionnaire';

const flags = createFeatureFlags({
  orderManagement: true,
  dragDrop: true,
  validation: true,
  categories: true,
  questions: true,
  questionnaires: true
});
```

## API Endpoints

The module expects these API endpoints:

### Questionnaires
- `GET /categoryquestionnairetemplates` - Get all questionnaires
- `POST /categoryquestionnairetemplates` - Create questionnaire
- `PUT /categoryquestionnairetemplates/{id}` - Update questionnaire
- `DELETE /categoryquestionnairetemplates/{id}` - Delete questionnaire
- `PUT /categoryquestionnairetemplates/order` - Update questionnaire order

### Questions
- `GET /categoryquestionnairetemplates/{id}/questions` - Get questions
- `POST /categoryquestionnairetemplates/{id}/questions` - Add question
- `PUT /categoryquestionnairetemplates/{id}/questions/{questionId}` - Update question
- `DELETE /categoryquestionnairetemplates/{id}/questions/{questionId}` - Delete question
- `PUT /categoryquestionnairetemplates/{id}/questions/order` - Update question order

### Categories
- `GET /categories` - Get all categories
- `PUT /categories/order` - Update category order

## Order Management

The module includes comprehensive order management:

### Features
- **Drag-and-Drop**: Intuitive reordering
- **Visual Indicators**: Order badges and status indicators
- **Validation**: Real-time validation for duplicates and gaps
- **Auto-Reorder**: Automatic fixing of order issues
- **Manual Controls**: Move up/down, to top/bottom

### Usage

```typescript
// Enable order management
<app-order-manager
  [items]="orderItems"
  [title]="'Manage Order'"
  (orderChanged)="onOrderChanged($event)">
</app-order-manager>

// Handle order changes
onOrderChanged(orderItems: OrderItem[]) {
  // Update local state
  this.items = orderItems;
  
  // Save to backend
  this.service.updateOrder(orderItems).subscribe();
}
```

## Styling

The module uses Angular Material design. Include the Material theme:

```scss
@import '@angular/material/prebuilt-themes/indigo-pink.css';
```

Custom styles are included in the component files.

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Dependencies

- Angular 18+
- Angular Material 18+
- Angular CDK 18+
- RxJS 7+

## License

This module is part of the questionnaire system and follows the same license as the parent project.

## Support

For issues and questions, please refer to the main project documentation or create an issue in the project repository. 