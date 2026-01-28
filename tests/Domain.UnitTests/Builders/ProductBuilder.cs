using IMS.Domain.Products;

namespace Domain.UnitTests.Builders;

/// <summary>
/// Test Data Builder for Product aggregate.
/// Implements the Builder Pattern for creating test instances with sensible defaults.
/// Usage: 
/// - var product = ProductBuilder.Create().Build();
/// - var product = ProductBuilder.Create().WithName("Custom").Build();
/// </summary>
public class ProductBuilder
{
    // Default valid values that can be overridden
    private string _name = "Test Product";
    private string _sku = "SKU-001";
    private string _description = "Test product description";
    private decimal _price = 99.99m;
    private string _supplier = "Test Supplier";
    private Guid _categoryId = Guid.NewGuid();

    private ProductBuilder()
    {
    }

    /// <summary>
    /// Creates a new ProductBuilder with default valid values.
    /// </summary>
    public static ProductBuilder Create() => new();

    /// <summary>
    /// Creates a ProductBuilder with all required values set to invalid/empty.
    /// Useful for testing validation failures.
    /// </summary>
    public static ProductBuilder CreateInvalid() => new ProductBuilder
    {
        _name = string.Empty,
        _sku = string.Empty,
        _description = string.Empty,
        _price = 0,
        _supplier = string.Empty,
        _categoryId = Guid.Empty
    };

    // Fluent methods for customization
    public ProductBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProductBuilder WithSku(string sku)
    {
        _sku = sku;
        return this;
    }

    public ProductBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ProductBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public ProductBuilder WithSupplier(string supplier)
    {
        _supplier = supplier;
        return this;
    }

    public ProductBuilder WithCategoryId(Guid categoryId)
    {
        _categoryId = categoryId;
        return this;
    }

    /// <summary>
    /// Builds the Product using the configured values.
    /// Returns Result<Product> to match the actual factory method signature.
    /// </summary>
    public Result<Product> Build()
    {
        return Product.Create(_name, _sku, _description, _price, _supplier, _categoryId);
    }

    /// <summary>
    /// Builds and returns the Product directly, throwing if creation fails.
    /// Use only when you know the values are valid.
    /// </summary>
    public Product BuildSuccessfully()
    {
        var result = Build();
        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to build product: {result.Errors.First().Description}");
        }
        return result.Value;
    }
}
