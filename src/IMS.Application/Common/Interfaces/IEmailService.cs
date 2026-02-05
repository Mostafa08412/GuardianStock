namespace IMS.Application.Common.Interfaces
{
    public interface IEmailService
    {

        public Task SendEmailAsync(string to, string subject, string body);


        Task SendLowStockEmailAsync(
IEnumerable<string> to,
string productName,
string sku,
int currentQuantity,
int threshold,
CancellationToken cancellationToken);
    }
}
