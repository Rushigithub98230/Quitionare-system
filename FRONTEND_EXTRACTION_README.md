# Frontend Extraction Guide - Angular Questionnaire System

This guide provides detailed instructions for extracting the Angular frontend from the Questionnaire System to use in another application.

## 📁 Project Structure

```
frontend/
├── src/
│   ├── app/
│   │   ├── category-questionnaire/
│   │   │   ├── components/
│   │   │   │   ├── category-dialog/
│   │   │   │   ├── category-questionnaire.component.ts
│   │   │   │   ├── question-dialog/
│   │   │   │   ├── questionnaire-dialog/
│   │   │   │   ├── response-detail-dialog/
│   │   │   │   └── ...
│   │   │   ├── models/
│   │   │   │   ├── api-response.model.ts
│   │   │   │   ├── category.model.ts
│   │   │   │   ├── question.model.ts
│   │   │   │   ├── questionnaire.model.ts
│   │   │   │   ├── response.model.ts
│   │   │   │   ├── user-question-response.model.ts
│   │   │   │   └── user.model.ts
│   │   │   ├── services/
│   │   │   │   ├── analytics.service.ts
│   │   │   │   ├── api.service.ts
│   │   │   │   ├── auth.service.ts
│   │   │   │   ├── category.service.ts
│   │   │   │   ├── file-upload.service.ts
│   │   │   │   ├── order.service.ts
│   │   │   │   ├── question.service.ts
│   │   │   │   ├── questionnaire.service.ts
│   │   │   │   ├── response.service.ts
│   │   │   │   └── user-question-response.service.ts
│   │   │   └── ...
│   │   └── ...
│   ├── environments/
│   │   └── environment.ts
│   ├── index.html
│   ├── main.ts
│   └── styles.scss
├── angular.json
├── package.json
├── tsconfig.json
└── ...
```

## 🚀 Extraction Steps

### Step 1: Copy Core Files

#### 1.1 Copy the entire `frontend/` directory
```bash
# Copy the entire frontend folder to your new project
cp -r frontend/ /path/to/your/new/project/
```

#### 1.2 Essential Files to Copy
- `src/app/category-questionnaire/` - Main application module
- `src/environments/` - Environment configuration
- `angular.json` - Angular CLI configuration
- `package.json` - Dependencies
- `tsconfig.json` - TypeScript configuration
- `src/styles.scss` - Global styles

### Step 2: Update Dependencies

#### 2.1 Core Angular Dependencies
```json
{
  "dependencies": {
    "@angular/animations": "^17.0.0",
    "@angular/common": "^17.0.0",
    "@angular/compiler": "^17.0.0",
    "@angular/core": "^17.0.0",
    "@angular/forms": "^17.0.0",
    "@angular/platform-browser": "^17.0.0",
    "@angular/platform-browser-dynamic": "^17.0.0",
    "@angular/router": "^17.0.0",
    "rxjs": "~7.8.0",
    "tslib": "^2.3.0",
    "zone.js": "~0.14.2"
  }
}
```

#### 2.2 Angular Material Dependencies
```json
{
  "dependencies": {
    "@angular/material": "^17.0.0",
    "@angular/cdk": "^17.0.0"
  }
}
```

#### 2.3 Additional Dependencies
```json
{
  "dependencies": {
    "@angular/flex-layout": "^15.0.0-beta.42",
    "chart.js": "^4.0.0",
    "ng2-charts": "^4.0.0"
  }
}
```

### Step 3: Environment Configuration

#### 3.1 Update `src/environments/environment.ts`
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api', // Update to your backend URL
  appName: 'Your Application Name'
};
```

#### 3.2 Update `src/environments/environment.prod.ts`
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://your-production-api.com/api', // Update to your production backend URL
  appName: 'Your Application Name'
};
```

### Step 4: Update Angular Configuration

#### 4.1 Update `angular.json`
```json
{
  "projects": {
    "your-app-name": {
      "root": "",
      "sourceRoot": "src",
      "projectType": "application",
      "prefix": "app",
      "architect": {
        "build": {
          "builder": "@angular-devkit/build-angular:browser",
          "options": {
            "outputPath": "dist/your-app-name",
            "index": "src/index.html",
            "main": "src/main.ts",
            "polyfills": ["zone.js"],
            "tsConfig": "tsconfig.app.json",
            "assets": [
              "src/favicon.ico",
              "src/assets"
            ],
            "styles": [
              "src/styles.scss"
            ],
            "scripts": []
          }
        }
      }
    }
  }
}
```

#### 4.2 Update `src/main.ts`
```typescript
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
```

### Step 5: Update Application Configuration

#### 5.1 Update `src/app/app.config.ts`
```typescript
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideHttpClient } from '@angular/common/http';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideAnimations(),
    provideHttpClient()
  ]
};
```

#### 5.2 Update `src/app/app.routes.ts`
```typescript
import { Routes } from '@angular/router';
import { CategoryQuestionnaireComponent } from './category-questionnaire/category-questionnaire.component';

export const routes: Routes = [
  { path: '', redirectTo: '/questionnaire', pathMatch: 'full' },
  { path: 'questionnaire', component: CategoryQuestionnaireComponent },
  { path: '**', redirectTo: '/questionnaire' }
];
```

