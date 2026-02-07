using IMS.Application.Common.Interfaces;

namespace IMS.Domain.Abstractions
{
    public class SettableDateProvider : IDateTime
    {
        public DateTime UTCNow { get; set; } = DateTime.UtcNow;

    }
}
