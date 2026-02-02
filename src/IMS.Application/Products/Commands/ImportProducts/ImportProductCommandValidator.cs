using FluentValidation;

namespace IMS.Application.Products.Commands.ImportProducts
{
    public class ImportProductCommandValidator : AbstractValidator<ImportProductsCommand>
    {
        public ImportProductCommandValidator()
        {
            RuleFor(X => X.file).NotNull().WithMessage("CSV file is required");
        }
    }
}
