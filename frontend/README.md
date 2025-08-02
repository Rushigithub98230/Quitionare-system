# Questionnaire System - Admin Panel Frontend

This is the Angular 18 frontend for the Questionnaire System Admin Panel, built with Material Design and modern Angular practices.

## 🚀 Features

### Admin Dashboard
- **Categories Management**: Create, update, and delete categories
- **Questionnaires Management**: Create, update, and delete questionnaires
- **Combined View**: Hierarchical view showing categories with their questionnaires
- **Real-time Data**: Synchronized with backend APIs
- **Responsive Design**: Works on desktop and mobile devices

### Key Components
- **Admin Dashboard**: Main component with tabbed interface
- **Category Service**: Handles all category-related API calls
- **Questionnaire Service**: Handles all questionnaire-related API calls
- **Authentication Service**: Manages user authentication and authorization

## 🏗️ Architecture

### Project Structure
```
src/app/
├── admin/
│   ├── components/
│   │   └── admin-dashboard.component.ts
│   └── services/
│       ├── category.service.ts
│       └── questionnaire.service.ts
├── core/
│   ├── models/
│   │   ├── category.model.ts
│   │   ├── questionnaire.model.ts
│   │   ├── question.model.ts
│   │   ├── user.model.ts
│   │   ├── user-question-response.model.ts
│   │   └── api-response.model.ts
│   └── services/
│       ├── api.service.ts
│       └── auth.service.ts
└── shared/
    └── components/
        ├── login/
        ├── register/
        └── profile/
```

### Core Services

#### ApiService
- Base HTTP service for all API calls
- Handles authentication headers
- Provides standardized error handling

#### AuthService
- Manages user authentication state
- Handles login, logout, and token management
- Provides role-based access control

#### CategoryService
- CRUD operations for categories
- Fetches categories with questionnaire counts
- Handles category-specific operations

#### QuestionnaireService
- CRUD operations for questionnaires
- Question management within questionnaires
- Category-specific questionnaire operations

## 🎨 UI Components

### Admin Dashboard
The main dashboard features three tabs:

1. **Categories Tab**
   - Table view of all categories
   - Status indicators (Active/Inactive)
   - Questionnaire count badges
   - Action menus for edit/delete operations

2. **Questionnaires Tab**
   - Table view of all questionnaires
   - Category associations
   - Status indicators
   - Question count badges
   - Version information

3. **Combined View Tab**
   - Hierarchical expansion panels
   - Categories with nested questionnaires
   - Card-based questionnaire display
   - Integrated management actions

### Features
- **Material Design**: Modern, accessible UI components
- **Responsive Layout**: Adapts to different screen sizes
- **Loading States**: Spinner indicators during API calls
- **Error Handling**: User-friendly error messages
- **Form Validation**: Client-side validation with Material Design
- **Confirmation Dialogs**: Safe delete operations

## 🔧 Development

### Prerequisites
- Node.js 18+
- Angular CLI 18+
- Backend API running on `http://localhost:5000`

### Installation
```bash
cd frontend
npm install
```

### Development Server
```bash
npm start
```
Navigate to `http://localhost:4200`

### Build for Production
```bash
npm run build
```

## 🔌 API Integration

### Backend Endpoints
The frontend integrates with the following backend endpoints:

#### Categories
- `GET /api/categories` - Get all categories
- `GET /api/categories/{id}` - Get category by ID
- `POST /api/categories` - Create category
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category

#### Questionnaires
- `GET /api/categoryquestionnairetemplates` - Get all questionnaires
- `GET /api/categoryquestionnairetemplates/{id}` - Get questionnaire by ID
- `POST /api/categoryquestionnairetemplates` - Create questionnaire
- `PUT /api/categoryquestionnairetemplates/{id}` - Update questionnaire
- `DELETE /api/categoryquestionnairetemplates/{id}` - Delete questionnaire

#### Questions
- `POST /api/categoryquestionnairetemplates/{id}/questions` - Add question
- `PUT /api/categoryquestionnairetemplates/{id}/questions/{questionId}` - Update question
- `DELETE /api/categoryquestionnairetemplates/{id}/questions/{questionId}` - Delete question

### Authentication
- JWT-based authentication
- Role-based access control (Admin/User)
- Automatic token refresh
- Secure API communication

## 🎯 Usage

### Admin Access
1. Register with Admin role or login with existing admin account
2. Navigate to the admin dashboard
3. Use the tabbed interface to manage categories and questionnaires

### Category Management
1. Click "Create Category" to add new categories
2. Use the action menu to edit or delete categories
3. View questionnaire counts for each category

### Questionnaire Management
1. Click "Create Questionnaire" to add new questionnaires
2. Associate questionnaires with categories
3. Manage questions within questionnaires
4. Set questionnaire properties (active, mandatory, version)

### Combined View
1. Use the "Combined View" tab for hierarchical management
2. Expand categories to see associated questionnaires
3. Manage both categories and questionnaires in one view

## 🔒 Security

- Admin-only access to dashboard
- JWT token authentication
- Role-based authorization
- Secure API communication
- Input validation and sanitization

## 🚀 Future Enhancements

- Question type management
- Advanced form builders
- Response analytics
- Bulk operations
- Export/import functionality
- Real-time notifications
- Advanced search and filtering

## 📝 Notes

- The frontend is designed to work with the existing .NET backend
- All API responses follow the standardized JsonModel format
- Material Design provides consistent, accessible UI components
- The application is fully responsive and mobile-friendly 