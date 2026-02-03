using IMS.Application.Common.Models;
using IMS.Domain.Core.Primitives.Result;
using IMS.Domain.Transactions;
using MediatR;

namespace IMS.Application.Transactions.Queries.ListTransactions
{
    public record ListTransactionsQuery : IRequest<Result<PaginatedList<TransactionDto>>>
    {
        public string? SearchTerm { get; init; }

        public string? Sku { get; init; }
        public TransactionType? Type { get; init; }
        public DateTime? FromDate { get; init; }
        public DateTime? ToDate { get; init; }
        public decimal? MinAmount { get; init; }
        public decimal? MaxAmount { get; init; }


        public string? SortBy { get; init; } = "date";
        public bool IsDescending { get; init; } = true;
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
