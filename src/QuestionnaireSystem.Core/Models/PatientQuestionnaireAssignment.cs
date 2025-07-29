using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.Models;

public class PatientQuestionnaireAssignment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid PatientId { get; set; }
    
    [Required]
    public Guid QuestionnaireId { get; set; }
    
    [Required]
    public Guid AssignedBy { get; set; }
    
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? DueDate { get; set; }
    
    [MaxLength(50)]
    public string Status { get; set; } = "pending"; // pending, in_progress, completed, expired
    
    public bool NotificationSent { get; set; } = false;
    
    public int ReminderCount { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual QuestionnaireTemplate Questionnaire { get; set; } = null!;
    public virtual ICollection<PatientResponse> PatientResponses { get; set; } = new List<PatientResponse>();
} 