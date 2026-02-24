using GuardianStock.Domain.Core.Primitives;

namespace GuardianStock.Domain.Core.Errors
{

    public static partial class Errors
    {
        public static class ProductErrors
        {
            public static Error ProductNotFound = new Error("Product_NotFound", "Product not found!", ErrorType.NotFound);
            public static Error ProductIdIsRequired => new Error("Product_ProductIdIsRequired__ProductId", "The product identifier is required.", ErrorType.Validation);
            public static Error NameIsRequired => new Error("Product_NameIsRequired__Name", "Product name is required", ErrorType.Validation);
            public static Error NameTooLong => new Error("Product_NameTooLong__Name", "Product name must not exceed 100 characters", ErrorType.Validation);
            public static Error DescriptionIsRequired => new Error("Product_DescriptionIsRequired__Description", "Product description is required", ErrorType.Validation);
            public static Error DescriptionTooLong => new Error("Product_DescriptionTooLong__Description", "Product description must not exceed 500 characters", ErrorType.Validation);
            public static Error SkuIsRequired => new Error("Product_SkuIsRequired__Sku", "Product sku is required", ErrorType.Validation);
            public static Error InvalidPrice => new Error("Product_InvalidPrice__Price", "Product price must be greater than zero", ErrorType.Validation);
            public static Error SupplierIsRequired => new Error("Product_SupplierIsRequired__Supplier", "Product supplier is required", ErrorType.Validation);
            public static Error SupplierTooLong => new Error("Product_SupplierTooLong__Supplier", "Product supplier must not exceed 100 characters", ErrorType.Validation);
            public static Error CategoryIsRequired => new Error("Product_CategoryIsRequired__CategoryId", "Product category is required", ErrorType.Validation);
            public static Error ImageNotFound => new Error("Product_ImageNotFound", "Product image not found", ErrorType.NotFound);
            public static Error InvalidInitialQuantity => new Error("Product_InvalidInitialQuantity__InitialQuantity", "Initial quantity must be greater than or equal to zero", ErrorType.Validation);
            public static Error InvalidLowStockThreshold => new Error("Product_InvalidLowStockThreshold__LowStockThreshold", "Low stock threshold must be greater than zero", ErrorType.Validation);
        }
    }

}
