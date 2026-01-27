namespace IMS.Domain.Core.Primitives.Result
{
    public class Result<T> : Result
    {

        public T? Value { get; }

        public Result(bool isSuccess, IEnumerable<Error> errors, T? value)
         : base(isSuccess, errors)
        {

            Value = value;

        }
        public Result(bool isSuccess, IEnumerable<Error> errors, T? value, string message)
        : base(isSuccess, errors)
        {
            Message = message;
            Value = value;

        }
        public static Result<T> Success(T Value, string Message) => new Result<T>(true, Enumerable.Empty<Error>(), Value, Message);

        public static Result<T> Success(T Value) => new Result<T>(true, Enumerable.Empty<Error>(), Value);

        public new static Result<T> Failure(Error error) => new Result<T>(false, new List<Error>() { error }, default);

        public new static Result<T> Failure(IEnumerable<Error> Errors) => new Result<T>(false, Errors, default);


    }
}
