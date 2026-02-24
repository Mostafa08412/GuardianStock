using GuardianStock.Domain.Core.Primitives;

namespace GuardianStock.Domain.Core.Errors
{
    public static partial class Errors
    {
        public class TransactionErrors
        {
            public static Error NotFound => new Error("Transaction_NotFound", "Transaction is not found!", ErrorType.NotFound);
            public static Error IdIsRequired => new Error("Transaction_IdIsRequired__Id", "Transaction ID is required.", ErrorType.Validation);
            public static Error InvalidProductId => new Error("Transaction_InvalidProductId__ProductId", "Product ID is required", ErrorType.Validation);
            public static Error InvalidQuantity => new Error("Transaction_InvalidQuantity__Quantity", "Quantity must be greater than zero", ErrorType.Validation);
            public static Error InvalidUnitPrice => new Error("Transaction_InvalidUnitPrice__UnitPrice", "Unit price must be greater than zero", ErrorType.Validation);
        }
    }
}
