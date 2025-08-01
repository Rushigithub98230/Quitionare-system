using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.Models;

public class UserQuestionResponse
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Guid QuestionnaireId { get; set; }
    
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? CompletedAt { get; set; }
    
    public bool IsCompleted { get; set; } = false;
    
    public bool IsDraft { get; set; } = true;
    
    [MaxLength(45)]
    public string? SubmissionIp { get; set; }
    
    public string? UserAgent { get; set; }
    
    public int? TimeTaken { get; set; } // in seconds
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual User User { get; set; } = null!;
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual CategoryQuestionnaireTemplate Questionnaire { get; set; } = null!;
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<QuestionResponse> QuestionResponses { get; set; } = new List<QuestionResponse>();
} 