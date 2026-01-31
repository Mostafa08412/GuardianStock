using IMS.Domain.Abstractions;


namespace IMS.Domain.Transactions
{
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
