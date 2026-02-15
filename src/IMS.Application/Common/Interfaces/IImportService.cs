using IMS.Application.Products.Commands.BulkImportProducts;
using IMS.Application.Products.Commands.GeneratePreview.Dtos;

namespace IMS.Application.Common.Interfaces
{
    public interface IImportService
    {
        Task SendOnPreviewReadySignal(string userId, GeneratePreviewResponse report, CancellationToken cancellationToken);
        Task SendOnProgress(string userId, string jobId, decimal percentage, string message, CancellationToken cancellationToken);
        Task OnJobFailed(string userId, string jobId, string message, CancellationToken cancellationToken);

        Task SendOnImportCompleted(string userId, ImportSummary summary, CancellationToken cancellationToken);


    }
}
