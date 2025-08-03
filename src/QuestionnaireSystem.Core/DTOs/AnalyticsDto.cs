namespace QuestionnaireSystem.Core.DTOs;

public class AnalyticsTrendsDto
{
    public int Total { get; set; }
    public int Today { get; set; }
    public int ThisWeek { get; set; }
    public int ThisMonth { get; set; }
}

public class CategoryAnalyticsDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int ResponseCount { get; set; }
    public int QuestionnaireCount { get; set; }
    public int TotalQuestionsAnswered { get; set; }
    public List<string> Questionnaires { get; set; } = new();
}

public class QuestionnaireAnalyticsDto
{
    public Guid QuestionnaireId { get; set; }
    public string QuestionnaireName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int ResponseCount { get; set; }
    public int TotalQuestionsAnswered { get; set; }
}

public class AnalyticsSummaryDto
{
    public AnalyticsTrendsDto Trends { get; set; } = new();
    public List<CategoryAnalyticsDto> TopCategories { get; set; } = new();
    public List<QuestionnaireAnalyticsDto> TopQuestionnaires { get; set; } = new();
} 