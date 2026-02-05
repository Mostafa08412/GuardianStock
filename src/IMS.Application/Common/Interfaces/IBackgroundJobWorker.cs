using IMS.Application.LowStockAlerts.Commands.SendLowStockEmail;

namespace IMS.Application.Common.Interfaces
{
    public interface IBackgroundJobWorker
    {

        void EnqueueSendLowStockEmailJob(SendLowStockEmailCommand request);


    }
}
