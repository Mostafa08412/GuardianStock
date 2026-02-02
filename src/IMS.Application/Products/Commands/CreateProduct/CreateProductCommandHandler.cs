using IMS.Domain.Abstractions;
using IMS.Domain.Core.Errors;
using IMS.Domain.Core.Primitives.Result;
using IMS.Domain.Inventories;
using IMS.Domain.Products;
using MediatR;

namespace IMS.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISkuGenerator _skuGenerator;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, ISkuGenerator skuGenerator)
    {
        _unitOfWork = unitOfWork;
        _skuGenerator = skuGenerator;
    }

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {

        var categoryExists = await _unitOfWork.Categories.ExistsAsync(request.CategoryId, cancellationToken);

        if (!categoryExists)
            return Result<Guid>.Failure(Errors.CategoryErrors.NotFound);


        var generatedSku = _skuGenerator.GenerateSKU(request.Supplier);

        // 2. Create Product
        var productResult = Product.Create(
            request.Name,
            generatedSku,
            request.Description,
            request.Price,
            request.Supplier,
            request.CategoryId);

        if (productResult.IsFailure)
        {
            return Result<Guid>.Failure(productResult.Error!);
        }

        var product = productResult.Value!;

        // 3. Create Initial Inventory
        var inventoryResult = Inventory.Create(
            request.InitialQuantity,
            request.LowStockThreshold,
            product.Id);

        if (inventoryResult.IsFailure)
        {
            return Result<Guid>.Failure(inventoryResult.Error!);
        }

        var inventory = inventoryResult.Value!;

        _unitOfWork.Products.Add(product);
        _unitOfWork.Inventories.Add(inventory);



        return Result<Guid>.Success(product.Id);

    }
}
