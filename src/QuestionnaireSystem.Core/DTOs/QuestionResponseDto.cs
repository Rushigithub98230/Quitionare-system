using System.ComponentModel.DataAnnotations;

namespace QuestionnaireSystem.Core.DTOs;

public class QuestionResponseDto
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid QuestionId { get; set; }
    
    public string? TextResponse { get; set; }
    
    public decimal? NumberResponse { get; set; }
    
    public DateTime? DateResponse { get; set; }
    
    public List<Guid> SelectedOptionIds { get; set; } = new List<Guid>();
    
    public string? FileUrl { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public DateTime? CreatedDate { get; set; }
    
    public DateTime? UpdatedDate { get; set; }
    
    public DateTime? DeletedDate { get; set; }
    
    // Navigation properties
    public CategoryQuestionDto? Question { get; set; }
    public UserQuestionResponseDto? UserQuestionResponse { get; set; }
}

public class CreateQuestionResponseDto
{
    [Required]
    public Guid QuestionId { get; set; }
    
    public string? TextResponse { get; set; }
    
    public decimal? NumberResponse { get; set; }
    
    public DateTime? DateResponse { get; set; }
    
    public List<Guid> SelectedOptionIds { get; set; } = new List<Guid>();
    
    public string? FileUrl { get; set; }
    
    public string? ImageUrl { get; set; }
}

public class UpdateQuestionResponseDto
{
    public string? TextResponse { get; set; }
    
    public decimal? NumberResponse { get; set; }
    
    public DateTime? DateResponse { get; set; }
    
    public List<Guid> SelectedOptionIds { get; set; } = new List<Guid>();
    
    public string? FileUrl { get; set; }
    
    public string? ImageUrl { get; set; }
} 