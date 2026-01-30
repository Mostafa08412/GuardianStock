using IMS.Domain.Abstractions;
using IMS.Domain.Core.Primitives;
using IMS.Domain.Core.Primitives.Result;

namespace IMS.Domain.Transactions
{
    public class Transaction : Aggregate, IAuditable
    {
        protected Transaction() : base()
        {
        }

        private Transaction(Guid id, int quantity, decimal unitPrice, Guid productId, TransactionType type) : base(id)
        {
            Quantity = quantity;
            UnitPrice = unitPrice;
            ProductId = productId;
            Type = type;
            CreatedOnUTC = DateTime.UtcNow;
            // TotalAmount is calculated property, no need to set
        }

        public int Quantity { get; private set; }

        public decimal UnitPrice { get; private set; }

        public decimal TotalAmount => Quantity * UnitPrice;

        public Guid ProductId { get; private set; }

        public TransactionType Type { get; private set; }

        public DateTime CreatedOnUTC { get; private set; }

        public string CreatedBy { get; private set; }

        public DateTime UpdatedOnUTC { get; private set; }

        public string UpdatedBy { get; private set; }


        public static Result<Transaction> RecordSale(Guid productId, int quantity, decimal unitPrice)
        {
            if (productId == Guid.Empty)
            {
                return Result<Transaction>.Failure(Errors.TransactionErrors.InvalidProductId);
            }

            if (quantity <= 0)
            {
                return Result<Transaction>.Failure(Errors.TransactionErrors.InvalidQuantity);
            }

            if (unitPrice <= 0)
            {
                return Result<Transaction>.Failure(Errors.TransactionErrors.InvalidUnitPrice);
            }

            var newId = Guid.CreateVersion7();

            var transaction = new Transaction(newId, quantity, unitPrice, productId, TransactionType.Sale);

            transaction.AddDomainEvent(new TransactionCreatedDomainEvent(transaction.Id, productId, unitPrice, quantity, TransactionType.Sale));

            return Result<Transaction>.Success(transaction);
        }

        public static Result<Transaction> RecordPurchase(Guid productId, int quantity, decimal unitPrice)
        {
            if (productId == Guid.Empty)
            {
                return Result<Transaction>.Failure(Errors.TransactionErrors.InvalidProductId);
            }

            if (quantity <= 0)
            {
                return Result<Transaction>.Failure(Errors.TransactionErrors.InvalidQuantity);
            }

            if (unitPrice <= 0)
            {
                return Result<Transaction>.Failure(Errors.TransactionErrors.InvalidUnitPrice);
            }

            var newId = Guid.CreateVersion7();

            var transaction = new Transaction(newId, quantity, unitPrice, productId, TransactionType.Purchase);

            transaction.AddDomainEvent(new TransactionCreatedDomainEvent(transaction.Id, productId, unitPrice, quantity, TransactionType.Purchase));

            return Result<Transaction>.Success(transaction);
        }
    }

    public static class Errors
    {
        public static class TransactionErrors
        {
            public static Error InvalidProductId => new Error("Transaction.InvalidProductId", "Product ID is required", ErrorType.Validation);
            public static Error InvalidQuantity => new Error("Transaction.InvalidQuantity", "Quantity must be greater than zero", ErrorType.Validation);
            public static Error InvalidUnitPrice => new Error("Transaction.InvalidUnitPrice", "Unit price must be greater than zero", ErrorType.Validation);
        }
    }

    public record TransactionCreatedDomainEvent : IDomainEvent
    {
        public TransactionCreatedDomainEvent(Guid transactionId, Guid productId, decimal unitPrice, int quantity, TransactionType type)
        {
            TransactionId = transactionId;
            ProductId = productId;
            UnitPrice = unitPrice;
            Quantity = quantity;
            Type = type;
        }

        public Guid TransactionId { get; init; }
        public Guid ProductId { get; init; }

        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
        public decimal TotalAmount => UnitPrice * Quantity;

        public TransactionType Type { get; init; }
    }
}
