using Hangfire;
using IMS.Application.Common.Interfaces;
using IMS.Application.LowStockAlerts.Commands.SendLowStockEmail;
using IMS.Application.Products.Commands.BulkImportProducts;
using IMS.Application.Products.Commands.GeneratePreview;

namespace IMS.Infrastructure.Persistence.BackgroundJobs
{

    internal class BackgroundJobWorker : IBackgroundJobWorker
    {

        public void EnqueueSendLowStockEmailJob(SendLowStockEmailCommand request)
        {
            var jobId = BackgroundJob.Schedule<BackgroundJobBridge>(bridge => bridge.SendRequestAsync(request), TimeSpan.FromSeconds(1));

        }


        public void EnqueueGeneratePreviewJob(GeneratePreviewCommand request)
        {
            var jobId = BackgroundJob.Enqueue<BackgroundJobBridge>(bridge => bridge.SendRequestAsyncWithNoRetry(request));

        }

        public void EnqueueImportProductsFromPreviewJob(BulkImportProductsJob request)
        {
            var jobId = BackgroundJob.Enqueue<BackgroundJobBridge>(bridge => bridge.SendRequestAsyncWithNoRetry(request));

        }
    }
}
