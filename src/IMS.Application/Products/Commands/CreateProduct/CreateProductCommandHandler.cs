using IMS.Application.Products.Queries.ListProducts;
using IMS.Domain.Abstractions;
using IMS.Domain.Core.Errors;
using IMS.Domain.Core.Primitives.Result;
using IMS.Domain.Inventories;
using IMS.Domain.Products;
using MediatR;

namespace IMS.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductListItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISkuGenerator _skuGenerator;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, ISkuGenerator skuGenerator)
    {
        _unitOfWork = unitOfWork;
        _skuGenerator = skuGenerator;
    }

    public async Task<Result<ProductListItemDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {

        var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
            return Result<ProductListItemDto>.Failure(Errors.CategoryErrors.NotFound);


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
            return Result<ProductListItemDto>.Failure(productResult.Error!);
        }

        var product = productResult.Value!;

        // 3. Create Initial Inventory
        var inventoryResult = Inventory.Create(
            request.InitialQuantity,
            request.LowStockThreshold,
            product.Id);

        if (inventoryResult.IsFailure)
        {
            return Result<ProductListItemDto>.Failure(inventoryResult.Error!);
        }

        var inventory = inventoryResult.Value!;

        _unitOfWork.Products.Add(product);
        _unitOfWork.Inventories.Add(inventory);
        var productDTO =
            new ProductListItemDto(
                product.Id,
                product.Name,
                product.Sku,
                product.Price,
                product.Supplier,
                category.Name,
                product.CategoryId,
                inventory.Quantity,
                inventory.LowStockThreshold,
                product.Description
                );

        return Result<ProductListItemDto>.Success(productDTO);
    }
}
