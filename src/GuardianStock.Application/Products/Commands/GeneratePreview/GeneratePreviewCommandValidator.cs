using FluentValidation;
using GuardianStock.Application.Common.Errors;

namespace GuardianStock.Application.Products.Commands.GeneratePreview
{
    public class GeneratePreviewCommandValidator : AbstractValidator<GeneratePreviewCommand>
    {
        public GeneratePreviewCommandValidator()
        {
            RuleFor(X => X.filePath)
                .NotNull()
                .WithMessage(ApplicationErrors.CsvReader.FileIsRequired.Description)
                .WithErrorCode(ApplicationErrors.CsvReader.FileIsRequired.Code);
        }
    }
}
