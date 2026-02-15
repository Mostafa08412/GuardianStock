using IMS.Application.Common.Interfaces;
using IMS.Application.Products.Commands.GeneratePreview.Dtos;
using IMS.Domain.Abstractions;
using IMS.Domain.Inventories;
using IMS.Domain.Products;
using IMS.Domain.StockHistories;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;

namespace IMS.Application.Products.Commands.BulkImportProducts
{
    public record BulkImportProductsJob : IRequest
    {
        public string JobId { get; init; }

        public string PreviewId { get; init; }

        public string UserId { get; init; }
    }

    public class ImportSummary
    {
        public string JobId { get; set; }

        public int TotalImported { get; set; }

        public int TotalFailed { get; set; }

        public DateTime CompletedAt { get; set; }

    }



    public class BulkImportProductsCommandHandler : IRequestHandler<BulkImportProductsJob>
    {

        private readonly HybridCache _cache;

        private readonly ISkuGenerator _skuGenerator;

        private readonly IUnitOfWork _uow;

        private readonly IDateTime _dateTime;

        private readonly IImportService _signalService;


        public BulkImportProductsCommandHandler(HybridCache cache, ISkuGenerator skuGenerator, IUnitOfWork uow, IDateTime dateTime, IImportService signalService)
        {
            _cache = cache;
            _skuGenerator = skuGenerator;
            _uow = uow;
            _dateTime = dateTime;
            _signalService = signalService;
        }

        public async Task Handle(BulkImportProductsJob request, CancellationToken cancellationToken)
        {
            int failedImportsCount = 0;

            var importProductsReport = await _cache.GetOrCreateAsync<GeneratePreviewResponse?>(
             request.PreviewId,
             factory: async (cancellationToken) =>
             {
                 return null;
             },
             cancellationToken: cancellationToken
             );

            if (importProductsReport is null)
            {
                await _signalService.OnJobFailed(request.UserId, request.JobId, "The preview is missing, please upload a csv again.", cancellationToken);

                throw new Exception($"Import Preview with id {request.PreviewId} is not existing in the cache.");

            }

            var products = new List<Domain.Products.Product>();
            var inventories = new List<Domain.Inventories.Inventory>();

            foreach (var pM in importProductsReport.RowResults)
            {
                if (pM.IsValid)
                {
                    var result = Domain.Products.Product.Create(pM.Name, _skuGenerator.GenerateSKU(pM.Supplier), pM.Description, Decimal.Parse(pM.Price), pM.Supplier, new Guid(pM.CategoryId));
                    result.Value.CreatedBy = request.UserId;
                    if (result.IsFailure) failedImportsCount++;

                    else
                    {
                        products.Add(result.Value!);
                        var quantity = int.Parse(pM.Quantity);
                        var inventory = Inventory.Create(quantity, 10, result.Value.Id).Value!;
                        inventories.Add(inventory);
                        _uow.StockHistories.Add(StockHistory.Create(inventory.Id, null, _dateTime.UTCNow, quantity));

                    }

                }

            }

            var importSummary = new ImportSummary();

            importSummary.JobId = request.JobId;
            importSummary.TotalImported = products.Count();
            importSummary.TotalFailed = failedImportsCount;
            importSummary.CompletedAt = _dateTime.UTCNow.ToLocalTime();


            _uow.Products.AddRange(products);

            _uow.Inventories.AddRange(inventories);

            await _uow.Complete(cancellationToken);

            await _signalService.SendOnProgress(request.UserId, request.JobId, 100.0m, "Import is completed.", cancellationToken);

            await _signalService.SendOnImportCompleted(request.UserId, importSummary, cancellationToken);


        }
    }
}
