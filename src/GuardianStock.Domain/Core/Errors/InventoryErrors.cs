using GuardianStock.Domain.Core.Primitives;
using GuardianStock.Domain.Inventories;

namespace GuardianStock.Domain.Core.Errors
{
    public static partial class Errors
    {
        public class InventoryErrors
        {
            // --- New Error ---
            public static Error InventoryIdIsRequired => new Error(
                "Inventory_InventoryIdIsRequired__InventoryId",
                "The inventory identifier is required.",
                ErrorType.Validation);

            // --- Existing Errors ---
            public static Error InvalidQuantity => new Error("Inventory_InvalidQuantity__Quantity", "Inventory quantity must be greater than 1.", ErrorType.Validation);

            public static Error NotFound => new Error("Inventory_NotFound", "This Product Inventory is not found", ErrorType.NotFound);

            public static Error InvalidLowStockThreshold => new Error("Inventory_InvalidLowStockThreshold__Threshold", $"Low stock threshold minimum is {Inventory.MinimumLowStockThreshold}.", ErrorType.Validation);

            public static Error ProductIsRequired => new Error("Inventory_ProductIsRequired__Product", "Inventory product is required.", ErrorType.Validation);

            public static Error InvalidShipQuantity => new Error("Inventory_InvalidShipQuantity__Quantity", "The quantity to ship must be greater than zero.", ErrorType.Validation);

            public static Error InvalidRestockQuantity => new Error("Inventory_InvalidRestockQuantity__Quantity", "The quantity to restock must be greater than zero.", ErrorType.Validation);

            // Changed to => for consistency
            public static Error InsufficientStock => new("Inventory_InsufficientStock", "The available quantity in stock is not enough to fulfill this request.", ErrorType.ConditionNotMet);

            // Changed to => for consistency
            public static Error QuantityCannotBeLowerThanThreshold => new("Inventory_QuantityCannotBeLowerThanThreshold__NewLowStockThreshold", $"The quantity must be greater than {Inventory.MinimumLowStockThreshold}", ErrorType.Validation);

            public static Error CannotAdjustThresholdWithActiveAlert => new(
                "Inventory_CannotAdjustThresholdWithActiveAlert",
                "The low stock threshold cannot be adjusted while there is an active alert. Please dismiss or reset the alert first.",
                ErrorType.ConditionNotMet);

            public static Error ProductIdIsRequired => new Error(
                "Inventory_ProductIdIsRequired__ProductId",
                "ProductId is required to send low stock alert.",
                ErrorType.Validation);
        }
    }
}