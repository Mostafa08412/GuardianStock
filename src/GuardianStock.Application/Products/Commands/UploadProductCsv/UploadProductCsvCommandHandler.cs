using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Application.Products.Commands.GeneratePreview;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GuardianStock.Application.Products.Commands.UploadProductCsv
{
    public class UploadProductCsvCommandHandler : IRequestHandler<UploadProductCsvCommand, Result<UploadProductCsvResponse>>
    {
        private readonly IFileManagerService _fileManager;
        private readonly IImportService _signalService;
        private readonly IBackgroundJobWorker _backgroundJobWorker;
        private readonly ILogger<UploadProductCsvCommandHandler> _logger;
        private readonly ICurrentUser _user;
        public UploadProductCsvCommandHandler(
             ICurrentUser user,
        IFileManagerService fileManager,
            IImportService signalService,
            IBackgroundJobWorker backgroundJobWorker,
            ILogger<UploadProductCsvCommandHandler> logger)
        {
            _user = user;
            _fileManager = fileManager;
            _signalService = signalService;
            _backgroundJobWorker = backgroundJobWorker;
            _logger = logger;
        }

        public async Task<Result<UploadProductCsvResponse>> Handle(UploadProductCsvCommand request, CancellationToken cancellationToken)
        {
            var jobId = request.jobId;
            var userId = _user.UserId?.ToString() ?? string.Empty;

            await _signalService.SendOnProgress(userId, jobId, 0, "Uploading file...", cancellationToken);

            var result = await _fileManager.UploadFileAsync(request.File, "temp/preview", jobId, cancellationToken);

            await _signalService.SendOnProgress(userId, jobId, 20, "File uploaded...", cancellationToken);


            var command = new GeneratePreviewCommand
            {
                jobId = jobId,
                filePath = result,
                userId = userId
            };


            _backgroundJobWorker.EnqueueGeneratePreviewJob(command);

            return Result<UploadProductCsvResponse>.Success(new UploadProductCsvResponse { jobId = jobId });


        }
    }
}
