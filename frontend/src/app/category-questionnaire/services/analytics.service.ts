import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Response, ResponseAnalytics, ResponseFilter, ResponseExportOptions } from '../models/response.model';

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

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
  statusCode: number;
}

@Injectable({
  providedIn: 'root'
})
export class AnalyticsService {
  private apiUrl = `${environment.apiUrl}/responses`;
  private analyticsCache = new BehaviorSubject<AnalyticsSummary | null>(null);
  private lastFetchTime = 0;
  private readonly CACHE_DURATION = 5 * 60 * 1000; // 5 minutes

  constructor(private http: HttpClient) {}

  // Get cached analytics or fetch from server
  getAnalyticsSummary(forceRefresh = false): Observable<AnalyticsSummary> {
    const now = Date.now();
    const cached = this.analyticsCache.value;
    
    if (!forceRefresh && cached && (now - this.lastFetchTime) < this.CACHE_DURATION) {
      return new Observable(observer => {
        observer.next(cached);
        observer.complete();
      });
    }

    return this.http.get<ApiResponse<AnalyticsSummary>>(`${this.apiUrl}/analytics/summary`)
      .pipe(
        map(response => response.data),
        tap(data => {
          this.analyticsCache.next(data);
          this.lastFetchTime = now;
        })
      );
  }

  getAnalyticsTrends(): Observable<AnalyticsTrends> {
    return this.http.get<ApiResponse<AnalyticsTrends>>(`${this.apiUrl}/analytics/trends`)
      .pipe(map(response => response.data));
  }

  getCategoryAnalytics(): Observable<CategoryAnalytics[]> {
    return this.http.get<ApiResponse<CategoryAnalytics[]>>(`${this.apiUrl}/analytics/categories`)
      .pipe(map(response => response.data));
  }

  getQuestionnaireAnalytics(): Observable<QuestionnaireAnalytics[]> {
    return this.http.get<ApiResponse<QuestionnaireAnalytics[]>>(`${this.apiUrl}/analytics/questionnaires`)
      .pipe(map(response => response.data));
  }

  // Enhanced analytics with filtering
  getFilteredAnalytics(filter: ResponseFilter): Observable<FilteredAnalytics> {
    let params = new HttpParams();
    
    if (filter.categoryId) {
      params = params.set('categoryId', filter.categoryId);
    }
    if (filter.questionnaireId) {
      params = params.set('questionnaireId', filter.questionnaireId);
    }
    if (filter.dateRange) {
      params = params.set('dateRange', filter.dateRange);
    }
    if (filter.completionStatus) {
      params = params.set('completionStatus', filter.completionStatus);
    }
    if (filter.userId) {
      params = params.set('userId', filter.userId);
    }
    if (filter.startDate) {
      params = params.set('startDate', filter.startDate);
    }
    if (filter.endDate) {
      params = params.set('endDate', filter.endDate);
    }

    return this.http.get<ApiResponse<FilteredAnalytics>>(`${this.apiUrl}/analytics/filtered`, { params })
      .pipe(map(response => response.data));
  }

  // Get analytics for specific date range
  getDateRangeAnalytics(startDate: string, endDate: string): Observable<DateRangeAnalytics> {
    const params = new HttpParams()
      .set('startDate', startDate)
      .set('endDate', endDate);

    return this.http.get<ApiResponse<DateRangeAnalytics>>(`${this.apiUrl}/analytics/date-range`, { params })
      .pipe(map(response => response.data));
  }

  // Get detailed response analytics
  getResponseAnalytics(responseId: string): Observable<ResponseAnalytics> {
    return this.http.get<ApiResponse<ResponseAnalytics>>(`${this.apiUrl}/analytics/response/${responseId}`)
      .pipe(map(response => response.data));
  }

  // Get real-time analytics
  getRealTimeAnalytics(): Observable<AnalyticsSummary> {
    return this.http.get<ApiResponse<AnalyticsSummary>>(`${this.apiUrl}/analytics/realtime`)
      .pipe(map(response => response.data));
  }

  // Export analytics data
  exportAnalytics(options: ResponseExportOptions): Observable<Blob> {
    return this.http.post(`${this.apiUrl}/analytics/export`, options, { responseType: 'blob' });
  }

  // Get performance metrics
  getPerformanceMetrics(): Observable<{
    averageResponseTime: number;
    completionRate: number;
    userSatisfaction: number;
    topPerformingCategories: CategoryAnalytics[];
    topPerformingQuestionnaires: QuestionnaireAnalytics[];
  }> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/analytics/performance`)
      .pipe(map(response => response.data));
  }

  // Get engagement analytics
  getEngagementAnalytics(): Observable<{
    dailyActiveUsers: number;
    weeklyActiveUsers: number;
    monthlyActiveUsers: number;
    averageSessionDuration: number;
    bounceRate: number;
    retentionRate: number;
  }> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/analytics/engagement`)
      .pipe(map(response => response.data));
  }

  // Get comparative analytics
  getComparativeAnalytics(period1: string, period2: string): Observable<{
    period1: AnalyticsSummary;
    period2: AnalyticsSummary;
    comparison: {
      responseGrowth: number;
      completionRateChange: number;
      averageTimeChange: number;
      topCategoriesChange: CategoryAnalytics[];
      topQuestionnairesChange: QuestionnaireAnalytics[];
    };
  }> {
    const params = new HttpParams()
      .set('period1', period1)
      .set('period2', period2);

    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/analytics/comparative`, { params })
      .pipe(map(response => response.data));
  }

  // Get predictive analytics
  getPredictiveAnalytics(): Observable<{
    predictedResponses: number;
    predictedCompletionRate: number;
    trendingCategories: CategoryAnalytics[];
    trendingQuestionnaires: QuestionnaireAnalytics[];
    recommendations: string[];
  }> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/analytics/predictive`)
      .pipe(map(response => response.data));
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
  refreshAnalytics(): Observable<AnalyticsSummary> {
    return this.getAnalyticsSummary(true);
  }

  // Get analytics for specific category
  getCategorySpecificAnalytics(categoryId: string): Observable<CategoryAnalytics> {
    return this.http.get<ApiResponse<CategoryAnalytics>>(`${this.apiUrl}/analytics/categories/${categoryId}`)
      .pipe(map(response => response.data));
  }

  // Get analytics for specific questionnaire
  getQuestionnaireSpecificAnalytics(questionnaireId: string): Observable<QuestionnaireAnalytics> {
    return this.http.get<ApiResponse<QuestionnaireAnalytics>>(`${this.apiUrl}/analytics/questionnaires/${questionnaireId}`)
      .pipe(map(response => response.data));
  }

  // Get user-specific analytics
  getUserAnalytics(userId: string): Observable<{
    totalResponses: number;
    completedResponses: number;
    averageCompletionTime: number;
    favoriteCategories: CategoryAnalytics[];
    favoriteQuestionnaires: QuestionnaireAnalytics[];
    activityTrend: AnalyticsTrends;
  }> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/analytics/users/${userId}`)
      .pipe(map(response => response.data));
  }

  // Get system health analytics
  getSystemHealthAnalytics(): Observable<{
    totalUsers: number;
    activeUsers: number;
    systemPerformance: number;
    errorRate: number;
    uptime: number;
    responseTime: number;
  }> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/analytics/system-health`)
      .pipe(map(response => response.data));
  }
} 