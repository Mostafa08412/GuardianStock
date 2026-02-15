using IMS.Application.Common.Errors;
using IMS.Application.Common.Interfaces;
using IMS.Application.Contracts.CsvFileReader;
using IMS.Application.Products.Commands.GeneratePreview.Dtos;
using IMS.Domain.Abstractions;
using IMS.Domain.Inventories;
using IMS.Domain.Products;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using static IMS.Domain.Core.Errors.Errors;

namespace IMS.Application.Products.Commands.GeneratePreview
{
    public class GeneratePreviewCommandHandler : IRequestHandler<GeneratePreviewCommand>
    {
        private readonly IProductCsvReader productCsvReader;
        private readonly IUnitOfWork unitOfWork;
        private readonly ISkuGenerator skuGenerator;
        private readonly HybridCache hybridCache;
        private readonly IImportService signalService;


        public GeneratePreviewCommandHandler(IProductCsvReader productCsvReader, IUnitOfWork unitOfWork, ISkuGenerator skuGenerator, HybridCache hybridCache, IImportService signalService)
        {
            this.productCsvReader = productCsvReader;
            this.unitOfWork = unitOfWork;
            this.skuGenerator = skuGenerator;
            this.hybridCache = hybridCache;
            this.signalService = signalService;
        }

        public async Task Handle(GeneratePreviewCommand request, CancellationToken cancellationToken)
        {

            await signalService.SendOnProgress(request.userId, request.jobId, 21, "Reading CSV...", cancellationToken);


            var productCsvModelListResult = await productCsvReader.ReadProductCSVFile(request.filePath);


            if (productCsvModelListResult.IsFailure)
            {
                await signalService.OnJobFailed(request.userId, request.jobId, productCsvModelListResult.Error.Description, cancellationToken);

                throw new Exception(productCsvModelListResult.Error.Description);

            }


            await signalService.SendOnProgress(request.userId, request.jobId, 22, "Validating CSV...", cancellationToken);


            var previewResponse = new GeneratePreviewResponse();

            previewResponse.JobId = request.jobId;

            var discoveredProduct = new HashSet<(string, string)>();

            IEnumerable<(Guid Id, string Name)> categories = await unitOfWork.Categories.GetAllCategoriesNamesWithIdsAsync(cancellationToken);

            var categoriesDict = categories.ToDictionary(x => x.Name.ToLower(), x => x.Id);

            decimal totalProgress = 0.22m;

            decimal progressUnit = (0.8m) / productCsvModelListResult.Value.Count();


            for (int i = 0; i < productCsvModelListResult.Value!.Count; i++)
            {
                await signalService.SendOnProgress
                    (request.userId,
                    request.jobId,
                    totalProgress * 100,
                    $"Validating {i + 1} from {productCsvModelListResult.Value.Count()}  ...",
                    cancellationToken);

                var productCsvModel = productCsvModelListResult.Value[i];

                int rowNumber = i + 2;

                var validationErrors = ValidateDomainRules(productCsvModel);

                bool isProductDiscovered =
                    discoveredProduct.Contains((productCsvModel.Name.ToLower(), productCsvModel.Category.ToLower()));

                if (isProductDiscovered)
                {
                    previewResponse.RowResults.Add(PreviewRowResult.CreateFailure(productCsvModel, rowNumber, ApplicationErrors.CsvReader.Product.DuplicateProduct.Description));
                    continue;
                }


                bool enteredCategoryNameIsExisting =
                    categoriesDict.TryGetValue(productCsvModel.Category.ToLower(), out Guid categoryId);


                if (!enteredCategoryNameIsExisting)
                    validationErrors.Add(ApplicationErrors.CsvReader.Product.InvalidCategory.Description);



                if (validationErrors.Any())
                {
                    previewResponse.RowResults.Add(PreviewRowResult.CreateFailure(productCsvModel, rowNumber, validationErrors));
                    continue;
                }




                // 3. Domain Object Creation to double validation check 
                var createResult = Product.Create(productCsvModel.Name, skuGenerator.GenerateSKU(productCsvModel.Supplier), productCsvModel.Description, Decimal.Parse(productCsvModel.Price!), productCsvModel.Supplier, categoryId);

                if (createResult.IsFailure)
                {
                    previewResponse.RowResults.Add(PreviewRowResult.CreateFailure(productCsvModel, rowNumber, createResult.Error!.Description));
                }
                else
                {
                    discoveredProduct.Add((productCsvModel.Name.ToLower(), productCsvModel.Category.ToLower()));
                    previewResponse.RowResults.Add(PreviewRowResult.CreateSuccess(productCsvModel, rowNumber, categoryId));
                }
                totalProgress = totalProgress + progressUnit;

                await signalService.SendOnProgress(request.userId, request.jobId, totalProgress * 100, $"Validating {i + 1} from {productCsvModelListResult.Value.Count()}  completed.", cancellationToken);

            }

            await signalService.SendOnProgress(request.userId, request.jobId, 100.0m, "Preview is ready", cancellationToken);

            await hybridCache.SetAsync(request.jobId, previewResponse);

            await signalService.SendOnPreviewReadySignal(request.userId, previewResponse, cancellationToken);

        }
        private List<string> ValidateDomainRules(ProductCSVModel p)
        {
            var errors = new List<string>();

            // 1. Name Validation
            if (string.IsNullOrWhiteSpace(p.Name))
                errors.Add(ProductErrors.NameIsRequired.Description);

            // 2. Description Validation
            if (string.IsNullOrWhiteSpace(p.Description))
                errors.Add(ProductErrors.DescriptionIsRequired.Description);

            // 3. Supplier Validation
            if (string.IsNullOrWhiteSpace(p.Supplier))
                errors.Add(ProductErrors.SupplierIsRequired.Description);

            // 4. Price Validation (Parsing + Domain Rule)
            if (!decimal.TryParse(p.Price, out decimal pPrice))
            {
                errors.Add(ApplicationErrors.CsvReader.Product.InvalidPrice.Description);
            }
            else if (pPrice <= 0)
            {
                errors.Add(ProductErrors.InvalidPrice.Description);
            }

            // 5. Initial Quantity Validation (Parsing)
            if (!int.TryParse(p.InitialQuantity, out _))
            {
                errors.Add(ApplicationErrors.CsvReader.Product.InvalidInitialStock.Description);
            }

            // 6. Low Stock Threshold Validation (Parsing + Business Rule)
            if (!int.TryParse(p.LowStockAlertThreshold, out int pLowStockThreshold))
            {
                errors.Add(ApplicationErrors.CsvReader.Product.InvalidLowStockFormat.Description);
            }
            else if (pLowStockThreshold < Inventory.MinimumLowStockThreshold)
            {
                errors.Add(ApplicationErrors.CsvReader.Product.LowStockTooLow(Inventory.MinimumLowStockThreshold).Description);
            }

            return errors;
        }
    }
}
