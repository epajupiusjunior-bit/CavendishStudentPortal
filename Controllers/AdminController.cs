using CavendishACMISPortal.Data;
using CavendishACMISPortal.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace CavendishACMISPortal.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context) => _context = context;

        public IActionResult Dashboard()
        {
            ViewBag.TotalStudents = _context.Users.Count(u => u.Role == "Student");
            ViewBag.TotalCourses = _context.Courses.Count();
            ViewBag.BestStudents = _context.Results
                .Include(r => r.User)
                .GroupBy(r => r.UserId)
                .Select(g => new { Student = g.First().User, AvgScore = g.Average(r => (double)r.Score) })
                .OrderByDescending(s => s.AvgScore)
                .Take(5)
                .ToList();

            ViewBag.Defaulters = _context.Users
                .Where(u => u.Role == "Student" && u.AccountBalance > 50000)
                .OrderByDescending(u => (double)u.AccountBalance)
                .ToList();

            return View();
        }

        // ====================== REQUESTED ACTIONS ======================

        public IActionResult Courses()
        {
            var courses = _context.Courses
                .Include(c => c.Modules)
                .ToList();
            return View(courses);
        }

        public IActionResult Modules()
        {
            var modules = _context.Modules
                .Include(m => m.Course)
                .ToList();
            return View(modules);
        }

        public IActionResult Students()
        {
            var students = _context.Users
                .Where(u => u.Role == "Student")
                .ToList();
            return View(students);
        }

        public IActionResult Results()
        {
            var results = _context.Results
                .Include(r => r.User)
                .Include(r => r.Module)
                .ToList();
            return View(results);
        }

        // ====================== EXISTING FEATURES (unchanged) ======================

        // NEW: Full Module CRUD
        public IActionResult AddModule() => View();
        [HttpPost]
        public IActionResult AddModule(Module model)
        {
            _context.Modules.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Modules");
        }

        public IActionResult EditModule(int id) => View(_context.Modules.Include(m => m.Course).FirstOrDefault(m => m.Id == id));
        [HttpPost]
        public IActionResult EditModule(Module model)
        {
            _context.Update(model);
            _context.SaveChanges();
            return RedirectToAction("Modules");
        }

        // NEW: Course CRUD
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

        // NEW: Result Entry Form
        public IActionResult EnterResult() => View(new Result());
        [HttpPost]
        public IActionResult EnterResult(Result model)
        {
            _context.Results.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Results");
        }

        // NEW: Fee / Invoice Editing
        public IActionResult EditFees(int id)
        {
            var invoice = _context.Invoices.Find(id);
            return View(invoice);
        }
        [HttpPost]
        public IActionResult EditFees(Invoice invoice)
        {
            _context.Update(invoice);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        // NEW: Export to Excel
        public IActionResult ExportStudents()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Students");
            var students = _context.Users.Where(u => u.Role == "Student").ToList();
            worksheet.Cell(1, 1).Value = "Student No";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Programme";
            worksheet.Cell(1, 4).Value = "Balance";
            for (int i = 0; i < students.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = students[i].Username;
                worksheet.Cell(i + 2, 2).Value = students[i].FullName;
                worksheet.Cell(i + 2, 3).Value = students[i].Programme;
                worksheet.Cell(i + 2, 4).Value = students[i].AccountBalance;
            }
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students.xlsx");
        }

        [HttpGet]
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
                using var image = await SixLabors.ImageSharp.Image.LoadAsync(stream);

                var resizeOptions = new ResizeOptions
                {
                    Size = new SixLabors.ImageSharp.Size(300, 300),
                    Mode = ResizeMode.Max
                };
                image.Mutate(x => x.Resize(resizeOptions));

                await image.SaveAsJpegAsync(filePath);
                student.ProfilePicture = fileName;
            }

            student.Role = "Student";
            _context.Users.Add(student);
            await _context.SaveChangesAsync();

            return RedirectToAction("Students");
        }
    }
}