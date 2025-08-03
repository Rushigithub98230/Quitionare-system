using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.DTOs;

public class CategoryQuestionnaireTemplateDto
{
    public Guid Id { get; set; }
    
    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public Guid CategoryId { get; set; }
    
    public string CategoryName { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public bool IsMandatory { get; set; } = false;
    
    public int DisplayOrder { get; set; } = 0;
    
    public int Version { get; set; } = 1;
    
    [Required]
    public int CreatedBy { get; set; }
    
    public string CreatedByUserName { get; set; } = string.Empty;
    
    public DateTime? CreatedDate { get; set; }
    
    public DateTime? UpdatedDate { get; set; }
    
    public DateTime? DeletedDate { get; set; }
    
    public int QuestionCount { get; set; }
    
    // Navigation properties
    public CategoryDto? Category { get; set; }
    public UserDto? CreatedByUser { get; set; }
    public List<CategoryQuestionDto> Questions { get; set; } = new List<CategoryQuestionDto>();
    public List<UserQuestionResponseDto> UserResponses { get; set; } = new List<UserQuestionResponseDto>();
}

public class CreateCategoryQuestionnaireTemplateDto
{
    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public Guid CategoryId { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsMandatory { get; set; } = false;
    
    public int DisplayOrder { get; set; } = 0;
    
    public int Version { get; set; } = 1;
    
    // TODO: Re-enable authentication for production
    public int? CreatedBy { get; set; }
    
    public List<CreateCategoryQuestionDto> Questions { get; set; } = new List<CreateCategoryQuestionDto>();
}

public class UpdateCategoryQuestionnaireTemplateDto
{
    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public Guid CategoryId { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsMandatory { get; set; } = false;
    
    public int DisplayOrder { get; set; } = 0;
    
    public int Version { get; set; } = 1;
    
    public List<CreateCategoryQuestionDto> Questions { get; set; } = new List<CreateCategoryQuestionDto>();
}

public class CategoryQuestionnaireTemplateSummaryDto
{
    public Guid Id { get; set; }
    
    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public Guid CategoryId { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsMandatory { get; set; } = false;
    
    public int DisplayOrder { get; set; } = 0;
    
    public int Version { get; set; } = 1;
    
    // TODO: Re-enable authentication for production
    public int? CreatedBy { get; set; }
    
    public DateTime? CreatedDate { get; set; }
    
    public DateTime? UpdatedDate { get; set; }
    
    public DateTime? DeletedDate { get; set; }
    
    // Navigation properties
    public CategoryDto? Category { get; set; }
    public UserDto? CreatedByUser { get; set; }
    public int QuestionCount { get; set; }
    public int ResponseCount { get; set; }
}

public class CategoryQuestionnaireTemplateWithCategoryDto
{
    public Guid Id { get; set; }
    
    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public Guid CategoryId { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsMandatory { get; set; } = false;
    
    public int DisplayOrder { get; set; } = 0;
    
    public int Version { get; set; } = 1;
    
    [Required]
    public int CreatedBy { get; set; }
    
    public DateTime? CreatedDate { get; set; }
    
    public DateTime? UpdatedDate { get; set; }
    
    public DateTime? DeletedDate { get; set; }
    
    // Navigation properties
    public CategoryDto? Category { get; set; }
    public UserDto? CreatedByUser { get; set; }
    public List<CategoryQuestionDto> Questions { get; set; } = new List<CategoryQuestionDto>();
}

public class CategoryQuestionnaireTemplateWithResponsesDto
{
    public Guid Id { get; set; }
    
    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public Guid CategoryId { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsMandatory { get; set; } = false;
    
    public int DisplayOrder { get; set; } = 0;
    
    public int Version { get; set; } = 1;
    
    [Required]
    public int CreatedBy { get; set; }
    
    public DateTime? CreatedDate { get; set; }
    
    public DateTime? UpdatedDate { get; set; }
    
    public DateTime? DeletedDate { get; set; }
    
    // Navigation properties
    public CategoryDto? Category { get; set; }
    public UserDto? CreatedByUser { get; set; }
    public List<CategoryQuestionDto> Questions { get; set; } = new List<CategoryQuestionDto>();
    public List<UserQuestionResponseDto> UserResponses { get; set; } = new List<UserQuestionResponseDto>();
} 