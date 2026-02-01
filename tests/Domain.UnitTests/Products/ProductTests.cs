using Domain.UnitTests.Builders;
using IMS.Domain.Products;
using Errors = IMS.Domain.Core.Errors.Errors;

namespace Domain.UnitTests.Products;

/// <summary>
/// Unit tests for Product aggregate.
/// Tests are organized using Arrange-Act-Assert (AAA) pattern.
/// Test naming convention: MethodName_StateUnderTest_ExpectedBehavior
/// </summary>
public class ProductTests
{
    #region Create Factory Method Tests

    [Fact]
    public void Create_WithValidInputs_ShouldReturnSuccessResult()
    {
        // Arrange
        var name = "Laptop";
        var sku = "LAP-001";
        var description = "Dell XPS 15";
        var price = 1299.99m;
        var supplier = "Dell Inc.";
        var categoryId = Guid.NewGuid();

        // Act
        var result = Product.Create(name, sku, description, price, supplier, categoryId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(name);
        result.Value.Sku.Should().Be(sku);
        result.Value.Description.Should().Be(description);
        result.Value.Price.Should().Be(price);
        result.Value.Supplier.Should().Be(supplier);
        result.Value.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public void Create_WithValidInputsUsingBuilder_ShouldReturnSuccessResult()
    {
        // Arrange & Act
        var result = ProductBuilder.Create()
            .WithName("Gaming Mouse")
            .WithPrice(79.99m)
            .Build();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Gaming Mouse");
        result.Value.Price.Should().Be(79.99m);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldReturnFailureResult(string invalidName)
    {
        // Arrange & Act
        var result = ProductBuilder.Create()
            .WithName(invalidName)
            .Build();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.ProductErrors.NameIsRequired);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidSku_ShouldReturnFailureResult(string invalidSku)
    {
        // Arrange & Act
        var result = ProductBuilder.Create()
            .WithSku(invalidSku)
            .Build();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.ProductErrors.SkuIsRequired);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidDescription_ShouldReturnFailureResult(string invalidDescription)
    {
        // Arrange & Act
        var result = ProductBuilder.Create()
            .WithDescription(invalidDescription)
            .Build();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.ProductErrors.DescriptionIsRequired);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-99.99)]
    public void Create_WithInvalidPrice_ShouldReturnFailureResult(decimal invalidPrice)
    {
        // Arrange & Act
        var result = ProductBuilder.Create()
            .WithPrice(invalidPrice)
            .Build();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.ProductErrors.InvalidPrice);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidSupplier_ShouldReturnFailureResult(string invalidSupplier)
    {
        // Arrange & Act
        var result = ProductBuilder.Create()
            .WithSupplier(invalidSupplier)
            .Build();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.ProductErrors.SupplierIsRequired);
    }

