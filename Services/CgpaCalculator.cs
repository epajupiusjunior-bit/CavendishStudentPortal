using CavendishACMISPortal.Models;

namespace CavendishACMISPortal.Services
{
    public static class CgpaCalculator
    {
        // Ugandan 5.0 Scale
        public static decimal GetGradePoint(string grade)
        {
            return grade?.ToUpper() switch
            {
                "A" => 5.0m,
                "B+" => 4.5m,
                "B" => 4.0m,
                "C+" => 3.5m,
                "C" => 3.0m,
                "D+" => 2.5m,
                "D" => 2.0m,
                "E" => 1.5m,
                "F" => 0.0m,
                _ => 0.0m
            };
        }

        public static decimal CalculateCGPA(IEnumerable<Result> results)
        {
            if (results == null || !results.Any()) return 0;

            decimal totalPoints = 0;
            int totalCredits = 0;

            foreach (var r in results)
            {
                if (r.Module != null)
                {
                    totalPoints += GetGradePoint(r.Grade) * r.Module.Credits;
                    totalCredits += r.Module.Credits;
                }
            }

            return totalCredits == 0 ? 0 : Math.Round(totalPoints / totalCredits, 2);
        }
    }
}