using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.Models;

public class CategoryQuestionnaireTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
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
    public Guid? CreatedBy { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? DeletedAt { get; set; }
    
    // Navigation properties
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual Category Category { get; set; } = null!;
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual User? CreatedByUser { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<CategoryQuestion> Questions { get; set; } = new List<CategoryQuestion>();
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<UserQuestionResponse> UserResponses { get; set; } = new List<UserQuestionResponse>();
} 