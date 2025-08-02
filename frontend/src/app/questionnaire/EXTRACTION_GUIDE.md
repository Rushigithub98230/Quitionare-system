# Questionnaire Module Extraction Guide

This guide explains how to extract and use the questionnaire module in other Angular projects.

## 📁 Current Structure

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
├── utils/                      # Utility functions
├── questionnaire.module.ts      # Main module file
├── index.ts                    # Public exports
├── package.json                # Module package info
├── README.md                   # Comprehensive documentation
├── extract.sh                  # Linux/Mac extraction script
├── extract.ps1                 # Windows extraction script
└── EXTRACTION_GUIDE.md        # This file
```

## 🚀 Quick Extraction

### Method 1: Using Extraction Scripts

**Linux/Mac:**
```bash
cd frontend/src/app/questionnaire
chmod +x extract.sh
./extract.sh /path/to/your/angular-project/src/app
```

**Windows (PowerShell):**
```powershell
cd frontend/src/app/questionnaire
.\extract.ps1 "C:\path\to\your\angular-project\src\app"
```

### Method 2: Manual Copy

1. Copy the entire `questionnaire` folder to your project's `src/app/` directory
2. Install required dependencies
3. Import the module

## 📦 Installation Steps

### 1. Install Dependencies

```bash
npm install @angular/material @angular/cdk
```

### 2. Import the Module

In your `app.module.ts` or standalone component:

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

## 🔧 Configuration

### Basic Setup

```typescript
import { createQuestionnaireConfig } from './questionnaire';

const config = createQuestionnaireConfig({
  apiUrl: 'https://your-api.com',
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

## 📋 API Requirements

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

## 🎯 Usage Examples

### Basic Question List

```typescript
@Component({
  selector: 'app-questionnaire-page',
  template: `
    <app-question-list 
      [questionnaire]="questionnaire"
      (questionAdded)="onQuestionAdded($event)"
      (questionUpdated)="onQuestionUpdated($event)"
      (questionDeleted)="onQuestionDeleted($event)">
    </app-question-list>
  `
})
export class QuestionnairePageComponent {
  questionnaire: Questionnaire = {
    id: '1',
    title: 'My Questionnaire',
    description: 'A sample questionnaire',
    categoryId: '1',
    isActive: true,
    isMandatory: false,
    displayOrder: 1
  };

  onQuestionAdded(question: Question) {
    console.log('Question added:', question);
  }

  onQuestionUpdated(question: Question) {
    console.log('Question updated:', question);
  }

  onQuestionDeleted(questionId: string) {
    console.log('Question deleted:', questionId);
  }
}
```

### Order Management

```typescript
@Component({
  selector: 'app-order-management',
  template: `
    <app-order-manager
      [items]="orderItems"
      [title]="'Question Order'"
      (orderChanged)="onOrderChanged($event)">
    </app-order-manager>
  `
})
export class OrderManagementComponent {
  orderItems: OrderItem[] = [
    { id: '1', displayOrder: 1, name: 'Question 1', type: 'question' },
    { id: '2', displayOrder: 2, name: 'Question 2', type: 'question' }
  ];

  onOrderChanged(items: OrderItem[]) {
    console.log('Order changed:', items);
    // Save to backend
    this.questionService.updateQuestionOrder(questionnaireId, items).subscribe();
  }
}
```

### Service Usage

```typescript
@Component({
  selector: 'app-questionnaire-service-demo'
})
export class QuestionnaireServiceDemoComponent {
  constructor(
    private questionnaireService: QuestionnaireService,
    private questionService: QuestionService,
    private orderService: OrderService
  ) {}

  // Get all questionnaires
  loadQuestionnaires() {
    this.questionnaireService.getAll().subscribe(questionnaires => {
      console.log('Questionnaires:', questionnaires);
    });
  }

  // Create a new questionnaire
  createQuestionnaire() {
    const questionnaire = {
      title: 'New Questionnaire',
      description: 'Description',
      categoryId: '1',
      isActive: true,
      isMandatory: false,
      displayOrder: 1
    };

    this.questionnaireService.create(questionnaire).subscribe(result => {
      console.log('Created:', result);
    });
  }

  // Validate order
  validateOrder(items: OrderItem[]) {
    const validation = this.orderService.validateOrder(items);
    if (!validation.isValid) {
      console.log('Order errors:', validation.errors);
    }
  }
}
```

## 🎨 Styling

### Material Theme

Include Angular Material theme in your `styles.scss`:

```scss
@import '@angular/material/prebuilt-themes/indigo-pink.css';
```

### Custom Styles

The module includes its own styles, but you can override them:

```scss
// Override questionnaire styles
.questionnaire-module {
  .question-list {
    // Your custom styles
  }
  
  .order-manager {
    // Your custom styles
  }
}
```

## 🔍 Troubleshooting

### Common Issues

1. **Module not found**
   - Ensure the questionnaire folder is copied to `src/app/`
   - Check import paths in `questionnaire.module.ts`

2. **Material components not working**
   - Install `@angular/material` and `@angular/cdk`
   - Import Material modules in your app

3. **API errors**
   - Verify API endpoints match the expected format
   - Check CORS settings on your backend

4. **Order management not working**
   - Ensure `@angular/cdk/drag-drop` is installed
   - Check browser console for errors

### Debug Mode

Enable debug logging:

```typescript
import { QUESTIONNAIRE_CONSTANTS } from './questionnaire';

console.log('Questionnaire constants:', QUESTIONNAIRE_CONSTANTS);
```

## 📚 Additional Resources

- [README.md](./README.md) - Comprehensive module documentation
- [index.ts](./index.ts) - All available exports
- [questionnaire.module.ts](./questionnaire.module.ts) - Module configuration

## 🤝 Support

For issues or questions:
1. Check the [README.md](./README.md) for detailed documentation
2. Review the [index.ts](./index.ts) for available exports
3. Check the browser console for error messages
4. Verify API endpoints match the expected format

## 📄 License

This module is part of the questionnaire system and follows the same license as the parent project. 