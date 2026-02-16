using IMS.Application.Auth.ForgetPassword;
using IMS.Application.LowStockAlerts.Commands.SendLowStockEmail;
using IMS.Application.Products.Commands.BulkImportProducts;
using IMS.Application.Products.Commands.GeneratePreview;
using IMS.Application.Users.Commands.SendUserCreatedEmail;

namespace IMS.Application.Common.Interfaces
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
