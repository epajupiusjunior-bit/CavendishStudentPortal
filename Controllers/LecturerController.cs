using Microsoft.AspNetCore.Mvc;
using CavendishACMISPortal.Data;
using CavendishACMISPortal.Models;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.IO;

namespace CavendishACMISPortal.Controllers
{
    public class LecturerController : Controller
    {
        private readonly AppDbContext _context;

        public LecturerController(AppDbContext context) => _context = context;

        private int CurrentLecturerId => int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

        public IActionResult Dashboard()
        {
            var lecturer = _context.Users.Find(CurrentLecturerId);

            var assignedModules = _context.LecturerAssignments
                .Include(a => a.Module)
                .ThenInclude(m => m.Course)
                .Where(a => a.LecturerId == CurrentLecturerId)
                .ToList();

            var moduleStats = assignedModules.Select(a => new ModuleStatViewModel
            {
                ModuleAssignment = a,
                StudentCount = _context.Registrations
                    .Where(r => r.ModuleId == a.ModuleId)
                    .Select(r => r.UserId)
                    .Distinct()
                    .Count()
            }).ToList();

            ViewBag.Lecturer = lecturer;
            ViewBag.ModuleStats = moduleStats;
            ViewBag.TotalStudentsTaught = moduleStats.Sum(m => m.StudentCount);

            return View();
        }

        public IActionResult MyModules()
        {
            var modules = _context.LecturerAssignments
                .Include(a => a.Module)
                .ThenInclude(m => m.Course)
                .Where(a => a.LecturerId == CurrentLecturerId)
                .Select(a => a.Module)
                .ToList();

            return View(modules);
        }

        public IActionResult MyStudents(int moduleId)
        {
            var students = _context.Registrations
                .Include(r => r.User)
                .Where(r => r.ModuleId == moduleId)
                .Select(r => r.User)
                .Distinct()
                .ToList();

            var results = _context.Results
                .Include(r => r.User)
                .Where(r => r.ModuleId == moduleId)
                .ToList();

            ViewBag.ModuleId = moduleId;
            ViewBag.ModuleName = _context.Modules.Find(moduleId)?.ModuleName;
            ViewBag.Results = results;

            return View(students);
        }
        // ==================== MODULE PERFORMANCE REPORT ====================
        public IActionResult ModulePerformance(int moduleId)
        {
            var results = _context.Results
                .Include(r => r.User)
                .Where(r => r.ModuleId == moduleId)
                .ToList();

            var module = _context.Modules.Find(moduleId);

            if (module == null)
                return RedirectToAction("MyModules");

            ViewBag.Module = module;
            return View(results);
        }
        // ==================== RESULT CRUD ====================
        public IActionResult AddResult(int moduleId)
        {
            ViewBag.ModuleId = moduleId;
            ViewBag.Students = _context.Users.Where(u => u.Role == "Student").ToList();
            return View(new Result());
        }

        [HttpPost]
        public IActionResult AddResult(Result model)
        {
            if (!model.IsValid())
            {
                ModelState.AddModelError("", "Invalid scores.");
                ViewBag.ModuleId = model.ModuleId;
                ViewBag.Students = _context.Users.Where(u => u.Role == "Student").ToList();
                return View(model);
            }

            _context.Results.Add(model);
            _context.SaveChanges();
            return RedirectToAction("MyStudents", new { moduleId = model.ModuleId });
        }

        public IActionResult EditResult(int id)
        {
            var result = _context.Results
                .Include(r => r.User)
                .Include(r => r.Module)
                .FirstOrDefault(r => r.Id == id);

            if (result == null) return RedirectToAction("MyModules");

            ViewBag.ModuleId = result.ModuleId;
            return View(result);
        }

        [HttpPost]
        public IActionResult EditResult(Result model)
        {
            if (!model.IsValid()) return View(model);

            var existing = _context.Results.Find(model.Id);
            if (existing != null)
            {
                existing.CAT1 = model.CAT1;
                existing.CAT2 = model.CAT2;
                existing.FinalExam = model.FinalExam;
                _context.SaveChanges();
            }
            return RedirectToAction("MyStudents", new { moduleId = model.ModuleId });
        }

        public IActionResult DeleteResult(int id)
        {
            var result = _context.Results.Find(id);
            if (result != null)
            {
                int moduleId = result.ModuleId;
                _context.Results.Remove(result);
                _context.SaveChanges();
                return RedirectToAction("MyStudents", new { moduleId });
            }
            return RedirectToAction("MyModules");
        }

        // ==================== BULK IMPORT ====================
        public IActionResult BulkImport(int moduleId)
        {
            ViewBag.ModuleId = moduleId;
            ViewBag.ModuleName = _context.Modules.Find(moduleId)?.ModuleName;
            return View();
        }

        [HttpPost]
        public IActionResult BulkImport(int moduleId, IFormFile excelFile)
        {
            if (excelFile == null)
            {
                TempData["Error"] = "Please upload an Excel file";
                return RedirectToAction("BulkImport", new { moduleId });
            }

            using var stream = excelFile.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var ws = workbook.Worksheet(1);

            for (int row = 2; row <= ws.LastRowUsed().RowNumber(); row++)
            {
                string studentNo = ws.Cell(row, 1).GetString();
                decimal cat1 = ws.Cell(row, 2).GetValue<decimal>();
                decimal cat2 = ws.Cell(row, 3).GetValue<decimal>();
                decimal final = ws.Cell(row, 4).GetValue<decimal>();

                var student = _context.Users.FirstOrDefault(u => u.Username == studentNo && u.Role == "Student");
                if (student != null)
                {
                    var result = _context.Results.FirstOrDefault(r => r.UserId == student.Id && r.ModuleId == moduleId);
                    if (result == null)
                    {
                        result = new Result { UserId = student.Id, ModuleId = moduleId };
                        _context.Results.Add(result);
                    }
                    result.CAT1 = cat1;
                    result.CAT2 = cat2;
                    result.FinalExam = final;
                }
            }

            _context.SaveChanges();
            TempData["Success"] = "Results imported successfully!";
            return RedirectToAction("MyStudents", new { moduleId });
        }
    }
}