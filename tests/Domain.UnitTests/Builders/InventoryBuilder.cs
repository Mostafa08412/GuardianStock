using IMS.Domain.Inventories;

namespace Domain.UnitTests.Builders;

/// <summary>
/// Test Data Builder for Inventory aggregate.
/// Implements the Builder Pattern for creating test instances with sensible defaults.
/// Usage:
/// - var inventory = InventoryBuilder.Create().Build();
/// - var inventory = InventoryBuilder.Create().WithQuantity(50).Build();
/// </summary>
public class InventoryBuilder
{
    // Default valid values that can be overridden
    private int _quantity = 12;
    private int _lowStockThreshold = 10;
    private Guid _productId = Guid.NewGuid();



    /// <summary>
    /// Creates a new InventoryBuilder with default valid values.
    /// </summary>
    public static InventoryBuilder Create() => new();

    /// <summary>
    /// Creates an InventoryBuilder with all required values set to invalid.
    /// Useful for testing validation failures.
    /// </summary>
    public static InventoryBuilder CreateInvalid() => new InventoryBuilder
    {
        _quantity = 0,
        _lowStockThreshold = 0,
        _productId = Guid.Empty
    };

        public InventoryBuilder WithQuantity(int quantity)
        {
            _quantity = quantity;
            return this;
        }
        public InventoryBuilder WithLowStockThreshold(int lowStockThreshold)
        {
            _lowStockThreshold = lowStockThreshold;
            return this;
        }
        public InventoryBuilder WithProductId(Guid productId)
        {
            _productId = productId;
            return this;
        }

    /// <summary>
    /// Builds the Inventory using the configured values.
    /// Returns Result&lt;Inventory&gt; to match the actual factory method signature.
    /// </summary>
    public Result<Inventory> Build()
    {
        return Inventory.Create(_quantity, _lowStockThreshold, _productId);
    }

    /// <summary>
    /// Builds and returns the Inventory directly, throwing if creation fails.
    /// Use only when you know the values are valid.
    /// </summary>
    public Inventory BuildSuccessfully()
    {
        var result = Build();
        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to build inventory: {result.Error.Description}");
        }
        return result.Value;
    }
}
