namespace CavendishACMISPortal.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;     // Student No or Teacher ID
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Student";            // Admin, Lecturer, Student
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Programme { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }
        public string? Department { get; set; }                  // For Lecturers
    }
}