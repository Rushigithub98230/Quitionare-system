using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.Models;

public class QuestionOptionResponse : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid QuestionResponseId { get; set; }
    
    [Required]
    public Guid OptionId { get; set; }
    
    [MaxLength(1000)]
    public string? CustomText { get; set; }
    
    // Navigation properties
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual QuestionResponse QuestionResponse { get; set; } = null!;
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual QuestionOption Option { get; set; } = null!;
} 