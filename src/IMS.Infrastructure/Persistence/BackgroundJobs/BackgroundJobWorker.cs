using Hangfire;
using IMS.Application.Common.Interfaces;
using IMS.Application.LowStockAlerts.Commands.SendLowStockEmail;
using IMS.Application.Products.Commands.GeneratePreview;

namespace IMS.Infrastructure.Persistence.BackgroundJobs
{

    internal class BackgroundJobWorker : IBackgroundJobWorker
    {

        public void EnqueueSendLowStockEmailJob(SendLowStockEmailCommand request)
        {
            var jobId = BackgroundJob.Schedule<BackgroundJobBridge>(bridge => bridge.SendRequestAsync(request), TimeSpan.FromSeconds(3));

        }

        public void EnqueueGeneratePreviewJob(GeneratePreviewCommand request)
        {
            var jobId = BackgroundJob.Schedule<BackgroundJobBridge>(bridge => bridge.SendRequestAsync(request), TimeSpan.FromSeconds(3));

        }
    }
}
