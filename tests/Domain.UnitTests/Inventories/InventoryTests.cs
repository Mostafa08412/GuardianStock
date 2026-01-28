using Domain.UnitTests.Builders;
using IMS.Domain.Inventories;
using static IMS.Domain.Inventories.Errors;

namespace Domain.UnitTests.Inventories
{
    public class InventoryTests
    {
        #region Create Factory Method Tests

        [Fact]
        public void Create_WithValidInputs_ShouldReturnSuccessResult()
        {

            //Arrange
            var quantity = 15;
            var lowStockThreshold = 10;
            var productId = Guid.NewGuid();

            //Act
            var result = Inventory.Create(quantity, lowStockThreshold, productId);

            //Assert

            result.IsSuccess.Should().BeTrue();
            result.Value!.ProductId.Should().Be(productId);
            result.Value!.LowStockThreshold.Should().Be(lowStockThreshold);
            result.Value!.ProductId.Should().Be(productId);
        }

        [Fact]
        public void Create_WithValidInputsUsingBuilder_ShouldReturnSuccessResult()
        {
            //Arrange,Act 
            var result = InventoryBuilder.Create()
                .WithQuantity(15)
                .WithLowStockThreshold(12)
                .Build();

            result.IsSuccess.Should().BeTrue();
            result.Value!.Quantity.Should().Be(15);
            result.Value!.LowStockThreshold.Should().Be(12);

        }

        [Theory]
        [InlineData(-10)]
        [InlineData(0)]
        [InlineData(-100)]
        public void Create_WithInvalidQuantity_ShouldReturnFailureResult(int quantity)
        {

            var result = InventoryBuilder.Create()
                .WithQuantity(quantity)
                .Build();

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(InventoryErrors.InvalidQuantity);

        }

        [Fact]
        public void Create_WithQuantityLowerThanThreshold_ShouldReturnFailureResult()
        {

            var result = InventoryBuilder.Create()
                .WithQuantity(20)
                .WithLowStockThreshold(25)
                .Build();

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(InventoryErrors.QuantityCannotBeLowerThanThreshold);

        }


        [Theory]
        [InlineData(-10)]
        [InlineData(1)]
        [InlineData(9)]
        public void Create_WithInvalidLowStockThreshold_ShouldReturnFailureResult(int lowStockThreshold)
        {

            var result = InventoryBuilder.Create()
                .WithLowStockThreshold(lowStockThreshold)
                .Build();

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(InventoryErrors.InvalidLowStockThreshold);

        }

        [Fact]
        public void Create_WithInvalidProductId_ShouldReturnFailureResult()
        {

            var result = InventoryBuilder.Create()
                .WithProductId(Guid.Empty)
                .Build();

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(InventoryErrors.ProductIsRequired);

        }


        #endregion

        #region Ship method tests
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]

        public void Ship_WithInvalidShippedQuantity_ShouldReturnResultFailure(int shippedQuantity)
        {
            var inventory = InventoryBuilder.Create().BuildSuccessfully();

            var result = inventory.Ship(shippedQuantity);

            result.IsSuccess.Should().BeFalse();

            result.Error.Should().Be(InventoryErrors.InvalidShipQuantity);

        }

        [Fact]
        public void Ship_WithShippedQuantityGreaterThanStock_ShouldReturnResultFailure()
        {
            var inventory = InventoryBuilder.Create().WithQuantity(20).BuildSuccessfully();

            var result = inventory.Ship(21);

            result.IsSuccess.Should().BeFalse();

            result.Error.Should().Be(InventoryErrors.InsufficientStock);

        }

        [Fact]
        public void Ship_WithValidShippedQuantity_ShouldReduceStock()
        {
            var inventory = InventoryBuilder.Create().WithQuantity(20).BuildSuccessfully();

            var result = inventory.Ship(5);

            result.IsSuccess.Should().BeTrue();

            inventory.Quantity.Should().Be(15);

        }


        [Fact]
        public void Ship_WithRemainingStockBelowThresholdAndLowStockAlertIsNull_ShouldTriggerLowStockAlert()
        {
            var inventory = InventoryBuilder.Create()
                .WithQuantity(20)
                .WithLowStockThreshold(15)
                .BuildSuccessfully();


            var result = inventory.Ship(6);


            inventory.LowStockAlert.Should().NotBe(null);

            inventory.LowStockAlert.Threshold.Should().Be(15);

            inventory.DomainEvents.Should().OnlyContain(X => X is LowStockAlertTriggeredDomainEvent);

            result.IsSuccess.Should().BeTrue();


        }

        #endregion

