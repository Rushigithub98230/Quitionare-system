using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.Models;

public class QuestionOption : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid QuestionId { get; set; }
    
    [Required, MaxLength(500)]
    public string OptionText { get; set; } = string.Empty;
    
    [Required, MaxLength(255)]
    public string OptionValue { get; set; } = string.Empty;
    
    public int DisplayOrder { get; set; }
    
    public bool IsCorrect { get; set; } = false;
    
    public bool HasTextInput { get; set; } = false;
    
    // Navigation properties
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual CategoryQuestion Question { get; set; } = null!;
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<QuestionOptionResponse> OptionResponses { get; set; } = new List<QuestionOptionResponse>();
} 