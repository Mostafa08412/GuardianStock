using IMS.Application.Common.Interfaces;
using IMS.Application.Products.Commands.GeneratePreview;
using IMS.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Products.Commands.UploadProductCsv
{
    public class UploadProductCsvCommandHandler : IRequestHandler<UploadProductCsvCommand, Result<UploadProductCsvResponse>>
    {
        private readonly IFileManagerService _fileManager;
        private readonly ISignalService _signalService;
        private readonly IBackgroundJobWorker _backgroundJobWorker;
        private readonly ILogger<UploadProductCsvCommandHandler> _logger;
        private readonly ICurrentUser _user;
        public UploadProductCsvCommandHandler(
             ICurrentUser user,
        IFileManagerService fileManager,
            ISignalService signalService,
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

            await _signalService.SendOnProgress(_user.UserId, jobId, 0, "Uploading file...", cancellationToken);

            var result = await _fileManager.UploadFileAsync(request.File, "temp/preview", jobId, cancellationToken);

            await _signalService.SendOnProgress(_user.UserId, jobId, 20, "File uploaded...", cancellationToken);


            var command = new GeneratePreviewCommand
            {
                jobId = jobId,
                filePath = result,
                userId = _user.UserId
            };


            _backgroundJobWorker.EnqueueGeneratePreviewJob(command);

            return Result<UploadProductCsvResponse>.Success(new UploadProductCsvResponse { jobId = jobId });


        }
    }
}
