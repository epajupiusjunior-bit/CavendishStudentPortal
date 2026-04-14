namespace CavendishACMISPortal.Models
{
    public class StudentModuleRegistration
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ModuleId { get; set; }
        public string Status { get; set; } = "Registered"; // Registered / Completed
        public User? User { get; set; }
        public Module? Module { get; set; }
    }
}