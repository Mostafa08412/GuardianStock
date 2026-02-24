using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Categories;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Domain.Core.Errors;
using MediatR;

namespace GuardianStock.Application.Categories.Commands.CreateCategory;

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
