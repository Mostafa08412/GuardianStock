namespace GuardianStock.Application.Dashboard.Queries
{
    public class RecentTransactionDto
    {
        public string Id { get; set; }
        public string ProductName { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public string UserName { get; set; }
    }
}
