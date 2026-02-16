using IMS.Domain.Core.Primitives.Result;

namespace IMS.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(
        string to,
        string subject,
        string body);

        Task SendForgetPasswordEmailAsync(
        string to,
        string name,
        string emailAddress,
        string otp,
        CancellationToken cancellationToken);

        Task<Result> SendLowStockEmailAsync(
        IEnumerable<string> to,
        string productName,
        string inventoryId,
        string sku,
        int currentQuantity,
        int threshold,
        CancellationToken cancellationToken);

        Task<Result> SendUserCreatedEmailAsync(
        string to,
        string fullName,
        string email,
        string password,
        CancellationToken cancellationToken);
    }
}
