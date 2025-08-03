using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.DTOs;

public class CategoryQuestionDto
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid QuestionnaireId { get; set; }
    
    [Required]
    public string QuestionText { get; set; } = string.Empty;
    
    [Required]
    public Guid QuestionTypeId { get; set; }
    
    public string QuestionTypeName { get; set; } = string.Empty;
    
    public bool IsRequired { get; set; } = false;
    
    public int DisplayOrder { get; set; }
    
    [MaxLength(100)]
    public string? SectionName { get; set; }
    
    public string? HelpText { get; set; }
    
    [MaxLength(255)]
    public string? Placeholder { get; set; }
    
    // Validation properties
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    
    // Image support
    public string? ImageUrl { get; set; }
    
    public string? ImageAltText { get; set; }
    
    // Validation and settings
    public string? ValidationRules { get; set; } // JSON string
    
    public string? ConditionalLogic { get; set; } // JSON string
    
    public string? Settings { get; set; } // JSON string
    
    public DateTime? CreatedDate { get; set; }
    
    public DateTime? UpdatedDate { get; set; }
    
    public DateTime? DeletedDate { get; set; }
    
    // Navigation properties
    public CategoryQuestionnaireTemplateDto? Questionnaire { get; set; }
    public QuestionTypeDto? QuestionType { get; set; }
    public List<QuestionOptionDto> Options { get; set; } = new List<QuestionOptionDto>();
    public List<QuestionResponseDto> Responses { get; set; } = new List<QuestionResponseDto>();
}

public class CreateCategoryQuestionDto
{
    [Required]
    public Guid QuestionnaireId { get; set; }
    
    [Required]
    public string QuestionText { get; set; } = string.Empty;
    
    [Required]
    public Guid QuestionTypeId { get; set; }
    
    public bool IsRequired { get; set; } = false;
    
    public int? DisplayOrder { get; set; } // Optional - will be auto-generated
    
    [MaxLength(100)]
    public string? SectionName { get; set; }
    
    public string? HelpText { get; set; }
    
    [MaxLength(255)]
    public string? Placeholder { get; set; }
    
    // Validation properties
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    
    // Image support
    public string? ImageUrl { get; set; }
    
    public string? ImageAltText { get; set; }
    
    // Validation and settings
    public string? ValidationRules { get; set; } // JSON string
    
    public string? ConditionalLogic { get; set; } // JSON string
    
    public string? Settings { get; set; } // JSON string
    
    public List<CreateQuestionOptionDto> Options { get; set; } = new List<CreateQuestionOptionDto>();
}

public class UpdateCategoryQuestionDto
{
    [Required]
    public string QuestionText { get; set; } = string.Empty;
    
    [Required]
    public Guid QuestionTypeId { get; set; }
    
    public bool IsRequired { get; set; } = false;
    
    public int? DisplayOrder { get; set; } // Optional - will be auto-generated if not provided
    
    [MaxLength(100)]
    public string? SectionName { get; set; }
    
    public string? HelpText { get; set; }
    
    [MaxLength(255)]
    public string? Placeholder { get; set; }
    
    // Validation properties
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    
    // Image support
    public string? ImageUrl { get; set; }
    
    public string? ImageAltText { get; set; }
    
    // Validation and settings
    public string? ValidationRules { get; set; } // JSON string
    
    public string? ConditionalLogic { get; set; } // JSON string
    
    public string? Settings { get; set; } // JSON string
    
    public List<UpdateQuestionOptionDto> Options { get; set; } = new List<UpdateQuestionOptionDto>();
}

public class QuestionOrderUpdateDto
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public int DisplayOrder { get; set; }
} 