    [Fact]
    public void Create_WithEmptyCategoryId_ShouldReturnFailureResult()
    {
        // Arrange & Act
        var result = ProductBuilder.Create()
            .WithCategoryId(Guid.Empty)
            .Build();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.ProductErrors.CategoryIsRequired);
    }

    #endregion

    #region Rename Method Tests

    [Fact]
    public void Rename_WithValidName_ShouldUpdateNameSuccessfully()
    {
        // Arrange
        var product = ProductBuilder.Create().BuildSuccessfully();
        var newName = "Updated Product Name";

        // Act
        var result = product.Rename(newName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Name.Should().Be(newName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Rename_WithInvalidName_ShouldReturnFailureResult(string invalidName)
    {
        // Arrange
        var product = ProductBuilder.Create().BuildSuccessfully();
        var originalName = product.Name;

        // Act
        var result = product.Rename(invalidName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.ProductErrors.NameIsRequired);
        product.Name.Should().Be(originalName); // Name should not change
    }

    #endregion

    #region Redescription Method Tests

    [Fact]
    public void Redescription_WithValidDescription_ShouldUpdateDescriptionSuccessfully()
    {
        // Arrange
        var product = ProductBuilder.Create().BuildSuccessfully();
        var newDescription = "Updated product description with new details";

        // Act
        var result = product.Redescription(newDescription);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Description.Should().Be(newDescription);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Redescription_WithInvalidDescription_ShouldReturnFailureResult(string invalidDescription)
    {
        // Arrange
        var product = ProductBuilder.Create().BuildSuccessfully();
        var originalDescription = product.Description;

        // Act
        var result = product.Redescription(invalidDescription);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.ProductErrors.DescriptionIsRequired);
        product.Description.Should().Be(originalDescription);
    }

    #endregion

    #region UpdatePrice Method Tests

    [Theory]
    [InlineData(99.99)]
    [InlineData(0.01)] // Edge case: minimum valid price
    [InlineData(9999.99)]
    public void UpdatePrice_WithValidPrice_ShouldUpdatePriceSuccessfully(decimal newPrice)
    {
        // Arrange
        var product = ProductBuilder.Create().BuildSuccessfully();

        // Act
        var result = product.UpdatePrice(newPrice);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Price.Should().Be(newPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999.99)]
    public void UpdatePrice_WithInvalidPrice_ShouldReturnFailureResult(decimal invalidPrice)
    {
        // Arrange
        var product = ProductBuilder.Create()
            .WithPrice(50.00m)
            .BuildSuccessfully();
        var originalPrice = product.Price;

        // Act
        var result = product.UpdatePrice(invalidPrice);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.ProductErrors.InvalidPrice);
        product.Price.Should().Be(originalPrice);
    }

    #endregion

    #region UpdateSupplier Method Tests

    [Fact]
    public void UpdateSupplier_WithValidSupplier_ShouldUpdateSupplierSuccessfully()
    {
        // Arrange
        var product = ProductBuilder.Create().BuildSuccessfully();
        var newSupplier = "New Supplier Co.";

        // Act
        var result = product.UpdateSupplier(newSupplier);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Supplier.Should().Be(newSupplier);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateSupplier_WithInvalidSupplier_ShouldReturnFailureResult(string invalidSupplier)
    {
        // Arrange
        var product = ProductBuilder.Create().BuildSuccessfully();
        var originalSupplier = product.Supplier;

        // Act
        var result = product.UpdateSupplier(invalidSupplier);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.ProductErrors.SupplierIsRequired);
        product.Supplier.Should().Be(originalSupplier);
    }

    #endregion

    #region Recategorize Method Tests

    [Fact]
    public void Recategorize_WithValidCategoryId_ShouldUpdateCategorySuccessfully()
    {
        // Arrange
        var product = ProductBuilder.Create().BuildSuccessfully();
        var newCategoryId = Guid.NewGuid();

        // Act
        var result = product.Recategorize(newCategoryId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.CategoryId.Should().Be(newCategoryId);
    }

    [Fact]
    public void Recategorize_WithEmptyCategoryId_ShouldReturnFailureResult()
    {
        // Arrange
        var product = ProductBuilder.Create().BuildSuccessfully();
        var originalCategoryId = product.CategoryId;

        // Act
        var result = product.Recategorize(Guid.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.ProductErrors.CategoryIsRequired);
        product.CategoryId.Should().Be(originalCategoryId);
    }

    #endregion

    #region SKU Immutability Tests

    [Fact]
    public void Sku_AfterCreation_ShouldRemainImmutable()
    {
        // Arrange
        var originalSku = "IMMUTABLE-SKU";
        var product = ProductBuilder.Create()
            .WithSku(originalSku)
            .BuildSuccessfully();

        // Act
        product.Rename("New Name");
        product.UpdatePrice(199.99m);
        product.Recategorize(Guid.NewGuid());

        // Assert
        product.Sku.Should().Be(originalSku);
    }

    #endregion
}
