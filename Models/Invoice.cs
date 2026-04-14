namespace CavendishACMISPortal.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Paid { get; set; }
        public decimal Due { get; set; }
        public string Status { get; set; } = "Pending";
        public User? User { get; set; }
    }
}