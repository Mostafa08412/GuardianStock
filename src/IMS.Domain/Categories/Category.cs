using IMS.Domain.Abstractions;
using IMS.Domain.Core.Errors;
using IMS.Domain.Core.Primitives;
using IMS.Domain.Core.Primitives.Result;

namespace IMS.Domain.Categories
{
    public class Category : Aggregate, IAuditable
    {
        protected Category()
        {
        }

        private Category(Guid id, string name, string description) : base(id)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public DateTime CreatedOnUTC { get; private set; }

        public string CreatedBy { get; private set; }

        public DateTime UpdatedOnUTC { get; private set; }

        public string UpdatedBy { get; private set; }


        public static Result<Category> Create(string name, string description)
        {
            var newId = Guid.CreateVersion7();


            if (string.IsNullOrWhiteSpace(name))
            {
                return Result<Category>.Failure(Errors.CategoryErrors.NameIsRequired);
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                return Result<Category>.Failure(Errors.CategoryErrors.DescriptionIsRequired);
            }

            var newCategory = new Category(newId, name, description);

            return Result<Category>.Success(newCategory);
        }

        public Result Rename(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                return Result<Category>.Failure(Errors.CategoryErrors.NameIsRequired);
            }

            Name = newName;

            return Result.Success();

        }

        public Result Redescription(string newDescription)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
            {
                return Result<Category>.Failure(Errors.CategoryErrors.DescriptionIsRequired);
            }

            Description = newDescription;

            return Result.Success();

        }

    }


}
