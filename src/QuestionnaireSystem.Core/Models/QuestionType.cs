using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.Models;

public class QuestionType : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, MaxLength(50)]
    public string TypeName { get; set; } = string.Empty;
    
    [Required, MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public bool HasOptions { get; set; } = false;
    
    public bool SupportsFileUpload { get; set; } = false;
    
    public bool SupportsImage { get; set; } = false;
    
    public string? ValidationRules { get; set; } // JSON string
    
    // Navigation properties
    public virtual ICollection<CategoryQuestion> Questions { get; set; } = new List<CategoryQuestion>();
} 