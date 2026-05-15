namespace CavendishACMISPortal.Models
{
    public class LecturerModuleAssignment
    {
        public int Id { get; set; }
        public int LecturerId { get; set; }
        public int ModuleId { get; set; }
        public User? Lecturer { get; set; }
        public Module? Module { get; set; }
    }
}