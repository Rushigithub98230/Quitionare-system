import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { ApiService } from './api.service';
import { Response, ResponseAnalytics, ResponseFilter, ResponseExportOptions } from '../models/response.model';
import { ApiResponse } from '../models/api-response.model';

export interface AnalyticsTrends {
  total: number;
  today: number;
  thisWeek: number;
  thisMonth: number;
  lastWeek: number;
  lastMonth: number;
  growthRate: number;
}

export interface CategoryAnalytics {
  categoryId: string;
  categoryName: string;
  responseCount: number;
  questionnaireCount: number;
  totalQuestions: number;
  questionnaires: string[];
  averageCompletionPercentage: number;
  averageCompletionTime: number;
  completionRate: number;
}

export interface QuestionnaireAnalytics {
  questionnaireId: string;
  questionnaireName: string;
  categoryId: string;
  categoryName: string;
  responseCount: number;
  totalQuestions: number;
  averageCompletionPercentage: number;
  averageCompletionTime: number;
  completionRate: number;
  engagementScore: number;
}

export interface AnalyticsSummary {
  trends: AnalyticsTrends;
  topCategories: CategoryAnalytics[];
  topQuestionnaires: QuestionnaireAnalytics[];
  responseDistribution: {
    completed: number;
    draft: number;
    total: number;
  };
  averageMetrics: {
    completionTime: number;
    completionPercentage: number;
    questionsPerResponse: number;
  };
}

export interface DateRangeAnalytics {
  startDate: string;
  endDate: string;
  responses: Response[];
  analytics: ResponseAnalytics;
}

export interface FilteredAnalytics {
  filter: ResponseFilter;
  responses: Response[];
  analytics: ResponseAnalytics;
  summary: AnalyticsSummary;
}

@Injectable({
  providedIn: 'root'
})
export class AnalyticsService {
  private analyticsCache = new BehaviorSubject<AnalyticsSummary | null>(null);
  private lastFetchTime = 0;
  private readonly CACHE_DURATION = 5 * 60 * 1000; // 5 minutes

  constructor(private apiService: ApiService) {}

  // Get cached analytics or fetch from server
  getAnalyticsSummary(forceRefresh = false): Observable<ApiResponse<AnalyticsSummary>> {
    const now = Date.now();
    const cached = this.analyticsCache.value;
    
    if (!forceRefresh && cached && (now - this.lastFetchTime) < this.CACHE_DURATION) {
      return new Observable(observer => {
        observer.next({ success: true, data: cached, message: 'Cached data', statusCode: 200 });
        observer.complete();
      });
    }

    return this.apiService.get<AnalyticsSummary>('/responses/analytics/summary');
  }

  getAnalyticsTrends(): Observable<ApiResponse<AnalyticsTrends>> {
    return this.apiService.get<AnalyticsTrends>('/responses/analytics/trends');
  }

  getCategoryAnalytics(): Observable<ApiResponse<CategoryAnalytics[]>> {
    return this.apiService.get<CategoryAnalytics[]>('/responses/analytics/categories');
  }

  getQuestionnaireAnalytics(): Observable<ApiResponse<QuestionnaireAnalytics[]>> {
    return this.apiService.get<QuestionnaireAnalytics[]>('/responses/analytics/questionnaires');
  }

  // Enhanced analytics with filtering
  getFilteredAnalytics(filter: ResponseFilter): Observable<ApiResponse<FilteredAnalytics>> {
    let params: any = {};
    
    if (filter.categoryId) {
      params.categoryId = filter.categoryId;
    }
    if (filter.questionnaireId) {
      params.questionnaireId = filter.questionnaireId;
    }
    if (filter.dateRange) {
      params.dateRange = filter.dateRange;
    }
    if (filter.completionStatus) {
      params.completionStatus = filter.completionStatus;
    }
    if (filter.userId) {
      params.userId = filter.userId;
    }
    if (filter.startDate) {
      params.startDate = filter.startDate;
    }
    if (filter.endDate) {
      params.endDate = filter.endDate;
    }

    return this.apiService.get<FilteredAnalytics>('/responses/analytics/filtered', params);
  }

  // Get analytics for specific date range
  getDateRangeAnalytics(startDate: string, endDate: string): Observable<ApiResponse<DateRangeAnalytics>> {
    const params = {
      startDate: startDate,
      endDate: endDate
    };

    return this.apiService.get<DateRangeAnalytics>('/responses/analytics/date-range', params);
  }

