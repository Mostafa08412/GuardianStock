using FluentValidation;
using IMS.Domain.Core.Primitives;
using IMS.Domain.Core.Primitives.Result;
using MediatR;
namespace IMS.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {

        private readonly IEnumerable<IValidator<TRequest>> _validators;


        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Check if validators is null or is empty

            if (_validators != null && _validators.Any())
            {
                foreach (var validator in _validators)
                {
                    var context = new ValidationContext<TRequest>(request);

                    var validationResult = validator.Validate(context);

                    if (validationResult.Errors.Any())
                    {

                        var Errors = validationResult.Errors.Select(
                            X => new Error(X.PropertyName, X.ErrorMessage, ErrorType.Validation)
                            );


                        //Check if Response is Generic Response<T> or Non-Generic Response
                        var IsResponseGenericResult =
                               typeof(TResponse).IsGenericType
                            && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>);



                        if (IsResponseGenericResult)
                        {
                            var T = typeof(TResponse).GetGenericArguments()[0];
                            var ResultOfT = typeof(Result<>).MakeGenericType(T);

                            return (TResponse)
                                ResultOfT.GetMethod("Failure", new Type[] { typeof(List<Error>) })!
                                .Invoke(null, new object[] { Errors })!;
                        }
                        else
                        {
                            return (TResponse)
                                typeof(Result).GetMethod("Failure", new Type[] { typeof(List<Error>) })!
                                .Invoke(null, new object[] { Errors })!;
                        }

                    }

                }
            }


            var response = await next();

            return response;

        }
    }
}
