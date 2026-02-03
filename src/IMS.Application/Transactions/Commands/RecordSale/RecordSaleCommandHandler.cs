using IMS.Domain.Abstractions;
using IMS.Domain.Core.Errors;
using IMS.Domain.Core.Primitives.Result;
using IMS.Domain.Transactions;
using MediatR;

namespace IMS.Application.Transactions.Queries.RecordSale
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
