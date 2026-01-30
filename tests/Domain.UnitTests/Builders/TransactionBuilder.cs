using IMS.Domain.Transactions;

namespace Domain.UnitTests.Builders;

/// <summary>
/// Test Data Builder for Transaction aggregate.
/// Creates test instances with sensible defaults.
/// </summary>
public class TransactionBuilder
{
    private Guid _productId = Guid.NewGuid();
    private int _quantity = 5;
    private decimal _unitPrice = 99.99m;
    private TransactionType _type = TransactionType.Sale;

    private TransactionBuilder()
    {
    }

    /// <summary>
    /// Creates a new TransactionBuilder with default valid values for a Sale.
    /// </summary>
    public static TransactionBuilder CreateSale() => new();

    /// <summary>
    /// Creates a new TransactionBuilder with default valid values for a Purchase.
    /// </summary>
    public static TransactionBuilder CreatePurchase() => new TransactionBuilder
    {
        _type = TransactionType.Purchase
    };

    /// <summary>
    /// Creates a TransactionBuilder with invalid values.
    /// </summary>
    public static TransactionBuilder CreateInvalid() => new TransactionBuilder
    {
        _productId = Guid.Empty,
        _quantity = 0,
        _unitPrice = 0
    };

    public TransactionBuilder WithProductId(Guid productId)
    {
        _productId = productId;
        return this;
    }

    public TransactionBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public TransactionBuilder WithUnitPrice(decimal unitPrice)
    {
        _unitPrice = unitPrice;
        return this;
    }

    /// <summary>
    /// Builds a Transaction using RecordSale factory method.
    /// </summary>
    public Result<Transaction> BuildSale()
    {
        return Transaction.RecordSale(_productId, _quantity, _unitPrice);
    }

    /// <summary>
    /// Builds a Transaction using RecordPurchase factory method.
    /// </summary>
    public Result<Transaction> BuildPurchase()
    {
        return Transaction.RecordPurchase(_productId, _quantity, _unitPrice);
    }

    /// <summary>
    /// Builds based on the configured type.
    /// </summary>
    public Result<Transaction> Build()
    {
        return _type == TransactionType.Sale
            ? BuildSale()
            : BuildPurchase();
    }

    /// <summary>
    /// Builds and returns the Transaction directly, throwing if creation fails.
    /// </summary>
    public Transaction BuildSuccessfully()
    {
        var result = Build();
        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to build transaction: {result.Error.Description}");
        }
        return result.Value;
    }
}
