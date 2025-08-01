using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.DTOs;

public class CreateCategoryDto
{
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
    public string? Features { get; set; }
    
    [MaxLength(500)]
    public string? ConsultationDescription { get; set; }
    
    public decimal BasePrice { get; set; }
    
    public bool RequiresQuestionnaireAssessment { get; set; } = true;
    
    public bool AllowsMedicationDelivery { get; set; } = true;
    
    public bool AllowsFollowUpMessaging { get; set; } = true;
    
    public int OneTimeConsultationDurationMinutes { get; set; } = 60;
    
    public bool IsMostPopular { get; set; } = false;
    
    public bool IsTrending { get; set; } = false;
}

public class UpdateCategoryDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(100)]
    public string? Icon { get; set; }
    
    [MaxLength(100)]
    public string? Color { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public int DisplayOrder { get; set; }
    
    [MaxLength(1000)]
    public string? Features { get; set; }
    
    [MaxLength(500)]
    public string? ConsultationDescription { get; set; }
    
    public decimal BasePrice { get; set; }
    
    public bool RequiresQuestionnaireAssessment { get; set; } = true;
    
    public bool AllowsMedicationDelivery { get; set; } = true;
    
    public bool AllowsFollowUpMessaging { get; set; } = true;
    
    public int OneTimeConsultationDurationMinutes { get; set; } = 60;
    
    public bool IsMostPopular { get; set; } = false;
    
    public bool IsTrending { get; set; } = false;
}

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public string? Features { get; set; }
    public string? ConsultationDescription { get; set; }
    public decimal BasePrice { get; set; }
    public bool RequiresQuestionnaireAssessment { get; set; }
    public bool AllowsMedicationDelivery { get; set; }
    public bool AllowsFollowUpMessaging { get; set; }
    public int OneTimeConsultationDurationMinutes { get; set; }
    public bool IsMostPopular { get; set; }
    public bool IsTrending { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool HasQuestionnaireTemplate { get; set; }
} 