using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.Models;

public class CategoryQuestion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid QuestionnaireId { get; set; }
    
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
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? DeletedAt { get; set; }
    
    // Navigation properties
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual CategoryQuestionnaireTemplate Questionnaire { get; set; } = null!;
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual QuestionType QuestionType { get; set; } = null!;
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<QuestionResponse> Responses { get; set; } = new List<QuestionResponse>();
} 