        #region Restock method tests
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]

        public void Restock_WithInvalidQuantity_ShouldReturnResultFailure(int newQuantity)
        {
            var inventory = InventoryBuilder.Create().BuildSuccessfully();


            var result = inventory.Restock(0);

            result.IsSuccess.Should().BeFalse();

        }

        [Fact]
        public void Restock_WithNewStockAboveThresholdAndExistingLowStockAlert_ShouldResetLowStockAlert()
        {
            var inventory = InventoryBuilder.Create()
                .WithQuantity(20)
                .WithLowStockThreshold(18)
                .BuildSuccessfully();

            inventory.Ship(3); // Low Stock Triggered.
            inventory.LowStockAlert.Should().NotBeNull();
            inventory.LowStockAlert.Threshold.Should().Be(18);

            var result = inventory.Restock(3); // Low Stock is reset.
            inventory.Quantity.Should().Be(20);

            inventory.LowStockAlert.Should().BeNull();
            result.IsSuccess.Should().BeTrue();

        }
        #endregion

        #region AdjustLowStockThreshold method tests

        [Theory]
        [InlineData(9)]
        [InlineData(1)]
        [InlineData(-20)]
        public void AdjustLowStockThreshold_WithInvalidThreshold_ShouldReturnResultFailure(int newThreshold)
        {
            var inventory = InventoryBuilder.Create().BuildSuccessfully();


            var result = inventory.AdjustLowStockThreshold(newThreshold);

            result.IsSuccess.Should().BeFalse();

            result.Error.Should().Be(InventoryErrors.InvalidLowStockThreshold);


        }

        [Fact]
        public void AdjustLowStockThreshold_WithActiveAlert_ShouldReturnResultFailure()
        {
            var inventory = InventoryBuilder.Create()
                .WithQuantity(21)
                .WithLowStockThreshold(20)
                .BuildSuccessfully();

            inventory.Ship(2); // Low Stock alert triggered.
            inventory.ConfirmNotificationSent();

            var result = inventory.AdjustLowStockThreshold(15);

            result.IsSuccess.Should().BeFalse();

            result.Error.Should().Be(InventoryErrors.CannotAdjustThresholdWithActiveAlert);
        }

        [Fact]
        public void AdjustLowStockThreshold_WithDismissedLowStockAlert_ShouldResetLowStockAlert()
        {
            var inventory = InventoryBuilder.Create()
                .WithQuantity(21)
                .WithLowStockThreshold(20)
                .BuildSuccessfully();

            inventory.Ship(2); // Low Stock alert triggered.
            inventory.ConfirmNotificationSent();
            inventory.DismissLowStockAlert();

            var result = inventory.AdjustLowStockThreshold(15);

            inventory.LowStockAlert.Should().BeNull();
            result.IsSuccess.Should().BeTrue();

        }

        [Fact]
        public void AdjustLowStockThreshold_WithResolvedLowStockAlert_ShouldResetLowStockAlert()
        {
            var inventory = InventoryBuilder.Create()
                .WithQuantity(21)
                .WithLowStockThreshold(20)
                .BuildSuccessfully();

            inventory.Ship(2); // Low Stock alert triggered.
            inventory.ConfirmNotificationSent();
            inventory.Restock(10);

            var result = inventory.AdjustLowStockThreshold(15);

            inventory.LowStockAlert.Should().BeNull();
            result.IsSuccess.Should().BeTrue();

        }

        [Fact]
        public void AdjustLowStockThreshold_WithThresholdBelowCurrentStock_ShouldTriggerLowStockAlert()
        {
            var inventory = InventoryBuilder.Create()
                .WithQuantity(21)
                .WithLowStockThreshold(20)
                .BuildSuccessfully();



            var result = inventory.AdjustLowStockThreshold(25);

            result.IsSuccess.Should().BeTrue();

            inventory.LowStockAlert.Should().NotBeNull();

            inventory.LowStockAlert.Threshold.Should().Be(25);

            inventory.DomainEvents.Should().OnlyContain(X => X is LowStockAlertTriggeredDomainEvent);
        }

        #endregion

        #region Status method tests

        [Theory]
        [InlineData(15, 10, InventoryStatus.Healthy)]
        [InlineData(10, 10, InventoryStatus.Low)]
        [InlineData(5, 10, InventoryStatus.Low)]
        [InlineData(3, 10, InventoryStatus.Critical)]
        public void GetInventoryStatus_GivenQuantityandThreshold_ShouldReturnExcpectedStatus(int quantity, int lowStockThreshold, InventoryStatus expectedStatus)
        {
            var inventory = InventoryBuilder
                .Create()
                .WithQuantity(quantity + lowStockThreshold)
                .WithLowStockThreshold(lowStockThreshold)
                .BuildSuccessfully();

            inventory.Ship(lowStockThreshold);

            inventory.Status.Should().Be(expectedStatus);

        }


        #endregion
    }
}
