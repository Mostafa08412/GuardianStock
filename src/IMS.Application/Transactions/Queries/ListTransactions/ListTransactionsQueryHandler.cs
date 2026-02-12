using FluentValidation;
using IMS.Application.Common.Interfaces;
using IMS.Application.Common.Models;
using IMS.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Transactions.Queries.ListTransactions
{
    public class ListTransactionsQueryHandler : IRequestHandler<ListTransactionsQuery, Result<PaginatedList<TransactionDto>>>
    {
        private readonly IApplicationDbContext _context;

        public ListTransactionsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedList<TransactionDto>>> Handle(ListTransactionsQuery request, CancellationToken ct)
        {

            var query = from transction in _context.Transactions.AsNoTracking()
                        join product in _context.Products.AsNoTracking()
                        on transction.ProductId equals product.Id
                        join user in _context.BusinessUsers.AsNoTracking()
                        on transction.CreatedBy equals user.Id.ToString()
                        select new
                        {
                            Id = transction.Id,
                            ProductName = product.Name,
                            ProductSku = product.Sku,
                            Price = transction.UnitPrice,
                            Total = transction.UnitPrice * transction.Quantity,
                            Quantity = transction.Quantity,
                            Type = transction.Type,
                            Date = transction.CreatedOnUTC,
                            User = user.FirstName + " " + user.LastName,
                        };

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(t => t.ProductName.Contains(request.SearchTerm));

            if (!string.IsNullOrWhiteSpace(request.Sku))
                query = query.Where(t => t.ProductSku == request.Sku);

            if (request.Type.HasValue)
                query = query.Where(t => t.Type == request.Type.Value);

            if (request.FromDate.HasValue)
                query = query.Where(t => t.Date >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(t => t.Date <= request.ToDate.Value);


            query = request.SortBy switch
            {
                "amount" => request.IsDescending ? query.OrderByDescending(t => t.Total) : query.OrderBy(t => t.Total),
                "quantity" => request.IsDescending ? query.OrderByDescending(t => t.Quantity) : query.OrderBy(t => t.Quantity),
                _ => request.IsDescending ? query.OrderByDescending(t => t.Date) : query.OrderBy(t => t.Date)
            };

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new TransactionDto(
                    t.Id,
                    t.Type.ToString(),
                    t.ProductName,
                    t.ProductSku,
                    t.Quantity,
                    t.Total,
                    t.User,
                    t.Date.ToLocalTime()))
                .ToListAsync(ct);

            return Result<PaginatedList<TransactionDto>>.Success(new PaginatedList<TransactionDto>(items, totalCount, request.PageNumber, request.PageSize));
        }
    }
}
