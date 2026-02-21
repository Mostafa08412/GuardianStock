namespace GuardianStock.Application.Transactions.Queries.GetTransaction
{
    public record TransactionDetailsDto(
        Guid Id,
        string ProductName,
        string ProductSku,
        decimal UnitPrice,
        int Quantity,
        decimal TotalAmount,
        string TransactionType,
        DateTime CreatedDate,
        string CreatedByUser
    );


}
