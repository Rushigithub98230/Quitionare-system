using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.DTOs;

public class QuestionOptionDto
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid QuestionId { get; set; }
    
    [Required]
    public string OptionText { get; set; } = string.Empty;
    
    public int DisplayOrder { get; set; }
    
    public string? Value { get; set; }
    
    public bool IsCorrect { get; set; } = false;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public DateTime? DeletedAt { get; set; }
    
    // Navigation properties
    public CategoryQuestionDto? Question { get; set; }
}

public class CreateQuestionOptionDto
{
    [Required]
    public Guid QuestionId { get; set; }
    
    [Required]
    public string OptionText { get; set; } = string.Empty;
    
    public int DisplayOrder { get; set; }
    
    public string? Value { get; set; }
    
    public bool IsCorrect { get; set; } = false;
}

public class UpdateQuestionOptionDto
{
    [Required]
    public string OptionText { get; set; } = string.Empty;
    
    public int DisplayOrder { get; set; }
    
    public string? Value { get; set; }
    
    public bool IsCorrect { get; set; } = false;
} 