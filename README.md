# Category-Based Questionnaire System

A complete implementation of a Category-Based Questionnaire System using .NET 8 for the backend and Angular 18 with Angular Material for the frontend.

## Features

### Backend (.NET 8)
- **Category Management**: CRUD operations for questionnaire categories
- **Dynamic Questionnaire Builder**: Admin can create questionnaires with various question types
- **Question Types**: Text input, textarea, radio buttons, checkboxes, dropdown, multi-select, number, date, email, phone, file upload, rating scale, slider, yes/no
- **Image Support**: Questions can include images with alt text
- **Validation Rules**: Configurable validation per question
- **Versioning**: Questionnaire versioning to maintain data integrity
- **JWT Authentication**: Secure API with JWT token authentication
- **Entity Framework Core**: Code-first approach with migrations
- **AutoMapper**: Clean mapping between models and DTOs
- **FluentValidation**: Comprehensive input validation
- **Soft Deletes**: Data preservation with soft delete functionality

### Frontend (Angular 18 + Angular Material)
- **Responsive Design**: Modern, mobile-friendly UI
- **Admin Portal**: Dynamic form builder for questionnaires
- **User Portal**: Dynamic questionnaire rendering based on category selection
- **Material Design**: Consistent UI using Angular Material components
- **Form Validation**: Real-time validation with user-friendly error messages
- **Accessibility**: WCAG compliant components
- **Lazy Loading**: Optimized performance with route-based code splitting

## Architecture

### Backend Structure
```
src/
├── QuestionnaireSystem.Core/           # Domain models and interfaces
│   ├── Models/                         # Entity models
│   ├── DTOs/                          # Data transfer objects
│   └── Interfaces/                    # Repository interfaces
├── QuestionnaireSystem.Infrastructure/ # Data access and external services
│   ├── Data/                          # DbContext and configurations
│   └── Repositories/                  # Repository implementations
└── QuestionnaireSystem.API/           # Web API layer
    ├── Controllers/                   # API endpoints
    ├── Services/                      # Business logic services
    ├── Validators/                    # FluentValidation rules
    └── Mapping/                       # AutoMapper profiles
```

### Frontend Structure
```
frontend/src/app/
├── models/                            # TypeScript interfaces
├── services/                          # API service classes
├── features/                          # Feature modules
│   ├── categories/                    # Category management
│   ├── questionnaires/                # Questionnaire management
│   ├── questionnaire-detail/          # Questionnaire builder
│   └── admin/                         # Admin dashboard
└── shared/                            # Shared components and utilities
```

## Database Schema

### Core Entities
- **Category**: Questionnaire categories with display order and color
- **QuestionnaireTemplate**: Questionnaire definitions with versioning
- **QuestionType**: Supported question types with metadata
- **Question**: Individual questions with validation rules and options
- **QuestionOption**: Options for choice-based questions
- **PatientQuestionnaireAssignment**: Assignment of questionnaires to patients
- **PatientResponse**: Patient responses to questionnaires
- **QuestionResponse**: Individual question responses
- **QuestionOptionResponse**: Option-based responses

### Key Features
- **Soft Deletes**: All entities support soft deletion
- **Audit Trail**: Created/Updated timestamps on all entities
- **Versioning**: Questionnaire versioning for data integrity
- **Relationships**: Proper foreign key relationships with cascade rules
- **Indexes**: Optimized database performance with strategic indexes

## API Endpoints

### Categories
- `GET /api/categories` - Get all categories
- `GET /api/categories/{id}` - Get category by ID
- `POST /api/categories` - Create new category
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category

### Questionnaires
- `GET /api/questionnaires` - Get all questionnaires
- `GET /api/questionnaires/category/{categoryId}` - Get questionnaires by category
- `GET /api/questionnaires/{id}` - Get questionnaire details
- `GET /api/questionnaires/{id}/response` - Get questionnaire for response (public)
- `POST /api/questionnaires` - Create new questionnaire
- `PUT /api/questionnaires/{id}` - Update questionnaire
- `DELETE /api/questionnaires/{id}` - Delete questionnaire

