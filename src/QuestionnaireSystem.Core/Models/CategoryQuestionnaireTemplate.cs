using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.Models;

public class CategoryQuestionnaireTemplate : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public Guid CategoryId { get; set; }
    
    public bool IsMandatory { get; set; } = false;
    
    public int DisplayOrder { get; set; } = 0;
    
    public int Version { get; set; } = 1;
    
    // Navigation properties
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual Category Category { get; set; } = null!;
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<CategoryQuestion> Questions { get; set; } = new List<CategoryQuestion>();
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<UserQuestionResponse> UserResponses { get; set; } = new List<UserQuestionResponse>();
} 