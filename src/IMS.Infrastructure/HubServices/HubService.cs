using IMS.Application.Common.Interfaces;
using IMS.Application.Products.Commands.GeneratePreview.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace IMS.Infrastructure.HubServices
{
    public class ImportHub : Hub
    {

        async Task SendSignalAsyncWithMessage(string channel, string message, CancellationToken cancellationToken)
        {
            await Clients.All.SendAsync(channel, message, cancellationToken);
        }

    }


    internal class SignalService : ISignalService
    {
        private readonly IHubContext<ImportHub> hubContext;

        public SignalService(IHubContext<ImportHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task OnJobFailed(string jobId, string message, CancellationToken cancellationToken)
        {
            await hubContext.Clients.All.SendAsync("OnJobFailed", new { jobId, message }, cancellationToken);

        }

        public async Task SendOnPreviewReadySignal(string jobId, ImportProductsReport report, CancellationToken cancellationToken)
        {
            await hubContext.Clients.All.SendAsync("OnPreviewReady", new { jobId, report }, cancellationToken);

        }

        public async Task SendOnProgress(string jobId, decimal percentage, string message, CancellationToken cancellationToken)
        {
            await hubContext.Clients.All.SendAsync("OnProgress", new { jobId, percentage, message }, cancellationToken);

        }



        async Task SendOnPreviewReadySignal(string jobId, CancellationToken cancellationToken)
        {
            await hubContext.Clients.All.SendAsync("OnPreviewReady", new { jobId }, cancellationToken);

        }
    }

}
