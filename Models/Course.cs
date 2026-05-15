namespace CavendishACMISPortal.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public ICollection<Module> Modules { get; set; } = new List<Module>();
    }
}