using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.Models;

public class QuestionResponse
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ResponseId { get; set; }
    
    [Required]
    public Guid QuestionId { get; set; }
    
    public string? TextResponse { get; set; }
    
    public decimal? NumberResponse { get; set; }
    
    public DateTime? DateResponse { get; set; }
    
    public DateTime? DatetimeResponse { get; set; }
    
    public bool? BooleanResponse { get; set; }
    
    public string? JsonResponse { get; set; } // JSON string
    
    [MaxLength(500)]
    public string? FilePath { get; set; }
    
    [MaxLength(255)]
    public string? FileName { get; set; }
    
    public int? FileSize { get; set; }
    
    [MaxLength(100)]
    public string? FileType { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual UserQuestionResponse Response { get; set; } = null!;
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual CategoryQuestion Question { get; set; } = null!;
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<QuestionOptionResponse> OptionResponses { get; set; } = new List<QuestionOptionResponse>();
} 