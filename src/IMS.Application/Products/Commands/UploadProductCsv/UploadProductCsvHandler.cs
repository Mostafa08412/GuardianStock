using IMS.Application.Common.Interfaces;
using IMS.Application.Products.Commands.GeneratePreview;
using IMS.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Products.Commands.UploadProductCsv
{
    public class UploadProductCsvHandler : IRequestHandler<UploadProductCsvCommand, Result<UploadProductCsvResult>>
    {
        private readonly IFileManagerService _fileManager;
        private readonly ISignalService _signalService;
        private readonly IBackgroundJobWorker _backgroundJobWorker;
        private readonly ILogger<UploadProductCsvHandler> _logger;

        public UploadProductCsvHandler(
            IFileManagerService fileManager,
            ISignalService signalService,
            IBackgroundJobWorker backgroundJobWorker,
            ILogger<UploadProductCsvHandler> logger)
        {
            _fileManager = fileManager;
            _signalService = signalService;
            _backgroundJobWorker = backgroundJobWorker;
            _logger = logger;
        }

        public async Task<Result<UploadProductCsvResult>> Handle(UploadProductCsvCommand request, CancellationToken cancellationToken)
        {
            var previewId = Guid.NewGuid().ToString();

            await _signalService.SendOnProgress(previewId, 0, "Uploading file...", cancellationToken);

            var result = await _fileManager.UploadFileAsync(request.File, "temp/preview", previewId, cancellationToken);

            await _signalService.SendOnProgress(previewId, 20, "File uploaded...", cancellationToken);


            var command = new GeneratePreviewCommand
            {
                previewId = previewId,
                filePath = result
            };


            _backgroundJobWorker.EnqueueGeneratePreviewJob(command);

            return Result<UploadProductCsvResult>.Success(new UploadProductCsvResult { jobId = previewId });


        }
    }
}
