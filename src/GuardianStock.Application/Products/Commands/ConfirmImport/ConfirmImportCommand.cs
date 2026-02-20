using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Application.Products.Commands.GeneratePreview.Dtos;
using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Core.Primitives;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Application.Products.Commands.BulkImportProducts;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;

namespace GuardianStock.Application.Products.Commands.ConfirmImport
{
    public class ConfirmImportCommand : IRequest<Result<Unit>>
    {
        public string PreviewId { get; init; }
        public string JobId { get; init; }
    }

    public class ConfirmImportCommandHandler : IRequestHandler<ConfirmImportCommand, Result<Unit>>
    {

        private readonly HybridCache _cache;

        private readonly IUnitOfWork _uow;

        private readonly IBackgroundJobWorker _backgroundJobWorker;

        private readonly ICurrentUser _user;

        public ConfirmImportCommandHandler(HybridCache cache, IUnitOfWork uow, IBackgroundJobWorker backgroundJobWorker, ICurrentUser user)
        {
            _cache = cache;
            _uow = uow;
            _backgroundJobWorker = backgroundJobWorker;
            _user = user;
        }

        public async Task<Result<Unit>> Handle(ConfirmImportCommand request, CancellationToken cancellationToken)
        {
            var importProductsReport = await _cache.GetOrCreateAsync<GeneratePreviewResponse?>(
                request.PreviewId,
                factory: async (cancellationToken) =>
                {
                    return null;
                },
                cancellationToken: cancellationToken
                );
            if (importProductsReport is null)
                return Result<Unit>.Failure(new Error("Preview.Expired", "The preview is no longer exists, please upload a csv file again.", ErrorType.NotFound));

            _backgroundJobWorker.EnqueueImportProductsFromPreviewJob(new BulkImportProductsJob { PreviewId = request.PreviewId, JobId = request.JobId, UserId = _user.UserId });

            return Result<Unit>.Success(Unit.Value);
        }
    }

}