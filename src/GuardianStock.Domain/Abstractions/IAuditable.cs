namespace GuardianStock.Domain.Abstractions
{
    public interface IAuditable
    {

        public DateTime CreatedOnUTC { get; }
        public Guid? CreatedBy { get; }
        public DateTime UpdatedOnUTC { get; }
        public Guid? UpdatedBy { get; }

    }
}
