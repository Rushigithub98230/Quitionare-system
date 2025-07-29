using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.Models;

public class QuestionOptionResponse
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid QuestionResponseId { get; set; }
    
    [Required]
    public Guid OptionId { get; set; }
    
    [MaxLength(1000)]
    public string? CustomText { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual QuestionResponse QuestionResponse { get; set; } = null!;
    public virtual QuestionOption Option { get; set; } = null!;
} 