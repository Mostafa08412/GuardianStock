using IMS.Application.Products.Commands.GeneratePreview.Dtos;

namespace IMS.Application.Common.Interfaces
{
    public interface ISignalService
    {
        Task SendOnPreviewReadySignal(string jobId, ImportProductsReport report, CancellationToken cancellationToken);
        Task SendOnProgress(string jobId, decimal percentage, string message, CancellationToken cancellationToken);
        Task OnJobFailed(string jobId, string message, CancellationToken cancellationToken);

    }
}
