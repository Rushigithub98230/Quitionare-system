namespace QuestionnaireSystem.Core.DTOs;

public class QuestionTypeDto
{
    public Guid Id { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool HasOptions { get; set; }
    public bool SupportsFileUpload { get; set; }
    public bool SupportsImage { get; set; }
    public string? ValidationRules { get; set; }
    public bool IsActive { get; set; }
} 