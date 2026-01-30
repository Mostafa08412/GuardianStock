using IMS.Application.Common.Interfaces;
using IMS.Domain.Products;

namespace IMS.Infrastructure.Common
{
    public class SkuGenerator : ISkuGenerator
    {
        private readonly IDateTime _dateTime;

        public SkuGenerator(IDateTime dateTime)
        {
            _dateTime = dateTime;
        }

        public string GenerateSKU(string supplierName)
        {
            if (supplierName.Length < 3)
                supplierName = supplierName.PadRight(3, 'X').ToUpper();

            var firstPart = supplierName.Substring(0, 3).ToUpper();

            var secondPart = _dateTime.UTCNow.ToString("yyMMdd");

            string guidString = Guid.NewGuid().ToString("N");

            var thirdPart = guidString.Substring(guidString.Length - 6).ToUpper();

            return $"{firstPart}-{secondPart}-{thirdPart}";

        }
    }
}
