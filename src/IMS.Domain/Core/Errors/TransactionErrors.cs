using IMS.Domain.Core.Primitives;

namespace IMS.Domain.Core.Errors
{
    public static partial class Errors
    {
        public class TransactionErrors
        {

            public static Error InvalidProductId => new Error("Transaction.InvalidProductId", "Product ID is required", ErrorType.Validation);
            public static Error InvalidQuantity => new Error("Transaction.InvalidQuantity", "Quantity must be greater than zero", ErrorType.Validation);
            public static Error InvalidUnitPrice => new Error("Transaction.InvalidUnitPrice", "Unit price must be greater than zero", ErrorType.Validation);

        }
    }
}
