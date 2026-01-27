using IMS.Application.Common.Interfaces;

namespace IMS.Domain.Abstractions
{
    public class DateProvider : IDateTime
    {
        public DateTime UTCNow => DateTime.UtcNow;

    }
}
