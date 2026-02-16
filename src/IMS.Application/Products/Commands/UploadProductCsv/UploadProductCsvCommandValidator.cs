using FluentValidation;
using IMS.Application.Common.Errors;

namespace IMS.Application.Products.Commands.UploadProductCsv
{
    public class UploadProductCsvCommandValidator : AbstractValidator<UploadProductCsvCommand>
    {
        public UploadProductCsvCommandValidator()
        {
            RuleFor(X => X.File)
                .NotEmpty()
                .WithMessage(ApplicationErrors.CsvReader.FileIsRequired.Description)
                .WithErrorCode(ApplicationErrors.CsvReader.FileIsRequired.Code)
                .NotNull()
                .WithMessage(ApplicationErrors.CsvReader.FileIsRequired.Description)
                .WithErrorCode(ApplicationErrors.CsvReader.FileIsRequired.Code);
        }
    }
}
