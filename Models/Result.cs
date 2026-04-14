namespace CavendishACMISPortal.Models
{
    public class Result
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ModuleId { get; set; }
        public decimal Score { get; set; }
        public string Grade { get; set; } = string.Empty;
        public User? User { get; set; }
        public Module? Module { get; set; }
    }
}