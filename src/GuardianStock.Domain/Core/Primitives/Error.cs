using System.Text.Json.Serialization;

namespace GuardianStock.Domain.Core.Primitives
{
    public class Error : ValueObject
    {
        public string Code { get; } = string.Empty;

        [JsonIgnore]
        public ErrorType ErrorType { get; }
        public string Description { get; } = string.Empty;

        public Error(string code, string description, ErrorType errorType)
        {
            Code = code;
            Description = description;
            ErrorType = errorType;
        }

        public override string ToString() => $"{ErrorType.ToString()}-{Code}: {Description}";

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Code;
            yield return ErrorType;
            yield return Description;
        }
    }
}
