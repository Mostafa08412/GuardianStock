namespace IMS.Domain.Core.Primitives.Result
{



    public class Result
    {
        protected Result(bool isSuccess, IEnumerable<Error> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors.ToArray();
        }

        protected Result(bool isSuccess, IEnumerable<Error> errors, string message)
        {
            IsSuccess = isSuccess;
            Errors = errors.ToArray();
            Message = message;
        }


        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Message { get; init; } = string.Empty;

        public Error? Error => Errors.FirstOrDefault();
        public IReadOnlyCollection<Error> Errors { get; }

        public static Result Success()
            => new Result(true, Array.Empty<Error>());

        public static Result Success(string Message)
            => new Result(true, Array.Empty<Error>(), Message);

        public static Result Failure(Error error)
            => new Result(false, new[] { error });

        public static Result Failure(IEnumerable<Error> errors)
        {
            if (errors == null || !errors.Any())
                throw new ArgumentException("Failure result must contain at least one error.");

            return new Result(false, errors.ToArray());
        }
    }

}
