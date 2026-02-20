using FluentValidation;
using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Domain.Core.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GuardianStock.Application.Transactions.Queries.GetTransaction
{
    public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, Result<TransactionDetailsDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetTransactionQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<TransactionDetailsDto>> Handle(GetTransactionQuery request, CancellationToken ct)
        {
            var query = from transaction in _context.Transactions.AsNoTracking()
                        where transaction.Id == request.Id

                        join product in _context.Products on transaction.ProductId equals product.Id into prodGroup
                        from product in prodGroup.DefaultIfEmpty()

                        join user in _context.BusinessUsers on transaction.CreatedBy equals user.Id into userGroup
                        from user in userGroup.DefaultIfEmpty()

                        select new TransactionDetailsDto(
                            transaction.Id,
                            product.Name,
                            product.Sku,
                            transaction.UnitPrice,
                            transaction.Quantity,
                            transaction.UnitPrice * transaction.Quantity,
                            transaction.Type.ToString(),
                            transaction.CreatedOnUTC.ToLocalTime(),
                            user != null ? $"{user.FirstName} {user.LastName}" : "System"
                        );
            var result = await query.FirstOrDefaultAsync(ct);

            if (result is null)
                return Result<TransactionDetailsDto>.Failure(Errors.TransactionErrors.NotFound);

            return Result<TransactionDetailsDto>.Success(result); ;
        }
    }


}
