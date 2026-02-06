using FluentValidation;

namespace IMS.Application.Products.Commands.GeneratePreview
{
    public class GeneratePreviewCommandValidator : AbstractValidator<GeneratePreviewCommand>
    {
        public GeneratePreviewCommandValidator()
        {
            RuleFor(X => X.filePath).NotNull().WithMessage("File path is required");
        }
    }
}
