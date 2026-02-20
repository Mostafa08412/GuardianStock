using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Domain.Transactions;
using GuardianStock.Domain.Core.Errors;
using MediatR;

namespace GuardianStock.Application.Transactions.Commands.RecordSale
{
    public class RecordSaleCommandHandler : IRequestHandler<RecordSaleCommand, Result>
    {
        private readonly IUnitOfWork unitOfWork;

        public RecordSaleCommandHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RecordSaleCommand request, CancellationToken cancellationToken)
        {
            var product = await unitOfWork.Products.GetByIdAsync(request.productId, cancellationToken);

            if (product is null)
                return Result.Failure(Errors.ProductErrors.ProductNotFound);

            var isAvailableStockAsync = await unitOfWork.Inventories.IsAvailableStockAsync(request.productId, request.quantity, cancellationToken);

            if (!isAvailableStockAsync)
                return Result.Failure(Errors.InventoryErrors.InsufficientStock);


            var recordSaleTransactionResult = Transaction.RecordSale(request.productId, request.quantity, product.Price);

            if (recordSaleTransactionResult.IsFailure)
                return Result.Failure(recordSaleTransactionResult.Errors);

            unitOfWork.Transactions.Add(recordSaleTransactionResult.Value!);

            return Result.Success();


        }
    }
}
