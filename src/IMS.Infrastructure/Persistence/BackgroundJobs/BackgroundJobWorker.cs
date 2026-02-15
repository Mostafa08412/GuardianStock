using Hangfire;
using IMS.Application.Common.Interfaces;
using IMS.Application.LowStockAlerts.Commands.SendLowStockEmail;
using IMS.Application.Products.Commands.BulkImportProducts;
using IMS.Application.Products.Commands.GeneratePreview;
using IMS.Application.Users.Commands.SendUserCreatedEmail;

namespace IMS.Infrastructure.Persistence.BackgroundJobs
{

    internal class BackgroundJobWorker : IBackgroundJobWorker
    {

        public void EnqueueSendLowStockEmailJob(SendLowStockEmailCommand request)
        {
            BackgroundJob.Enqueue<BackgroundJobBridge>(bridge => bridge.SendRequestAsync(request));

        }


        public void EnqueueGeneratePreviewJob(GeneratePreviewCommand request)
        {
            BackgroundJob.Enqueue<BackgroundJobBridge>(bridge => bridge.SendRequestAsyncWithNoRetry(request));

        }

        public void EnqueueImportProductsFromPreviewJob(BulkImportProductsJob request)
        {
            BackgroundJob.Enqueue<BackgroundJobBridge>(bridge => bridge.SendRequestAsyncWithNoRetry(request));

        }

        public void EnqueueSendUserCreatedEmailJob(SendUserCreatedEmailCommand request)
        {
            BackgroundJob.Enqueue<BackgroundJobBridge>(bridge => bridge.SendRequestAsync(request));
        }
    }
}
