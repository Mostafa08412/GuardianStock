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


    internal class SignalService
    {
        private readonly IHubContext<ImportHub> hubContext;
        async Task SendOnPreviewReadySignal(string jobId, CancellationToken cancellationToken)
        {
            await hubContext.Clients.All.SendAsync("OnPreviewReady", jobId, cancellationToken);

        }
    }

}
