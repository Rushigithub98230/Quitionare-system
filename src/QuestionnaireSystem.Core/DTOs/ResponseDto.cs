using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.DTOs;

public class QuestionOptionResponseDto
{
    [Required]
    public Guid OptionId { get; set; }
    public string? CustomText { get; set; }
}

public class ResponseSummaryDto
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public Guid QuestionnaireId { get; set; }
    public string QuestionnaireTitle { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsDraft { get; set; }
    public int? TimeTaken { get; set; }
    public DateTime CreatedDate { get; set; }
    public int QuestionCount { get; set; }
}

public class ResponseDetailDto : ResponseSummaryDto
{
    public List<QuestionResponseDetailDto> QuestionResponses { get; set; } = new();
}

public class QuestionResponseDetailDto
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public string? TextResponse { get; set; }
    public decimal? NumberResponse { get; set; }
    public DateTime? DateResponse { get; set; }
    public DateTime? DatetimeResponse { get; set; }
    public bool? BooleanResponse { get; set; }
    public string? JsonResponse { get; set; }
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
    public int? FileSize { get; set; }
    public string? FileType { get; set; }
    public List<QuestionOptionResponseDetailDto> OptionResponses { get; set; } = new();
}

public class QuestionOptionResponseDetailDto
{
    public Guid Id { get; set; }
    public Guid OptionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public string OptionValue { get; set; } = string.Empty;
    public string? CustomText { get; set; }
} 