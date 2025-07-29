using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.Models;

public class QuestionOption
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid QuestionId { get; set; }
    
    [Required, MaxLength(500)]
    public string OptionText { get; set; } = string.Empty;
    
    [Required, MaxLength(255)]
    public string OptionValue { get; set; } = string.Empty;
    
    public int DisplayOrder { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool HasTextInput { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Question Question { get; set; } = null!;
    public virtual ICollection<QuestionOptionResponse> OptionResponses { get; set; } = new List<QuestionOptionResponse>();
} 