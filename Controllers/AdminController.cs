using Microsoft.AspNetCore.Mvc;
using CavendishACMISPortal.Data;
using CavendishACMISPortal.Models;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace CavendishACMISPortal.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context) => _context = context;

        // ==================== DASHBOARD ====================
        public IActionResult Dashboard()
        {
            ViewBag.TotalStudents = _context.Users.Count(u => u.Role == "Student");
            ViewBag.TotalLecturers = _context.Users.Count(u => u.Role == "Lecturer");
            ViewBag.TotalCourses = _context.Courses.Count();
            ViewBag.TotalModules = _context.Modules.Count();

            ViewBag.BestStudents = _context.Results
                .Include(r => r.User)
                .ToList()
                .GroupBy(r => r.UserId)
                .Select(g => new { Student = g.First().User, AvgScore = g.Average(r => (double)r.Total) })
                .OrderByDescending(s => s.AvgScore)
                .Take(5)
                .ToList();

            return View();
        }

        // ==================== STUDENTS CRUD ====================
        public IActionResult Students()
        {
            var students = _context.Users.Where(u => u.Role == "Student").ToList();
            return View(students);
        }

        public IActionResult AddStudent()
        {
            ViewBag.Courses = _context.Courses.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent(User student, IFormFile? profilePicture)
        {
            if (profilePicture != null && profilePicture.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = student.Username + Path.GetExtension(profilePicture.FileName).ToLower();
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = profilePicture.OpenReadStream();
                using var image = await Image.LoadAsync(stream);

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(300, 300),
                    Mode = ResizeMode.Max
                }));

                await image.SaveAsJpegAsync(filePath);
                student.ProfilePicture = fileName;
            }

            student.Role = "Student";
            _context.Users.Add(student);
            await _context.SaveChangesAsync();

            return RedirectToAction("Students");
        }

        public IActionResult EditStudent(int id)
        {
            var student = _context.Users.Find(id);
            return View(student);
        }

        [HttpPost]
        public IActionResult EditStudent(User student)
        {
            var existing = _context.Users.Find(student.Id);
            if (existing != null)
            {
                existing.FullName = student.FullName;
                existing.Email = student.Email;
                existing.Phone = student.Phone;
                existing.District = student.District;
                existing.Programme = student.Programme;
                _context.SaveChanges();
            }
            return RedirectToAction("Students");
        }

        public IActionResult DeleteStudent(int id)
        {
            var student = _context.Users.Find(id);
            if (student != null)
            {
                _context.Users.Remove(student);
                _context.SaveChanges();
            }
            return RedirectToAction("Students");
        }

        // ==================== LECTURERS CRUD ====================
        public IActionResult Lecturers()
        {
            var lecturers = _context.Users.Where(u => u.Role == "Lecturer").ToList();
            return View(lecturers);
        }

        public IActionResult AddLecturer() => View();

        [HttpPost]
        public IActionResult AddLecturer(User lecturer)
        {
            lecturer.Role = "Lecturer";
            _context.Users.Add(lecturer);
            _context.SaveChanges();
            return RedirectToAction("Lecturers");
        }

        public IActionResult EditLecturer(int id)
        {
            var lecturer = _context.Users.Find(id);
            return View(lecturer);
        }

        [HttpPost]
        public IActionResult EditLecturer(User lecturer)
        {
            var existing = _context.Users.Find(lecturer.Id);
            if (existing != null)
            {
                existing.FullName = lecturer.FullName;
                existing.Email = lecturer.Email;
                existing.Phone = lecturer.Phone;
                existing.Department = lecturer.Department;
                _context.SaveChanges();
            }
            return RedirectToAction("Lecturers");
        }

        public IActionResult DeleteLecturer(int id)
        {
            var lecturer = _context.Users.Find(id);
            if (lecturer != null)
            {
                _context.Users.Remove(lecturer);
                _context.SaveChanges();
            }
            return RedirectToAction("Lecturers");
        }
        // ==================== ASSIGN MODULES TO LECTURER ====================
        public IActionResult AssignModules(int lecturerId)
        {
            var lecturer = _context.Users.Find(lecturerId);
            if (lecturer == null) return RedirectToAction("Lecturers");

            var allModules = _context.Modules.Include(m => m.Course).ToList();
            var assignedIds = _context.LecturerAssignments
                .Where(a => a.LecturerId == lecturerId)
                .Select(a => a.ModuleId)
                .ToList();

            ViewBag.Lecturer = lecturer;
            ViewBag.AllModules = allModules;
            ViewBag.AssignedModuleIds = assignedIds;

            return View();
        }

        [HttpPost]
        public IActionResult AssignModules(int lecturerId, int[] moduleIds)
        {
            // Remove old assignments
            var old = _context.LecturerAssignments.Where(a => a.LecturerId == lecturerId);
            _context.LecturerAssignments.RemoveRange(old);

            // Add new ones
            foreach (var mid in moduleIds)
            {
                _context.LecturerAssignments.Add(new LecturerModuleAssignment
                {
                    LecturerId = lecturerId,
                    ModuleId = mid
                });
            }

            _context.SaveChanges();
            return RedirectToAction("Lecturers");
        }
        // ==================== COURSES CRUD ====================
        public IActionResult Courses() => View(_context.Courses.ToList());

        public IActionResult AddCourse() => View();

        [HttpPost]
        public IActionResult AddCourse(Course model)
        {
            _context.Courses.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Courses");
        }

        public IActionResult EditCourse(int id) => View(_context.Courses.Find(id));

        [HttpPost]
        public IActionResult EditCourse(Course model)
        {
            _context.Update(model);
            _context.SaveChanges();
            return RedirectToAction("Courses");
        }

        public IActionResult DeleteCourse(int id)
        {
            var course = _context.Courses.Find(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
            }
            return RedirectToAction("Courses");
        }

        // ==================== MODULES CRUD ====================
        public IActionResult Modules() => View(_context.Modules.Include(m => m.Course).ToList());

        public IActionResult AddModule()
        {
            ViewBag.Courses = _context.Courses.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult AddModule(Module model)
        {
            _context.Modules.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Modules");
        }

        public IActionResult EditModule(int id)
        {
            ViewBag.Courses = _context.Courses.ToList();
            return View(_context.Modules.Include(m => m.Course).FirstOrDefault(m => m.Id == id));
        }

        [HttpPost]
        public IActionResult EditModule(Module model)
        {
            _context.Update(model);
            _context.SaveChanges();
            return RedirectToAction("Modules");
        }

        public IActionResult DeleteModule(int id)
        {
            var module = _context.Modules.Find(id);
            if (module != null)
            {
                _context.Modules.Remove(module);
                _context.SaveChanges();
            }
            return RedirectToAction("Modules");
        }

        // ==================== RESULTS CRUD ====================
        public IActionResult Results()
        {
            var results = _context.Results
                .Include(r => r.User)
                .Include(r => r.Module)
                .ToList();
            return View(results);
        }

        public IActionResult EnterResult()
        {
            ViewBag.Students = _context.Users.Where(u => u.Role == "Student").ToList();
            ViewBag.Modules = _context.Modules.ToList();
            return View(new Result());
        }

        [HttpPost]
        public IActionResult EnterResult(Result model)
        {
            _context.Results.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Results");
        }

        public IActionResult EditResult(int id)
        {
            ViewBag.Students = _context.Users.Where(u => u.Role == "Student").ToList();
            ViewBag.Modules = _context.Modules.ToList();
            return View(_context.Results.Find(id));
        }

        [HttpPost]
        public IActionResult EditResult(Result model)
        {
            _context.Update(model);
            _context.SaveChanges();
            return RedirectToAction("Results");
        }

        public IActionResult DeleteResult(int id)
        {
            var result = _context.Results.Find(id);
            if (result != null)
            {
                _context.Results.Remove(result);
                _context.SaveChanges();
            }
            return RedirectToAction("Results");
        }
    }
}