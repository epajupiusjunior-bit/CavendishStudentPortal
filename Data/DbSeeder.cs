using CavendishACMISPortal.Models;

namespace CavendishACMISPortal.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Users.Any()) return;

            // Admin
            context.Users.Add(new User { Username = "admin", Password = "Admin256!", FullName = "Dr. Sarah Nakato", Role = "Admin", Email = "admin@cavendish.ac.ug" });

            // Lecturer
            context.Users.Add(new User { Username = "LEC001", Password = "Lec256!", FullName = "Dr. John Mukisa", Role = "Lecturer", Email = "john.mukisa@cavendish.ac.ug", Department = "Fine Arts" });

            // Student
            context.Users.Add(new User { Username = "1800722717", Password = "Pius256!", FullName = "PIUS Epaju Junior", Role = "Student", Email = "400ericpaul@gmail.com", Phone = "0752591064", Programme = "BACHELOR OF INDUSTRIAL AND FINE ARTS - (BIFA)" });

            context.SaveChanges();
        }
    }
}