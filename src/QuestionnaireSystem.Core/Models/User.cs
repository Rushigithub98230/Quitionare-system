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
        
        public string? Category { get; set; } // e.g., "Hair Loss", "Weight Loss"
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public DateTime? LastLoginAt { get; set; }
        
        // Navigation properties
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<UserQuestionResponse> Responses { get; set; } = new List<UserQuestionResponse>();
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<CategoryQuestionnaireTemplate> CreatedQuestionnaires { get; set; } = new List<CategoryQuestionnaireTemplate>();
    }
} 