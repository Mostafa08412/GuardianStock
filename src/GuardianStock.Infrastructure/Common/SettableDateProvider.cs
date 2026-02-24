using GuardianStock.Application.Common.Interfaces;

namespace GuardianStock.Infrastructure.Common
{
    public class SettableDateProvider : IDateTime
    {
        public DateTime UTCNow { get; set; } = DateTime.UtcNow;

    }
}
