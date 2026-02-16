using IMS.Domain.Core.Primitives;

namespace IMS.Domain.Core.Errors
{
    public static partial class Errors
    {
        public class CategoryErrors
        {
            public static Error IdIsRequired => new Error("Category.IdIsRequired-Id", "The category identifier is required.", ErrorType.Validation);
            public static Error NameIsRequired => new Error("Category.NameIsRequired-Name", "Category name is required", ErrorType.Validation);
            public static Error NameTooLong => new Error("Category.NameTooLong-Name", "Category name must not exceed 100 characters", ErrorType.Validation);
            public static Error DescriptionIsRequired => new Error("Category.DescriptionIsRequired-Description", "Category description is required", ErrorType.Validation);
            public static Error DescriptionTooLong => new Error("Category.DescriptionTooLong-Description", "Category description must not exceed 500 characters", ErrorType.Validation);
            public static Error NotFound => new Error("Category.NotFound", "Category not found!", ErrorType.NotFound);
            public static Error NameAlreadyExists => new Error("Category.NameAlreadyExists", "Category with this name already exists", ErrorType.Conflict);
        }
    }
}
