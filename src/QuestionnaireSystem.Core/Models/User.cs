using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.Models
{
    public class User
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        public string Role { get; set; } = "User"; // User, Admin
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public DateTime? LastLoginAt { get; set; }
        
        // Navigation properties
        public virtual ICollection<PatientQuestionnaireAssignment> Assignments { get; set; } = new List<PatientQuestionnaireAssignment>();
        public virtual ICollection<PatientResponse> Responses { get; set; } = new List<PatientResponse>();
    }
} 