namespace IMS.Application.Common.Interfaces
{
    public interface ISignalService
    {
        Task SendOnPreviewReadySignal(string jobId, CancellationToken cancellationToken);

    }
}
