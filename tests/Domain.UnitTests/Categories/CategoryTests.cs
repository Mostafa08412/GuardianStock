using Domain.UnitTests.Builders;
using GuardianStock.Domain.Categories;
using Errors = GuardianStock.Domain.Core.Errors.Errors;
namespace Domain.UnitTests.Categories;

/// <summary>
/// Unit tests for Category aggregate.
/// </summary>
public class CategoryTests
{
    #region Create Factory Method Tests

    [Fact]
    public void Create_WithValidInputs_ShouldReturnSuccessResult()
    {
        // Arrange
        var name = "Electronics";
        var description = "Electronic devices and accessories";

        // Act
        var result = Category.Create(name, description);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(name);
        result.Value.Description.Should().Be(description);
    }

    [Fact]
    public void Create_WithValidInputsUsingBuilder_ShouldReturnSuccessResult()
    {
        // Arrange & Act
        var result = CategoryBuilder.Create()
            .WithName("Office Supplies")
            .WithDescription("Pens, papers, and office equipment")
            .Build();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Office Supplies");
        result.Value.Description.Should().Be("Pens, papers, and office equipment");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldReturnFailureResult(string invalidName)
    {
        // Arrange & Act
        var result = CategoryBuilder.Create()
            .WithName(invalidName)
            .Build();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.CategoryErrors.NameIsRequired);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidDescription_ShouldReturnFailureResult(string invalidDescription)
    {
        // Arrange & Act
        var result = CategoryBuilder.Create()
            .WithDescription(invalidDescription)
            .Build();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.CategoryErrors.DescriptionIsRequired);
    }

    #endregion

    #region Rename Method Tests

    [Fact]
    public void Rename_WithValidName_ShouldUpdateNameSuccessfully()
    {
        // Arrange
        var category = CategoryBuilder.Create().BuildSuccessfully();
        var newName = "Updated Category Name";

        // Act
        var result = category.Rename(newName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        category.Name.Should().Be(newName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Rename_WithInvalidName_ShouldReturnFailureResult(string invalidName)
    {
        // Arrange
        var category = CategoryBuilder.Create().BuildSuccessfully();
        var originalName = category.Name;

        // Act
        var result = category.Rename(invalidName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.CategoryErrors.NameIsRequired);
        category.Name.Should().Be(originalName);
    }

    #endregion

    #region Redescription Method Tests

    [Fact]
    public void Redescription_WithValidDescription_ShouldUpdateDescriptionSuccessfully()
    {
        // Arrange
        var category = CategoryBuilder.Create().BuildSuccessfully();
        var newDescription = "Updated description with new details";

        // Act
        var result = category.Redescription(newDescription);

        // Assert
        result.IsSuccess.Should().BeTrue();
        category.Description.Should().Be(newDescription);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Redescription_WithInvalidDescription_ShouldReturnFailureResult(string invalidDescription)
    {
        // Arrange
        var category = CategoryBuilder.Create().BuildSuccessfully();
        var originalDescription = category.Description;

        // Act
        var result = category.Redescription(invalidDescription);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.CategoryErrors.DescriptionIsRequired);
        category.Description.Should().Be(originalDescription);
    }

    #endregion
}
