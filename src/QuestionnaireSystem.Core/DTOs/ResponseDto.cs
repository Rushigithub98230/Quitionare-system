using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.DTOs;

public class PatientResponseDto
{
    public Guid QuestionId { get; set; }
    public string? TextResponse { get; set; }
    public decimal? NumberResponse { get; set; }
    public DateTime? DateResponse { get; set; }
    public DateTime? DatetimeResponse { get; set; }
    public bool? BooleanResponse { get; set; }
    public object? JsonResponse { get; set; }
    public List<SelectedOptionDto> SelectedOptions { get; set; } = new();
}

public class SelectedOptionDto
{
    public Guid OptionId { get; set; }
    public string? CustomText { get; set; }
}

public class SaveResponsesDto
{
    [Required]
    public Guid AssignmentId { get; set; }
    
    public bool IsDraft { get; set; } = true;
    
    public List<PatientResponseDto> Responses { get; set; } = new();
}

public class AssignQuestionnaireDto
{
    [Required]
    public Guid PatientId { get; set; }
    
    [Required]
    public Guid QuestionnaireId { get; set; }
    
    public DateTime? DueDate { get; set; }
}

public class QuestionnaireResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool IsMandatory { get; set; }
    public int Version { get; set; }
    public List<QuestionDetailDto> Questions { get; set; } = new();
}

public class ResponseSummaryDto
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid PatientId { get; set; }
    public Guid QuestionnaireId { get; set; }
    public string QuestionnaireTitle { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsDraft { get; set; }
    public int? TimeTaken { get; set; }
    public DateTime CreatedAt { get; set; }
} 