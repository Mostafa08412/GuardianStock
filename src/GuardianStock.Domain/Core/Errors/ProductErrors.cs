using GuardianStock.Domain.Core.Primitives;

namespace GuardianStock.Domain.Core.Errors
{

    public static partial class Errors
    {
        public static class ProductErrors
        {
            public static Error ProductNotFound = new Error("Product.NotFound", "Product not found!", ErrorType.NotFound);
            public static Error ProductIdIsRequired => new Error("Product.ProductIdIsRequired-ProductId", "The product identifier is required.", ErrorType.Validation);
            public static Error NameIsRequired => new Error("Product.NameIsRequired", "Product name is required", ErrorType.Validation);
            public static Error NameTooLong => new Error("Product.NameTooLong-Name", "Product name must not exceed 100 characters", ErrorType.Validation);
            public static Error DescriptionIsRequired => new Error("Product.DescriptionIsRequired", "Product description is required", ErrorType.Validation);
            public static Error DescriptionTooLong => new Error("Product.DescriptionTooLong-Description", "Product description must not exceed 500 characters", ErrorType.Validation);
            public static Error SkuIsRequired => new Error("Product.SkuIsRequired", "Product sku is required", ErrorType.Validation);
            public static Error InvalidPrice => new Error("Product.InvalidPrice", "Product price must be greater than zero", ErrorType.Validation);
            public static Error SupplierIsRequired => new Error("Product.SupplierIsRequired", "Product supplier is required", ErrorType.Validation);
            public static Error SupplierTooLong => new Error("Product.SupplierTooLong-Supplier", "Product supplier must not exceed 100 characters", ErrorType.Validation);
            public static Error CategoryIsRequired => new Error("Product.CategoryIsRequired", "Product category is required", ErrorType.Validation);
            public static Error ImageNotFound => new Error("Product.ImageNotFound", "Product image not found", ErrorType.NotFound);
            public static Error InvalidInitialQuantity => new Error("Product.InvalidInitialQuantity-InitialQuantity", "Initial quantity must be greater than or equal to zero", ErrorType.Validation);
            public static Error InvalidLowStockThreshold => new Error("Product.InvalidLowStockThreshold-LowStockThreshold", "Low stock threshold must be greater than zero", ErrorType.Validation);
        }
    }

}
