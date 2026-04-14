namespace CavendishACMISPortal.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Student";
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Programme { get; set; } = string.Empty;
        public decimal AccountBalance { get; set; }

        // ✅ New: Profile Picture
        public string? ProfilePicture { get; set; }   // e.g. "1800722717.jpg"
    }
}