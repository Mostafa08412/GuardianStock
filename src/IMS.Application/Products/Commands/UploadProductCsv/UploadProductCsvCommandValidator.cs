using FluentValidation;

namespace IMS.Application.Products.Commands.UploadProductCsv
{
    public class UploadProductCsvCommandValidator : AbstractValidator<UploadProductCsvCommand>
    {
        public UploadProductCsvCommandValidator()
        {
            RuleFor(X => X.File).NotEmpty().NotNull();
        }
    }
}
