using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.Models;

public class Category : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(100)]
    public string? Icon { get; set; }
    
    [MaxLength(100)]
    public string? Color { get; set; }
    
    public int DisplayOrder { get; set; }
    
    [MaxLength(1000)]
    public string? Features { get; set; } // separated by commas
    
    [MaxLength(500)]
    public string? ConsultationDescription { get; set; }
    
    public decimal BasePrice { get; set; }
    
    public bool RequiresQuestionnaireAssessment { get; set; } = true;
    
    public bool AllowsMedicationDelivery { get; set; } = true;
    
    public bool AllowsFollowUpMessaging { get; set; } = true;
    
    public int OneTimeConsultationDurationMinutes { get; set; } = 60;
    
    // Marketing and display properties
    public bool IsMostPopular { get; set; } = false;
    
    public bool IsTrending { get; set; } = false;
    
    // Navigation properties - One-to-One relationship
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual CategoryQuestionnaireTemplate? QuestionnaireTemplate { get; set; }
} 