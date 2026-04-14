namespace CavendishACMISPortal.Models
{
    public class Module
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string ModuleCode { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public Course? Course { get; set; }
    }
}