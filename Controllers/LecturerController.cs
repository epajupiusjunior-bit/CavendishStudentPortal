using Microsoft.AspNetCore.Mvc;
using CavendishACMISPortal.Data;
using CavendishACMISPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace CavendishACMISPortal.Controllers
{
    public class LecturerController : Controller
    {
        private readonly AppDbContext _context;

        public LecturerController(AppDbContext context) => _context = context;

        private int CurrentLecturerId => int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

        // ==================== DASHBOARD ====================
        public IActionResult Dashboard()
        {
            var lecturer = _context.Users.Find(CurrentLecturerId);
            var assignedModules = _context.LecturerAssignments
                .Include(a => a.Module)
                .ThenInclude(m => m.Course)
                .Where(a => a.LecturerId == CurrentLecturerId)
                .ToList();

            ViewBag.Lecturer = lecturer;
            ViewBag.AssignedModules = assignedModules;
            return View();
        }

        // ==================== MY MODULES ====================
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

        // ==================== STUDENTS IN A MODULE ====================
        public IActionResult MyStudents(int moduleId)
        {
            var students = _context.Results
                .Include(r => r.User)
                .Where(r => r.ModuleId == moduleId)
                .Select(r => r.User)
                .Distinct()
                .ToList();

            ViewBag.ModuleId = moduleId;
            ViewBag.ModuleName = _context.Modules.Find(moduleId)?.ModuleName ?? "Unknown Module";
            return View(students);
        }

        // ==================== ENTER RESULT ====================
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
                ModelState.AddModelError("", "Invalid scores. CAT1 & CAT2 must be ≤ 20, Final Exam ≤ 60.");
                ViewBag.ModuleId = model.ModuleId;
                ViewBag.Students = _context.Users.Where(u => u.Role == "Student").ToList();
                return View(model);
            }

            _context.Results.Add(model);
            _context.SaveChanges();
            return RedirectToAction("MyStudents", new { moduleId = model.ModuleId });
        }

        // ==================== GRADE REPORT ====================
        public IActionResult GradeReport(int moduleId)
        {
            var results = _context.Results
                .Include(r => r.User)
                .Include(r => r.Module)
                .Where(r => r.ModuleId == moduleId)
                .ToList();

            ViewBag.ModuleId = moduleId;
            return View(results);
        }
    }
}