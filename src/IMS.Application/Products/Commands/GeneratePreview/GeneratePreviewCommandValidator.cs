using FluentValidation;
using IMS.Application.Common.Errors;

namespace IMS.Application.Products.Commands.GeneratePreview
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
