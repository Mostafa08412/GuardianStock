using IMS.Application.Common.Interfaces;
using IMS.Domain.Abstractions;
using MediatR;

namespace IMS.Application.Common.Behaviors
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

            if (request is not ICommandRequest)
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
