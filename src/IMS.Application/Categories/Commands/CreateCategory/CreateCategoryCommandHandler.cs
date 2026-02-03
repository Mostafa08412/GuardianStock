using IMS.Domain.Abstractions;
using IMS.Domain.Categories;
using IMS.Domain.Core.Errors;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (!await _unitOfWork.Categories.IsNameUniqueAsync(request.Name, cancellationToken))
        {
            return Result<Guid>.Failure(Errors.CategoryErrors.NameAlreadyExists);
        }

        var categoryResult = Category.Create(request.Name, request.Description);

        if (categoryResult.IsFailure)
        {
            return Result<Guid>.Failure(categoryResult.Error);
        }

        var category = categoryResult.Value!;

        _unitOfWork.Categories.Add(category);

        return Result<Guid>.Success(category.Id);
    }
}
