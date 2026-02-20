using Domain.UnitTests.Builders;
using GuardianStock.Domain.Transactions;
using GuardianStock.Domain.Transactions;
using Errors = GuardianStock.Domain.Core.Errors.Errors;

namespace Domain.UnitTests.Transactions;

/// <summary>
/// Unit tests for Transaction aggregate.
/// Transactions are immutable logs - no update methods, only creation.
/// </summary>
public class TransactionTests
{
    #region RecordSale Factory Method Tests

    [Fact]
    public void RecordSale_WithValidInputs_ShouldReturnSuccessResult()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var quantity = 10;
        var unitPrice = 49.99m;

        // Act
        var result = Transaction.RecordSale(productId, quantity, unitPrice);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.ProductId.Should().Be(productId);
        result.Value.Quantity.Should().Be(quantity);
        result.Value.UnitPrice.Should().Be(unitPrice);
        result.Value.Type.Should().Be(TransactionType.Sale);
        result.Value.TotalAmount.Should().Be(quantity * unitPrice);
    }

    [Fact]
    public void RecordSale_WithValidInputsUsingBuilder_ShouldReturnSuccessResult()
    {
        // Arrange & Act
        var result = TransactionBuilder.CreateSale()
            .WithQuantity(5)
            .WithUnitPrice(99.99m)
            .BuildSale();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Type.Should().Be(TransactionType.Sale);
        result.Value.Quantity.Should().Be(5);
        result.Value.UnitPrice.Should().Be(99.99m);
        result.Value.TotalAmount.Should().Be(499.95m);
    }

    [Fact]
    public void RecordSale_WithEmptyProductId_ShouldReturnFailureResult()
    {
        // Arrange & Act
        var result = TransactionBuilder.CreateSale()
            .WithProductId(Guid.Empty)
            .BuildSale();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.TransactionErrors.InvalidProductId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void RecordSale_WithInvalidQuantity_ShouldReturnFailureResult(int invalidQuantity)
    {
        // Arrange & Act
        var result = TransactionBuilder.CreateSale()
            .WithQuantity(invalidQuantity)
            .BuildSale();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.TransactionErrors.InvalidQuantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-99.99)]
    public void RecordSale_WithInvalidUnitPrice_ShouldReturnFailureResult(decimal invalidPrice)
    {
        // Arrange & Act
        var result = TransactionBuilder.CreateSale()
            .WithUnitPrice(invalidPrice)
            .BuildSale();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.TransactionErrors.InvalidUnitPrice);
    }

    [Fact]
    public void RecordSale_ShouldRaiseDomainEvent()
    {
        // Arrange
        var productId = Guid.NewGuid();

        // Act
        var result = Transaction.RecordSale(productId, 10, 50.00m);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DomainEvents.Should().ContainSingle();
        result.Value.DomainEvents.Should().OnlyContain(e => e is TransactionCreatedDomainEvent);

        var domainEvent = result.Value.DomainEvents.First() as TransactionCreatedDomainEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.ProductId.Should().Be(productId);
        domainEvent.Quantity.Should().Be(10);
        domainEvent.UnitPrice.Should().Be(50.00m);
        domainEvent.Type.Should().Be(TransactionType.Sale);
        (domainEvent.Quantity * domainEvent.UnitPrice).Should().Be(500.00m);
    }

    #endregion

    #region RecordPurchase Factory Method Tests

    [Fact]
    public void RecordPurchase_WithValidInputs_ShouldReturnSuccessResult()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var quantity = 100;
        var unitPrice = 25.50m;

        // Act
        var result = Transaction.RecordPurchase(productId, quantity, unitPrice);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.ProductId.Should().Be(productId);
        result.Value.Quantity.Should().Be(quantity);
        result.Value.UnitPrice.Should().Be(unitPrice);
        result.Value.Type.Should().Be(TransactionType.Purchase);
        result.Value.TotalAmount.Should().Be(quantity * unitPrice);
    }

    [Fact]
    public void RecordPurchase_WithValidInputsUsingBuilder_ShouldReturnSuccessResult()
    {
        // Arrange & Act
        var result = TransactionBuilder.CreatePurchase()
            .WithQuantity(50)
            .WithUnitPrice(10.00m)
            .BuildPurchase();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Type.Should().Be(TransactionType.Purchase);
        result.Value.Quantity.Should().Be(50);
        result.Value.TotalAmount.Should().Be(500.00m);
    }

    [Fact]
    public void RecordPurchase_WithEmptyProductId_ShouldReturnFailureResult()
    {
        // Arrange & Act
        var result = TransactionBuilder.CreatePurchase()
            .WithProductId(Guid.Empty)
            .BuildPurchase();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.TransactionErrors.InvalidProductId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50)]
    public void RecordPurchase_WithInvalidQuantity_ShouldReturnFailureResult(int invalidQuantity)
    {
        // Arrange & Act
        var result = TransactionBuilder.CreatePurchase()
            .WithQuantity(invalidQuantity)
            .BuildPurchase();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.TransactionErrors.InvalidQuantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50.50)]
    public void RecordPurchase_WithInvalidUnitPrice_ShouldReturnFailureResult(decimal invalidPrice)
    {
        // Arrange & Act
        var result = TransactionBuilder.CreatePurchase()
            .WithUnitPrice(invalidPrice)
            .BuildPurchase();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.TransactionErrors.InvalidUnitPrice);
    }

    [Fact]
    public void RecordPurchase_ShouldRaiseDomainEvent()
    {
        // Arrange
        var productId = Guid.NewGuid();

        // Act
        var result = Transaction.RecordPurchase(productId, 20, 15.00m);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DomainEvents.Should().ContainSingle();
        result.Value.DomainEvents.Should().OnlyContain(e => e is TransactionCreatedDomainEvent);

        var domainEvent = result.Value.DomainEvents.First() as TransactionCreatedDomainEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.ProductId.Should().Be(productId);
        domainEvent.Quantity.Should().Be(20);
        domainEvent.UnitPrice.Should().Be(15.00m);
        domainEvent.Type.Should().Be(TransactionType.Purchase);
        (domainEvent.Quantity * domainEvent.UnitPrice).Should().Be(300.00m);
    }

    #endregion

    #region TotalAmount Computed Property Tests

    [Theory]
    [InlineData(1, 10.00, 10.00)]
    [InlineData(5, 10.00, 50.00)]
    [InlineData(10, 9.99, 99.90)]
    [InlineData(100, 1.50, 150.00)]
    public void TotalAmount_ShouldBeCalculatedCorrectly(int quantity, decimal unitPrice, decimal expectedTotal)
    {
        // Arrange & Act
        var transaction = TransactionBuilder.CreateSale()
            .WithQuantity(quantity)
            .WithUnitPrice(unitPrice)
            .BuildSuccessfully();

        // Assert
        transaction.TotalAmount.Should().Be(expectedTotal);
    }

    #endregion

    #region Immutability Tests

    [Fact]
    public void Transaction_AfterCreation_ShouldBeImmutable()
    {
        // Arrange
        var originalProductId = Guid.NewGuid();
        var originalQuantity = 10;
        var originalPrice = 50.00m;

        // Act
        var transaction = Transaction.RecordSale(originalProductId, originalQuantity, originalPrice).Value;

        // Assert - All properties should remain unchanged
        transaction.ProductId.Should().Be(originalProductId);
        transaction.Quantity.Should().Be(originalQuantity);
        transaction.UnitPrice.Should().Be(originalPrice);
        transaction.Type.Should().Be(TransactionType.Sale);

        // Verify no update methods exist (compile-time check - this test documents the design decision)
        var transactionType = typeof(Transaction);
        var updateMethods = transactionType.GetMethods()
            .Where(m => m.Name.StartsWith("Update") || m.Name.StartsWith("Change") || m.Name.StartsWith("Modify"))
            .ToList();

        updateMethods.Should().BeEmpty("Transactions should be immutable audit logs");
    }

    #endregion


}
