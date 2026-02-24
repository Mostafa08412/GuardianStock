namespace GuardianStock.Domain.Products
{
    public interface ISkuGenerator
    {
        string GenerateSKU(string supplierName);

    }
}
