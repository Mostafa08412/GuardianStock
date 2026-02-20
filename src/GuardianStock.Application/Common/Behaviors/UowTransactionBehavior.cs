using GuardianStock.Domain.Abstractions;
using MediatR;

namespace GuardianStock.Application.Common.Behaviors
{
    internal class UowTransactionBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {

        private readonly IUnitOfWork _unitOfWork;

        public UowTransactionBehavior(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {

            if (!request.GetType().Name.EndsWith("Command"))
                return await next.Invoke();

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var response = await next.Invoke();
                await _unitOfWork.Complete(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return response;
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackAsync(cancellationToken);
                throw;

            }


        }
    }
}
