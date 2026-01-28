using IMS.Domain.Abstractions;
using IMS.Domain.Categories;
using IMS.Domain.Core.Primitives;
using IMS.Domain.Core.Primitives.Result;

namespace IMS.Domain.Products
{
    public class Product : Aggregate, IAuditable
    {
        protected Product() : base()
        {
        }

        private Product(Guid id, string name, string sku, string description, decimal price, string supplier, Guid categoryId) : base(id)
        {
            Name = name;
            Sku = sku;
            Description = description;
            Price = price;
            Supplier = supplier;
            CategoryId = categoryId;
        }

        public string Name { get; private set; }

        public string Sku { get; private set; } // IMMUTABLE

        public string Description { get; private set; }

        public decimal Price { get; private set; }

        public string Supplier { get; private set; }

        public Guid CategoryId { get; private set; }

        public DateTime CreatedOnUTC { get; private set; }

        public string CreatedBy { get; private set; }

        public DateTime UpdatedOnUTC { get; private set; }

        public string UpdatedBy { get; private set; }


        public static Result<Product> Create(string name, string sku, string description, decimal price, string supplier, Guid categoryId)
        {
            var newId = Guid.CreateVersion7();

            if (string.IsNullOrWhiteSpace(name))
            {
                return Result<Product>.Failure(Errors.ProductErrors.NameIsRequired);
            }

            if (string.IsNullOrWhiteSpace(sku))
            {
                return Result<Product>.Failure(Errors.ProductErrors.SkuIsRequired);
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                return Result<Product>.Failure(Errors.ProductErrors.DescriptionIsRequired);
            }

            if (price <= 0)
            {
                return Result<Product>.Failure(Errors.ProductErrors.InvalidPrice);
            }

            if (string.IsNullOrWhiteSpace(supplier))
            {
                return Result<Product>.Failure(Errors.ProductErrors.SupplierIsRequired);
            }

            if (categoryId == Guid.Empty)
            {
                return Result<Product>.Failure(Errors.ProductErrors.CategoryIsRequired);
            }

            var newProduct = new Product(newId, name, sku, description, price, supplier, categoryId);

            return Result<Product>.Success(newProduct);
        }




        public Result Rename(string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                return Result<Category>.Failure(Errors.ProductErrors.NameIsRequired);
            }

            Name = newName;

            return Result.Success();

        }

        public Result Redescription(string newDescription)
        {
            if (string.IsNullOrEmpty(newDescription))
            {
                return Result<Category>.Failure(Errors.ProductErrors.DescriptionIsRequired);
            }

            Description = newDescription;

            return Result.Success();

        }

        public Result Recategorize(Guid newCategoryId)
        {
            if (newCategoryId == Guid.Empty)
            {
                return Result<Product>.Failure(Errors.ProductErrors.CategoryIsRequired);
            }

            CategoryId = newCategoryId;

            return Result.Success();
        }

        public Result UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0)
            {
                return Result.Failure(Errors.ProductErrors.InvalidPrice);
            }

            Price = newPrice;

            return Result.Success();
        }

        public Result UpdateSupplier(string newSupplier)
        {
            if (string.IsNullOrWhiteSpace(newSupplier))
            {
                return Result.Failure(Errors.ProductErrors.SupplierIsRequired);
            }

            Supplier = newSupplier;

            return Result.Success();
        }

        public static class Errors
        {

            public static class ProductErrors
            {
                public static Error NameIsRequired => new Error("Product.NameIsRequired", "Product name is required", ErrorType.Validation);
                public static Error DescriptionIsRequired => new Error("Product.DescriptionIsRequired", "Product description is required", ErrorType.Validation);
                public static Error SkuIsRequired => new Error("Product.SkuIsRequired", "Product sku is required", ErrorType.Validation);
                public static Error InvalidPrice => new Error("Product.InvalidPrice", "Product price must be greater than zero", ErrorType.Validation);
                public static Error SupplierIsRequired => new Error("Product.SupplierIsRequired", "Product supplier is required", ErrorType.Validation);
                public static Error CategoryIsRequired => new Error("Product.CategoryIsRequired", "Product category is required", ErrorType.Validation);
            }

        }
    }
}
