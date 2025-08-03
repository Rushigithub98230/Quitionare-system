using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.DTOs;

public class UserQuestionResponseDto
{
    public Guid Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public Guid QuestionnaireId { get; set; }
    
    public DateTime? CreatedDate { get; set; }
    
    public DateTime? UpdatedDate { get; set; }
    
    public DateTime? DeletedDate { get; set; }
    
    // Navigation properties
    public UserDto? User { get; set; }
    public CategoryQuestionnaireTemplateDto? Questionnaire { get; set; }
    public List<QuestionResponseDto> QuestionResponses { get; set; } = new List<QuestionResponseDto>();
}

public class CreateUserQuestionResponseDto
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public Guid QuestionnaireId { get; set; }
    
    public List<CreateQuestionResponseDto> QuestionResponses { get; set; } = new List<CreateQuestionResponseDto>();
}

public class UpdateUserQuestionResponseDto
{
    public List<CreateQuestionResponseDto> QuestionResponses { get; set; } = new List<CreateQuestionResponseDto>();
} 