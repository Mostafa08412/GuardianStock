namespace IMS.Domain.Abstractions
{
    public interface IAuditable
    {

        public DateTime CreatedOnUTC { get; }
        public string CreatedBy { get; }
        public DateTime UpdatedOnUTC { get; }
        public string UpdatedBy { get; }

    }
}
