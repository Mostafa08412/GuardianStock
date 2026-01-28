using IMS.Domain.Inventories;

namespace Domain.UnitTests.Builders
{
    public class InventoryBuilder
    {

        //Define default values for the Inventory.
        // Builder Create()
        // builder With[Property_Name](Property_Value)
        // Result<Inventory> Build()
        // BuildSuccessfully Result.IsSuccess => returns the Inventory: Result.IsFailure => throws Exception

        private int _quantity = 12;

        public int _lowStockThreshold = 10;

        public Guid _productId = Guid.NewGuid();



        public static InventoryBuilder Create() => new();


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

        public Result<Inventory> Build()
        {
            return Inventory.Create(_quantity, _lowStockThreshold, _productId);
        }

        public Inventory BuildSuccessfully()
        {
            var result = Build();

            if (result.IsSuccess)

                return result.Value!;
            else

                throw new InvalidOperationException(
               $"Failed to build inventory: {result.Error!.Description}");


        }

    }
}
