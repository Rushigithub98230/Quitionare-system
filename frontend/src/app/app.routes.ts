import { Routes } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Router } from '@angular/router';

const authGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  if (authService.isAuthenticated()) {
    return true;
  } else {
    router.navigate(['/login']);
    return false;
  }
};

const adminGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  if (authService.isAuthenticated() && authService.isAdmin()) {
    return true;
  } else {
    router.navigate(['/login']);
    return false;
  }
};

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'categories',
    loadComponent: () => import('./features/categories/categories.component').then(m => m.CategoriesComponent),
    canActivate: [authGuard]
  },
  {
    path: 'questionnaires',
    loadComponent: () => import('./features/questionnaires/questionnaires.component').then(m => m.QuestionnairesComponent),
    canActivate: [authGuard]
  },
  {
    path: 'questionnaire/:id',
    loadComponent: () => import('./features/questionnaire-detail/questionnaire-detail.component').then(m => m.QuestionnaireDetailComponent),
    canActivate: [authGuard]
  },
  {
    path: 'questionnaire/:id/stepper',
    loadComponent: () => import('./features/questionnaire/questionnaire-stepper/questionnaire-stepper.component').then(m => m.QuestionnaireStepperComponent),
    canActivate: [authGuard]
  },
  {
    path: 'admin',
    loadComponent: () => import('./features/admin/admin.component').then(m => m.AdminComponent),
    canActivate: [adminGuard]
  },
  {
    path: 'admin/questionnaire-builder',
    loadComponent: () => import('./features/admin/questionnaire-builder/questionnaire-builder.component').then(m => m.QuestionnaireBuilderComponent),
    canActivate: [adminGuard]
  },
  {
    path: 'admin/categories',
    loadComponent: () => import('./features/admin/categories/categories.component').then(m => m.CategoriesComponent),
    canActivate: [adminGuard]
  },
  {
    path: 'profile',
    loadComponent: () => import('./features/profile/profile.component').then(m => m.ProfileComponent),
    canActivate: [authGuard]
  },
  {
    path: 'my-responses',
    loadComponent: () => import('./features/my-responses/my-responses.component').then(m => m.MyResponsesComponent),
    canActivate: [authGuard]
  },
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
]; 