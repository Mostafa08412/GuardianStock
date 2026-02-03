namespace IMS.Application.Transactions.Queries.ListTransactions
{
    public record TransactionDto(
        Guid Id,
        string Type,
        string ProductName,
        string ProductSku,
        int Quantity,
        decimal Amount,
        string UserName,
        DateTime Date);
}
