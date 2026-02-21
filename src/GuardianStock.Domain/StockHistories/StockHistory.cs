using GuardianStock.Domain.Core.Primitives;

namespace GuardianStock.Domain.StockHistories
{
    public class StockHistory : Aggregate
    {
        private StockHistory(Guid Id, Guid inventoryId, Guid? previousTransactionId, DateTime timestampUTC, int currentStock) : base(Id)
        {
            InventoryId = inventoryId;

            PreviousTransactionId = previousTransactionId;

            TimestampUTC = timestampUTC;

            CurrentStock = currentStock;
        }

        public Guid InventoryId { get; init; }

        public Guid? PreviousTransactionId { get; init; }

        public DateTime TimestampUTC { get; init; }

        public int CurrentStock { get; init; }


        public static StockHistory Create(Guid inventoryId, Guid? previousTransactionId, DateTime timestampUTC, int currentStock)
        {

            return new StockHistory(Guid.CreateVersion7(), inventoryId, previousTransactionId, timestampUTC, currentStock);
        }

    }
}
