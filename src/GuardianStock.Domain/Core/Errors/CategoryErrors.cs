using GuardianStock.Domain.Core.Primitives;

namespace GuardianStock.Domain.Core.Errors
{
    public static partial class Errors
    {
        public class CategoryErrors
        {
            public static Error IdIsRequired => new Error("Category_IdIsRequired__Id", "The category identifier is required.", ErrorType.Validation);
            public static Error NameIsRequired => new Error("Category_NameIsRequired__Name", "Category name is required", ErrorType.Validation);
            public static Error NameTooLong => new Error("Category_NameTooLong__Name", "Category name must not exceed 100 characters", ErrorType.Validation);
            public static Error DescriptionIsRequired => new Error("Category_DescriptionIsRequired__Description", "Category description is required", ErrorType.Validation);
            public static Error DescriptionTooLong => new Error("Category_DescriptionTooLong__Description", "Category description must not exceed 500 characters", ErrorType.Validation);
            public static Error NotFound => new Error("Category_NotFound", "Category not found!", ErrorType.NotFound);
            public static Error NameAlreadyExists => new Error("Category_NameAlreadyExists", "Category with this name already exists", ErrorType.Conflict);
        }
    }
}
