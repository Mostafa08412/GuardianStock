using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Application.Products.Queries.ListProducts;
using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Domain.Inventories;
using GuardianStock.Domain.Products;
using GuardianStock.Domain.Core.Errors;
using MediatR;

namespace GuardianStock.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductListItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISkuGenerator _skuGenerator;
    private readonly IFileManagerService _fileManagerService;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, ISkuGenerator skuGenerator, IFileManagerService fileManagerService)
    {
        _unitOfWork = unitOfWork;
        _skuGenerator = skuGenerator;
        _fileManagerService = fileManagerService;
    }

    public async Task<Result<ProductListItemDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {

        var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
            return Result<ProductListItemDto>.Failure(Errors.CategoryErrors.NotFound);


        var generatedSku = _skuGenerator.GenerateSKU(request.Supplier);

        string? imageUrl = null;
        if (request.Image is not null)
        {
            var fileName = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(request.Image.FileName).TrimStart('.');
            await _fileManagerService.UploadFileAsync(request.Image, "products-images", fileName, cancellationToken);
            imageUrl = $"api/v2/products/images/{fileName}.{extension}";
        }

        // 2. Create Product
        var productResult = Product.Create(
            request.Name,
            generatedSku,
            request.Description,
            request.Price,
            request.Supplier,
            request.CategoryId,
            imageUrl);

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
                product.Description,
                product.ImageUrl
                );

        return Result<ProductListItemDto>.Success(productDTO);
    }
}
