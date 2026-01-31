using IMS.Domain.Core.Primitives;

namespace IMS.Domain.Core.Errors
{
    public static partial class Errors
    {
        public class CategoryErrors
        {
            public static Error NameIsRequired => new Error("Category.NameIsRequired", "Category name is required", ErrorType.Validation);
            public static Error DescriptionIsRequired => new Error("Category.DescriptionIsRequired", "Category description is required", ErrorType.Validation);
            public static Error NotFound => new Error("Category.NotFound", "Category not found!", ErrorType.NotFound);
        }
    }
}
