using FluentValidation;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Interfaces;

namespace QuestionnaireSystem.API.Validators;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters")
            .MustAsync(async (name, cancellation) =>
            {
                var exists = await categoryRepository.NameExistsAsync(name);
                return !exists;
            }).WithMessage("Category name already exists");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Color)
            .MaximumLength(50).WithMessage("Color cannot exceed 50 characters");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be non-negative");
    }
} 