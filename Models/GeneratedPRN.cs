namespace CavendishACMISPortal.Models
{
    public class GeneratedPRN
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PRNNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public DateTime GeneratedDate { get; set; } = DateTime.Now;
        public User? User { get; set; }
    }
}