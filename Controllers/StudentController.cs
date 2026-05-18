using Microsoft.AspNetCore.Mvc;
using CavendishACMISPortal.Data;
using CavendishACMISPortal.Models;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.IO;

namespace CavendishACMISPortal.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context) => _context = context;

        private int CurrentStudentId => int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

        public IActionResult Dashboard()
        {
            var student = _context.Users.Find(CurrentStudentId);

            // Enrolment Status
            var registrations = _context.Registrations
                .Where(r => r.UserId == CurrentStudentId)
                .ToList();

            bool isEnrolled = registrations.Any();
            int enrolledModulesCount = registrations.Count;

            // Get latest registered semester (you can improve this logic later)
            string currentSemester = "Not Registered";
            if (registrations.Any())
            {
                currentSemester = "Semester II • 2025/2026"; // You can store this in a separate table later
            }

            // CGPA
            var results = _context.Results
                .Include(r => r.Module)
                .Where(r => r.UserId == CurrentStudentId)
                .ToList();

            decimal cgpa = CavendishACMISPortal.Services.CgpaCalculator.CalculateCGPA(results);

            ViewBag.Student = student;
            ViewBag.IsEnrolled = isEnrolled;
            ViewBag.EnrolledModulesCount = enrolledModulesCount;
            ViewBag.CurrentSemester = currentSemester;
            ViewBag.CGPA = cgpa;

            return View();
        }

        public IActionResult BioData()
        {
            var student = _context.Users.Find(CurrentStudentId);
            ViewBag.Student = student;
            return View();
        }

        [HttpPost]
        public IActionResult UpdateBioData(string phone, string email)
        {
            var student = _context.Users.Find(CurrentStudentId);
            if (student != null)
            {
                student.Phone = phone;
                student.Email = email;
                _context.SaveChanges();
            }
            return RedirectToAction("BioData");
        }

        // ==================== ENROLMENT ====================
        public IActionResult Enrolment()
        {
            var student = _context.Users.Find(CurrentStudentId);
            var availableModules = _context.Modules.Include(m => m.Course).ToList();

            var failedModules = _context.Results
                .Include(r => r.Module)
                .Where(r => r.UserId == CurrentStudentId)
                .ToList()
                .Where(r => r.Grade == "F")
                .ToList();

            ViewBag.Student = student;
            ViewBag.AvailableModules = availableModules;
            ViewBag.FailedModules = failedModules;

            return View();
        }

        [HttpPost]
        public IActionResult RegisterSemester(string academicYear, string semester)
        {
            TempData["Success"] = $"Successfully registered for {semester} {academicYear}";
            return RedirectToAction("Enrolment");
        }

        [HttpPost]
        public IActionResult RegisterModules(int[] moduleIds)
        {
            foreach (var mid in moduleIds)
            {
                if (!_context.Registrations.Any(r => r.UserId == CurrentStudentId && r.ModuleId == mid))
                {
                    _context.Registrations.Add(new StudentModuleRegistration
                    {
                        UserId = CurrentStudentId,
                        ModuleId = mid,
                        Status = "Registered"
                    });
                }
            }
            _context.SaveChanges();
            TempData["Success"] = "Modules registered successfully!";
            return RedirectToAction("Enrolment");
        }

        [HttpPost]
        public IActionResult RegisterRetakes(int[] moduleIds)
        {
            foreach (var mid in moduleIds)
            {
                _context.Registrations.Add(new StudentModuleRegistration
                {
                    UserId = CurrentStudentId,
                    ModuleId = mid,
                    Status = "Retake"
                });
            }
            _context.SaveChanges();
            TempData["Success"] = "Retake registered successfully!";
            return RedirectToAction("Enrolment");
        }

        public IActionResult Results()
        {
            var results = _context.Results
                .Include(r => r.Module)
                .Where(r => r.UserId == CurrentStudentId)
                .ToList();

            ViewBag.Results = results;
            ViewBag.CGPA = CavendishACMISPortal.Services.CgpaCalculator.CalculateCGPA(results);
            return View();
        }

        public IActionResult DownloadTranscript()
        {
            var results = _context.Results
                .Include(r => r.Module)
                .Where(r => r.UserId == CurrentStudentId)
                .ToList();

            var student = _context.Users.Find(CurrentStudentId);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Transcript");

            ws.Cell(1, 1).Value = "CAVENDISH UNIVERSITY UGANDA";
            ws.Cell(2, 1).Value = "Official Academic Transcript";
            ws.Cell(4, 1).Value = "Student Name:"; ws.Cell(4, 2).Value = student?.FullName;
            ws.Cell(5, 1).Value = "Student Number:"; ws.Cell(5, 2).Value = student?.Username;

            ws.Cell(8, 1).Value = "Module";
            ws.Cell(8, 2).Value = "CAT1";
            ws.Cell(8, 3).Value = "CAT2";
            ws.Cell(8, 4).Value = "Final Exam";
            ws.Cell(8, 5).Value = "Total";
            ws.Cell(8, 6).Value = "Grade";

            int row = 9;
            foreach (var r in results)
            {
                ws.Cell(row, 1).Value = r.Module?.ModuleName;
                ws.Cell(row, 2).Value = r.CAT1;
                ws.Cell(row, 3).Value = r.CAT2;
                ws.Cell(row, 4).Value = r.FinalExam;
                ws.Cell(row, 5).Value = r.Total;
                ws.Cell(row, 6).Value = r.Grade;
                row++;
            }

            decimal cgpa = CavendishACMISPortal.Services.CgpaCalculator.CalculateCGPA(results);
            ws.Cell(row + 2, 5).Value = "CGPA";
            ws.Cell(row + 2, 6).Value = cgpa;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Transcript_{student?.Username}.xlsx");
        }
    }
}