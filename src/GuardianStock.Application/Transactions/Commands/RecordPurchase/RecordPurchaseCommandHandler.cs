using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Domain.Transactions;
using GuardianStock.Domain.Core.Errors;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GuardianStock.Application.Transactions.Commands.RecordPurchase
{
    public class RecordPurchaseCommandHandler : IRequestHandler<RecordPurchaseCommand, Result>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<RecordPurchaseCommandHandler> logger;

        public RecordPurchaseCommandHandler(IUnitOfWork unitOfWork, ILogger<RecordPurchaseCommandHandler> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public async Task<Result> Handle(RecordPurchaseCommand request, CancellationToken cancellationToken)
        {

            logger.LogInformation("Purchase Recording Started: Product {ProductId} | Quantity: {Quantity}",
                request.productId, request.quantity);

            var product = await unitOfWork.Products.GetByIdAsync(request.productId, cancellationToken);

            if (product is null)
            {

                logger.LogWarning("Purchase Recording Failed: Product {ProductId} not found.", request.productId);
                return Result.Failure(Errors.ProductErrors.ProductNotFound);
            }


            logger.LogInformation("Calculating Purchase Transaction: Product {ProductName} ({ProductId}) | Price: {Price}",
                product.Name, request.productId, product.Price);

            var recordPurchaseTransactionResult = Transaction.RecordPurchase(request.productId, request.quantity, product.Price);

            if (recordPurchaseTransactionResult.IsFailure)
            {

                logger.LogWarning("Purchase Recording Rejected: Product {ProductId} | Reason: {Error}",
                    request.productId, recordPurchaseTransactionResult.Errors.FirstOrDefault()?.Description);

                return Result.Failure(recordPurchaseTransactionResult.Errors);
            }

            unitOfWork.Transactions.Add(recordPurchaseTransactionResult.Value!);


            logger.LogInformation("Purchase Recording Completed: Product {ProductId} | Transaction Recorded Successfully.",
                request.productId);

            return Result.Success();
        }
    }
}
