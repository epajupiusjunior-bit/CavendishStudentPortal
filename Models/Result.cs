namespace CavendishACMISPortal.Models
{
    public class Result
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ModuleId { get; set; }

        public decimal CAT1 { get; set; }      // 20 marks
        public decimal CAT2 { get; set; }      // 20 marks
        public decimal FinalExam { get; set; } // 60 marks

        public decimal Total => CAT1 + CAT2 + FinalExam;

        public string Grade => GetGrade(Total);

        public User? User { get; set; }
        public Module? Module { get; set; }

        private static string GetGrade(decimal score)
        {
            return score switch
            {
                >= 80 => "A",
                >= 75 => "B+",
                >= 70 => "B",
                >= 65 => "C+",
                >= 60 => "C",
                >= 55 => "D+",
                >= 50 => "D",
                >= 45 => "E",
                _ => "F"
            };
        }
    }
}