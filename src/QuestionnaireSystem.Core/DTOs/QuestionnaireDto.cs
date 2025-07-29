using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.DTOs;

public class CreateQuestionnaireDto
{
    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public Guid CategoryId { get; set; }
    
    public bool IsMandatory { get; set; } = false;
    
    public int DisplayOrder { get; set; } = 0;
    
    public List<CreateQuestionDto> Questions { get; set; } = new();
}

public class UpdateQuestionnaireDto
{
    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public Guid CategoryId { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsMandatory { get; set; } = false;
    
    public int DisplayOrder { get; set; } = 0;
    
    public List<UpdateQuestionDto> Questions { get; set; } = new();
}

public class CreateQuestionDto
{
    [Required]
    public string QuestionText { get; set; } = string.Empty;
    
    [Required]
    public Guid QuestionTypeId { get; set; }
    
    public bool IsRequired { get; set; } = false;
    
    public int DisplayOrder { get; set; }
    
    [MaxLength(100)]
    public string? SectionName { get; set; }
    
    public string? HelpText { get; set; }
    
    [MaxLength(255)]
    public string? Placeholder { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public string? ImageAltText { get; set; }
    
    public object? ValidationRules { get; set; }
    
    public object? ConditionalLogic { get; set; }
    
    public object? Settings { get; set; }
    
    public List<CreateQuestionOptionDto> Options { get; set; } = new();
}

public class UpdateQuestionDto : CreateQuestionDto
{
    public Guid? Id { get; set; }
}

public class CreateQuestionOptionDto
{
    [Required, MaxLength(500)]
    public string OptionText { get; set; } = string.Empty;
    
    [Required, MaxLength(255)]
    public string OptionValue { get; set; } = string.Empty;
    
    public int DisplayOrder { get; set; }
    
    public bool HasTextInput { get; set; } = false;
}

public class QuestionnaireDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsMandatory { get; set; }
    public int DisplayOrder { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<QuestionDetailDto> Questions { get; set; } = new();
}

public class QuestionDetailDto
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public string QuestionTypeDisplayName { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? SectionName { get; set; }
    public string? HelpText { get; set; }
    public string? Placeholder { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageAltText { get; set; }
    public object? ValidationRules { get; set; }
    public object? ConditionalLogic { get; set; }
    public object? Settings { get; set; }
    public List<QuestionOptionDetailDto> Options { get; set; } = new();
}

public class QuestionOptionDetailDto
{
    public Guid Id { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public string OptionValue { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool HasTextInput { get; set; }
} 