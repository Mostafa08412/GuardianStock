using GuardianStock.Application.Auth.ForgetPassword;
using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Application.LowStockAlerts.Commands.SendLowStockEmail;
using GuardianStock.Application.Products.Commands.BulkImportProducts;
using GuardianStock.Application.Products.Commands.GeneratePreview;
using GuardianStock.Application.Users.Commands.SendUserCreatedEmail;
using Hangfire;

namespace GuardianStock.Infrastructure.Persistence.BackgroundJobs
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


        public void SendForgetPasswordEmail(SendForgetPasswordEmail request)
        {
            BackgroundJob.Enqueue<BackgroundJobBridge>(bridge =>
                bridge.SendRequestAsync(request));
        }
    }
}
