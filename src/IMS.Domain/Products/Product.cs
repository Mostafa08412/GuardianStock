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

        private Product(Guid id, string name, string sku, string description, Guid categoryId) : base(id)
        {
            Name = name;
            Sku = sku;
            Description = description;
            CategoryId = categoryId;
        }

        public string Name { get; private set; }

        public string Sku { get; private set; } // IMMUTABLE

        public string Description { get; private set; }

        public Guid CategoryId { get; private set; }

        public DateTime CreatedOnUTC { get; private set; }

        public string CreatedBy { get; private set; }

        public DateTime UpdatedOnUTC { get; private set; }

        public string UpdatedBy { get; private set; }


        public static Result<Product> Create(string name, string sku, string description, Guid categoryId)
        {
            var newId = Guid.CreateVersion7();


            if (string.IsNullOrEmpty(name))
            {
                return Result<Product>.Failure(Errors.ProductErrors.NameIsRequired);
            }

            if (string.IsNullOrEmpty(sku))
            {
                return Result<Product>.Failure(Errors.ProductErrors.SkuIsRequired);
            }

            if (string.IsNullOrEmpty(description))
            {
                return Result<Product>.Failure(Errors.ProductErrors.DescriptionIsRequired);
            }

            if (categoryId == Guid.Empty)
            {
                return Result<Product>.Failure(Errors.ProductErrors.CategoryIsRequired);
            }

            var newProduct = new Product(newId, name, sku, description, categoryId);

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

        public static class Errors
        {

            public static class ProductErrors
            {
                public static Error NameIsRequired => new Error("Product.NameIsRequired", "Product name is required", ErrorType.Validation);
                public static Error DescriptionIsRequired => new Error("Product.DescriptionIsRequired", "Product description is required", ErrorType.Validation);
                public static Error SkuIsRequired => new Error("Product.SkuIsRequired", "Product sku is required", ErrorType.Validation);
                public static Error CategoryIsRequired => new Error("Product.CategoryIsRequired", "Product category is required", ErrorType.Validation);

            }

        }
    }
}
