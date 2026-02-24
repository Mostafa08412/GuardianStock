using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Core.Primitives;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Domain.Transactions;


namespace GuardianStock.Domain.Transactions
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
        }

        public int Quantity { get; private set; }

        public decimal UnitPrice { get; private set; }

        public decimal TotalAmount => Quantity * UnitPrice;

        public Guid ProductId { get; private set; }

        public TransactionType Type { get; private set; }

        public DateTime CreatedOnUTC { get; private set; }

        public Guid? CreatedBy { get; private set; }

        public DateTime UpdatedOnUTC { get; private set; }

        public Guid? UpdatedBy { get; private set; }

        // For seeding with fake datetime.
        public Transaction(Guid productId, int quantity, decimal unitPrice, DateTime date, int type)
        {
            this.CreatedOnUTC = date;
            this.ProductId = productId;
            this.UnitPrice = unitPrice;
            this.Quantity = quantity;
            this.Type = type == 0 ? TransactionType.Sale : TransactionType.Purchase;
        }

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
}