### Step 6: Update API Service Configuration

#### 6.1 Update `src/app/category-questionnaire/services/api.service.ts`
```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = environment.apiUrl; // This will use your configured API URL

  constructor(private http: HttpClient) {}

  // ... rest of the service methods
}
```

### Step 7: Update Models and Interfaces

#### 7.1 Update model files to match your backend structure
- `src/app/category-questionnaire/models/category.model.ts`
- `src/app/category-questionnaire/models/questionnaire.model.ts`
- `src/app/category-questionnaire/models/question.model.ts`
- `src/app/category-questionnaire/models/response.model.ts`
- `src/app/category-questionnaire/models/user.model.ts`

### Step 8: Update Component Imports

#### 8.1 Update all component imports to use your new module structure
```typescript
// Update import paths in all components
import { ApiService } from '../services/api.service';
import { Category } from '../models/category.model';
// ... etc
```

## 🔧 Setup Instructions for New Project

### 1. Install Dependencies
```bash
cd /path/to/your/new/project
npm install
```

### 2. Install Angular CLI (if not already installed)
```bash
npm install -g @angular/cli
```

### 3. Install Angular Material
```bash
ng add @angular/material
```

### 4. Configure Angular Material Theme
Add to `src/styles.scss`:
```scss
@use '@angular/material' as mat;

// Include the common styles for Angular Material
@include mat.core();

// Define your theme
$primary-palette: mat.define-palette(mat.$indigo-palette);
$accent-palette: mat.define-palette(mat.$pink-palette, A200, A100, A400);
$warn-palette: mat.define-palette(mat.$red-palette);

$theme: mat.define-light-theme((
  color: (
    primary: $primary-palette,
    accent: $accent-palette,
    warn: $warn-palette,
  ),
  typography: mat.define-typography-config(),
  density: 0,
));

@include mat.all-component-themes($theme);
```

### 5. Update Index HTML
```html
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <title>Your Application Name</title>
  <base href="/">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <link rel="icon" type="image/x-icon" href="favicon.ico">
  <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500&display=swap" rel="stylesheet">
  <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
</head>
<body class="mat-typography">
  <app-root></app-root>
</body>
</html>
```

## 🎨 Customization Options

### 1. Update Application Name
- Update `src/app/app.component.ts`
- Update `src/index.html` title
- Update `angular.json` project name

### 2. Update Styling
- Modify `src/styles.scss` for global styles
- Update component-specific styles in `.scss` files
- Customize Angular Material theme colors

### 3. Update Routing
- Modify `src/app/app.routes.ts` for your routing needs
- Add new components and routes as needed

### 4. Update API Endpoints
- Update all service files to match your backend API structure
- Modify `ApiService` base URL and endpoints

## 🔍 Key Components to Customize

### 1. Authentication
- Update `src/app/category-questionnaire/services/auth.service.ts`
- Modify login/logout logic
- Update user model structure

### 2. Data Models
- Update all model interfaces to match your backend
- Modify property names and types as needed

### 3. Services
- Update all service methods to match your API
- Modify error handling and response processing

### 4. Components
- Update component templates and logic
- Modify form validation rules
- Update UI/UX to match your design requirements

## 🚨 Important Notes

### 1. Backend Compatibility
- Ensure your backend API matches the expected structure
- Update API endpoints in all service files
- Verify authentication mechanism compatibility

### 2. Environment Variables
- Update environment files with correct API URLs
- Configure production and development environments
- Set up proper CORS configuration

### 3. Dependencies
- Ensure all required dependencies are installed
- Check for version compatibility issues
- Update Angular Material theme if needed

### 4. Build Configuration
- Update build scripts in `package.json`
- Configure proper output paths
- Set up production build optimization

## 📝 Testing Checklist

- [ ] All components load without errors
- [ ] API calls work correctly
- [ ] Authentication flows properly
- [ ] Forms submit and validate correctly
- [ ] Responsive design works on all devices
- [ ] Production build completes successfully
- [ ] All routes navigate correctly
- [ ] Error handling works as expected

## 🆘 Troubleshooting

### Common Issues:

1. **Module not found errors**
   - Check import paths
   - Verify file locations
   - Update module declarations

2. **API connection errors**
   - Verify backend URL in environment files
   - Check CORS configuration
   - Verify API endpoints

3. **Styling issues**
   - Ensure Angular Material is properly configured
   - Check theme imports
   - Verify CSS file paths

4. **Build errors**
   - Check TypeScript configuration
   - Verify all dependencies are installed
   - Check Angular CLI version compatibility

## 📞 Support

If you encounter issues during extraction:
1. Check the original project structure
2. Verify all dependencies are correctly installed
3. Ensure environment configuration is correct
4. Test API connectivity
5. Review console errors for specific issues

This extraction guide should help you successfully move the Angular frontend to your new application while maintaining all functionality and customizing it to your specific needs. 