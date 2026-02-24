using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Domain.Core.Errors;
using MediatR;

namespace GuardianStock.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            return Result.Failure(Errors.CategoryErrors.NotFound);
        }

        if (request.Name is not null && request.Name != category.Name)
        {
            if (!await _unitOfWork.Categories.IsNameUniqueAsync(request.Name, cancellationToken))
            {
                return Result.Failure(Errors.CategoryErrors.NameAlreadyExists);
            }

            var result = category.Rename(request.Name);
            if (result.IsFailure) return result;
        }

        if (request.Description is not null)
        {
            var result = category.Redescription(request.Description);
            if (result.IsFailure) return result;
        }

        return Result.Success();
    }
}