  // Get detailed response analytics
  getResponseAnalytics(responseId: string): Observable<ApiResponse<ResponseAnalytics>> {
    return this.apiService.get<ResponseAnalytics>(`/responses/analytics/response/${responseId}`);
  }

  // Get real-time analytics
  getRealTimeAnalytics(): Observable<ApiResponse<AnalyticsSummary>> {
    return this.apiService.get<AnalyticsSummary>('/responses/analytics/realtime');
  }

  // Export analytics data
  exportAnalytics(options: ResponseExportOptions): Observable<Blob> {
    return this.apiService.getBlob('/responses/analytics/export', options);
  }

  // Get performance metrics
  getPerformanceMetrics(): Observable<ApiResponse<{
    averageResponseTime: number;
    completionRate: number;
    userSatisfaction: number;
    topPerformingCategories: CategoryAnalytics[];
    topPerformingQuestionnaires: QuestionnaireAnalytics[];
  }>> {
    return this.apiService.get<any>('/responses/analytics/performance');
  }

  // Get engagement analytics
  getEngagementAnalytics(): Observable<ApiResponse<{
    dailyActiveUsers: number;
    weeklyActiveUsers: number;
    monthlyActiveUsers: number;
    averageSessionDuration: number;
    bounceRate: number;
    retentionRate: number;
  }>> {
    return this.apiService.get<any>('/responses/analytics/engagement');
  }

  // Get comparative analytics
  getComparativeAnalytics(period1: string, period2: string): Observable<ApiResponse<{
    period1: AnalyticsSummary;
    period2: AnalyticsSummary;
    comparison: {
      responseGrowth: number;
      completionRateChange: number;
      averageTimeChange: number;
      topCategoriesChange: CategoryAnalytics[];
      topQuestionnairesChange: QuestionnaireAnalytics[];
    };
  }>> {
    const params = {
      period1: period1,
      period2: period2
    };

    return this.apiService.get<any>('/responses/analytics/comparative', params);
  }

  // Get predictive analytics
  getPredictiveAnalytics(): Observable<ApiResponse<{
    predictedResponses: number;
    predictedCompletionRate: number;
    trendingCategories: CategoryAnalytics[];
    trendingQuestionnaires: QuestionnaireAnalytics[];
    recommendations: string[];
  }>> {
    return this.apiService.get<any>('/responses/analytics/predictive');
  }

  // Clear analytics cache
  clearCache(): void {
    this.analyticsCache.next(null);
    this.lastFetchTime = 0;
  }

  // Get cached analytics
  getCachedAnalytics(): AnalyticsSummary | null {
    return this.analyticsCache.value;
  }

  // Check if cache is valid
  isCacheValid(): boolean {
    const now = Date.now();
    return this.analyticsCache.value !== null && (now - this.lastFetchTime) < this.CACHE_DURATION;
  }

  // Force refresh analytics
  refreshAnalytics(): Observable<ApiResponse<AnalyticsSummary>> {
    return this.getAnalyticsSummary(true);
  }

  // Get analytics for specific category
  getCategorySpecificAnalytics(categoryId: string): Observable<ApiResponse<CategoryAnalytics>> {
    return this.apiService.get<CategoryAnalytics>(`/responses/analytics/categories/${categoryId}`);
  }

  // Get analytics for specific questionnaire
  getQuestionnaireSpecificAnalytics(questionnaireId: string): Observable<ApiResponse<QuestionnaireAnalytics>> {
    return this.apiService.get<QuestionnaireAnalytics>(`/responses/analytics/questionnaires/${questionnaireId}`);
  }

  // Get user-specific analytics
  getUserAnalytics(userId: string): Observable<ApiResponse<{
    totalResponses: number;
    completedResponses: number;
    averageCompletionTime: number;
    favoriteCategories: CategoryAnalytics[];
    favoriteQuestionnaires: QuestionnaireAnalytics[];
    activityTrend: AnalyticsTrends;
  }>> {
    return this.apiService.get<any>(`/responses/analytics/users/${userId}`);
  }

  // Get system health analytics
  getSystemHealthAnalytics(): Observable<ApiResponse<{
    totalUsers: number;
    activeUsers: number;
    systemPerformance: number;
    errorRate: number;
    uptime: number;
    responseTime: number;
  }>> {
    return this.apiService.get<any>('/responses/analytics/system-health');
  }
} 