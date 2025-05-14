using AI.Agent.Domain.Entities;
using FluentValidation;

namespace AI.Agent.API.Validators;

public class DocumentValidator : AbstractValidator<Document>
{
    public DocumentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255)
            .WithMessage("Name is required and must not exceed 255 characters");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required");

        RuleFor(x => x.FileType)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("File type is required and must not exceed 50 characters");

        RuleFor(x => x.CreatedAt)
            .NotEmpty()
            .WithMessage("Created date is required");

        RuleFor(x => x.IsProcessed)
            .NotNull()
            .WithMessage("Processed status is required");
    }
} 