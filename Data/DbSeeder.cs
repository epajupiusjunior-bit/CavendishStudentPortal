using CavendishACMISPortal.Models;

namespace CavendishACMISPortal.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Users.Any()) return;

            // Admin
            context.Users.Add(new User
            {
                Username = "admin",
                Password = "Admin256!",
                FullName = "Dr. Sarah Nakato",
                Role = "Admin",
                Email = "admin@cavendish.ac.ug"
            });

            // Student
            var student = new User
            {
                Username = "1800722717",
                Password = "Pius256!",
                FullName = "PIUS Epaju Junior",
                Role = "Student",
                Email = "400ericpaul@gmail.com",
                Phone = "0752591064",
                District = "KABERAMAIDO",
                Programme = "BACHELOR OF INDUSTRIAL AND FINE ARTS - (BIFA)",
                AccountBalance = 110000
            };
            context.Users.Add(student);
            context.SaveChanges();

            var course = new Course { CourseCode = "BIFA", CourseName = "Bachelor of Industrial and Fine Arts" };
            context.Courses.Add(course);
            context.SaveChanges();

            // ✅ Fixed ambiguous reference by using full name
            context.Modules.AddRange(
                new Module { CourseId = course.Id, ModuleCode = "ART101", ModuleName = "Introduction to Fine Arts", Credits = 3 },
                new Module { CourseId = course.Id, ModuleCode = "IND102", ModuleName = "Industrial Design", Credits = 4 }
            );
            context.SaveChanges();
        }
    }
}