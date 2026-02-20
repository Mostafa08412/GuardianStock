using GuardianStock.Domain.Categories;
using GuardianStock.Domain.Core.Primitives.Result;

namespace Domain.UnitTests.Builders;

/// <summary>
/// Test Data Builder for Category aggregate.
/// Creates test instances with sensible defaults.
/// </summary>
public class CategoryBuilder
{
    private string _name = "Test Category";
    private string _description = "Test category description";

    private CategoryBuilder()
    {
    }

    /// <summary>
    /// Creates a new CategoryBuilder with default valid values.
    /// </summary>
    public static CategoryBuilder Create() => new();

    /// <summary>
    /// Creates a CategoryBuilder with invalid values.
    /// </summary>
    public static CategoryBuilder CreateInvalid() => new CategoryBuilder
    {
        _name = string.Empty,
        _description = string.Empty
    };

    public CategoryBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CategoryBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    /// <summary>
    /// Builds the Category using the configured values.
    /// </summary>
    public Result<Category> Build()
    {
        return Category.Create(_name, _description);
    }

    /// <summary>
    /// Builds and returns the Category directly, throwing if creation fails.
    /// </summary>
    public Category BuildSuccessfully()
    {
        var result = Build();
        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to build category: {result.Error.Description}");
        }
        return result.Value;
    }
}
