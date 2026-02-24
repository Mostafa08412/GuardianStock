using GuardianStock.Application.Auth.ForgetPassword;
using GuardianStock.Application.LowStockAlerts.Commands.SendLowStockEmail;
using GuardianStock.Application.Products.Commands.BulkImportProducts;
using GuardianStock.Application.Products.Commands.GeneratePreview;
using GuardianStock.Application.Users.Commands.SendUserCreatedEmail;

namespace GuardianStock.Application.Common.Interfaces
{
    public interface IBackgroundJobWorker
    {

        void EnqueueSendLowStockEmailJob(SendLowStockEmailCommand request);
        void EnqueueGeneratePreviewJob(GeneratePreviewCommand request);

        void EnqueueImportProductsFromPreviewJob(BulkImportProductsJob request);

        void EnqueueSendUserCreatedEmailJob(SendUserCreatedEmailCommand request);

        void SendForgetPasswordEmail(SendForgetPasswordEmail request);


    }
}
