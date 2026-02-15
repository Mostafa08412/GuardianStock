using IMS.Domain.Core.Primitives;
using IMS.Domain.Inventories;

namespace IMS.Domain.Core.Errors
{
    public static partial class Errors
    {
        public class InventoryErrors
        {
            public static Error InvalidQuantity => new Error("Inventory.InvalidQuantity", "Inventory quantity must be greater than 1.", ErrorType.Validation);
            public static Error NotFound => new Error("Inventory.NotFound", "This Product Inventory is not found", ErrorType.Validation);
            public static Error InvalidLowStockThreshold => new Error("Inventory.InvalidLowStockThreshold", $"Low stock threshold minimum is {Inventory.MinimumLowStockThreshold}.", ErrorType.Validation);
            public static Error ProductIsRequired => new Error("Inventory.ProductIsRequired", "Inventory product is required.", ErrorType.Validation);

            public static Error InvalidShipQuantity => new Error("Inventory.InvalidShipQuantity", "The quantity to ship must be greater than zero.", ErrorType.Validation);

            public static Error InvalidRestockQuantity => new Error("Inventory.InvalidRestockQuantity", "The quantity to restock must be greater than zero.", ErrorType.Validation);

            public static Error InsufficientStock = new("Inventory.InsufficientStock", "The available quantity in stock is not enough to fulfill this request.", ErrorType.ConditionNotMet);

            public static Error QuantityCannotBeLowerThanThreshold = new("Inventory.QuantityCannotBeLowerThanThreshold", $"The quantity must be greater than {Inventory.MinimumLowStockThreshold}", ErrorType.Validation);
            public static Error CannotAdjustThresholdWithActiveAlert => new(
            "Inventory.CannotAdjustThresholdWithActiveAlert",
            "The low stock threshold cannot be adjusted while there is an active alert. Please dismiss or reset the alert first.",
            ErrorType.ConditionNotMet);
        }
    }

}
