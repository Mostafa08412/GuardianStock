using IMS.Application.Common.Interfaces;
using IMS.Application.Products.Commands.BulkImportProducts;
using IMS.Application.Products.Commands.GeneratePreview.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace IMS.Infrastructure.HubServices
{
    public class ImportHub : Hub
    {
        private readonly ISignalService ss;

        public ImportHub(ISignalService ss)
        {
            this.ss = ss;
        }


    }


    internal class SignalService : ISignalService
    {
        private readonly IHubContext<ImportHub> hubContext;

        public SignalService(IHubContext<ImportHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task OnJobFailed(string userId, string jobId, string message, CancellationToken cancellationToken)
        {
            await hubContext.Clients.User(userId).SendAsync("OnJobFailed", new { jobId, message }, cancellationToken);

        }

        public async Task SendOnPreviewReadySignal(string userId, GeneratePreviewResponse report, CancellationToken cancellationToken)
        {
            await hubContext.Clients.User(userId).SendAsync("OnPreviewReady", report, cancellationToken);

        }

        public async Task SendOnProgress(string userId, string jobId, decimal percentage, string message, CancellationToken cancellationToken)
        {
            await hubContext.Clients.User(userId).SendAsync("OnProgress", new { jobId, percentage, message }, cancellationToken);

        }

        public async Task SendOnImportCompleted(string userId, ImportSummary summary, CancellationToken cancellationToken)
        {
            await hubContext.Clients.User(userId).SendAsync("OnImportCompleted", summary, cancellationToken);

        }

    }

}
