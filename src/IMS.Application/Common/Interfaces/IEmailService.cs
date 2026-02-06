using IMS.Domain.Core.Primitives.Result;

namespace IMS.Application.Common.Interfaces
{
    public interface IEmailService
    {

        public Task SendEmailAsync(string to, string subject, string body);


        Task<Result> SendLowStockEmailAsync(
        IEnumerable<string> to,
        string productName,
        string sku,
        int currentQuantity,
        int threshold,
        CancellationToken cancellationToken);
    }
}