### Patient Responses
- `POST /api/responses` - Save patient responses
- `GET /api/responses/patient/{patientId}` - Get patient responses
- `GET /api/responses/questionnaire/{questionnaireId}` - Get responses by questionnaire

## Question Types Supported

1. **Text Input**: Single line text input
2. **Text Area**: Multi-line text input
3. **Radio Button**: Single choice from options
4. **Checkbox**: Multiple choice from options
5. **Dropdown**: Single choice dropdown
6. **Multi-Select**: Multiple choice dropdown
7. **Number**: Numeric input with validation
8. **Date**: Date picker
9. **Email**: Email input with validation
10. **Phone**: Phone number input
11. **File Upload**: File upload with type restrictions
12. **Rating Scale**: Numeric rating input
13. **Slider**: Range slider input
14. **Yes/No**: Boolean choice

## Validation Features

### Question-Level Validation
- Required field validation
- Minimum/maximum length
- Numeric range validation
- Email format validation
- Phone number format validation
- File type restrictions
- File size limits

### Form-Level Validation
- All required questions must be answered
- Conditional logic support
- Cross-field validation
- Real-time validation feedback

## Security Features

- **JWT Authentication**: Secure token-based authentication
- **Role-Based Authorization**: Admin and user role separation
- **Input Validation**: Comprehensive server-side validation
- **CORS Configuration**: Secure cross-origin resource sharing
- **SQL Injection Prevention**: Parameterized queries with EF Core
- **XSS Prevention**: Input sanitization and output encoding

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Node.js 18+ and npm
- Angular CLI 18+

### Backend Setup
1. Clone the repository
2. Navigate to the backend directory: `cd src/QuestionnaireSystem.API`
3. Update connection string in `appsettings.json`
4. Run the application: `dotnet run`
5. The API will be available at `https://localhost:7001`

### Frontend Setup
1. Navigate to the frontend directory: `cd frontend`
2. Install dependencies: `npm install`
3. Start the development server: `npm start`
4. The application will be available at `http://localhost:4200`

### Database Setup
The application uses Entity Framework Core with code-first migrations. The database will be created automatically on first run.

## Configuration

### Backend Configuration
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=QuestionnaireSystem;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "your-super-secret-key-with-at-least-32-characters",
    "Issuer": "QuestionnaireSystem",
    "Audience": "QuestionnaireSystemUsers",
    "ExpiryInMinutes": 60
  }
}
```

### Frontend Configuration
Update the API base URL in `src/app/services/api.service.ts`:
```typescript
private baseUrl = 'https://localhost:7001/api';
```

## Usage

### Admin Workflow
1. Create categories for organizing questionnaires
2. Build questionnaires using the dynamic form builder
3. Add questions with various types and validation rules
4. Configure conditional logic and settings
5. Assign questionnaires to categories
6. Monitor responses and analytics

### User Workflow
1. Select a category to view available questionnaires
2. Fill out questionnaires with real-time validation
3. Save responses as draft or submit completed forms
4. View response history and status

## Production Deployment

### Backend Deployment
1. Build the application: `dotnet publish -c Release`
2. Deploy to Azure App Service, AWS, or on-premises
3. Configure production connection string
4. Set up SSL certificates
5. Configure CORS for production domain

### Frontend Deployment
1. Build the application: `ng build --configuration production`
2. Deploy to Azure Static Web Apps, AWS S3, or any static hosting
3. Update API base URL for production
4. Configure CDN for optimal performance

## Testing

### Backend Testing
- Unit tests for services and repositories
- Integration tests for API endpoints
- Database migration tests
- Validation tests

### Frontend Testing
- Unit tests for components and services
- Integration tests for user workflows
- E2E tests with Cypress or Playwright
- Accessibility tests

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support and questions, please open an issue in the repository or contact the development team. 