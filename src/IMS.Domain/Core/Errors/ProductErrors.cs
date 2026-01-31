using IMS.Domain.Core.Primitives;

namespace IMS.Domain.Core.Errors
{

    public static partial class Errors
    {
        public static class ProductErrors
        {
            public static Error ProductNotFound = new Error("Product.NotFound", "Product not found!", ErrorType.NotFound);
            public static Error NameIsRequired => new Error("Product.NameIsRequired", "Product name is required", ErrorType.Validation);
            public static Error DescriptionIsRequired => new Error("Product.DescriptionIsRequired", "Product description is required", ErrorType.Validation);
            public static Error SkuIsRequired => new Error("Product.SkuIsRequired", "Product sku is required", ErrorType.Validation);
            public static Error InvalidPrice => new Error("Product.InvalidPrice", "Product price must be greater than zero", ErrorType.Validation);
            public static Error SupplierIsRequired => new Error("Product.SupplierIsRequired", "Product supplier is required", ErrorType.Validation);
            public static Error CategoryIsRequired => new Error("Product.CategoryIsRequired", "Product category is required", ErrorType.Validation);
        }
    }

}
