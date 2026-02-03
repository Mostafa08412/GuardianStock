using IMS.Domain.Abstractions;
using IMS.Domain.Core.Errors;
using IMS.Domain.Core.Primitives.Result;
using IMS.Domain.Transactions;
using MediatR;

namespace IMS.Application.Transactions.Commands.RecordPurchase
{
    public class RecordPurchaseCommandHandler : IRequestHandler<RecordPurchaseCommand, Result>
    {
        private readonly IUnitOfWork unitOfWork;

        public RecordPurchaseCommandHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RecordPurchaseCommand request, CancellationToken cancellationToken)
        {
            var product = await unitOfWork.Products.GetByIdAsync(request.productId, cancellationToken);

            if (product is null)
                return Result.Failure(Errors.ProductErrors.ProductNotFound);

            var recordPurchaseTransactionResult = Transaction.RecordPurchase(request.productId, request.quantity, product.Price);

            if (recordPurchaseTransactionResult.IsFailure)
                return Result.Failure(recordPurchaseTransactionResult.Errors);

            unitOfWork.Transactions.Add(recordPurchaseTransactionResult.Value!);

            return Result.Success();


        }
    }
}
