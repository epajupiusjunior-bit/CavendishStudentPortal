using Microsoft.AspNetCore.Mvc;
using CavendishACMISPortal.Data;
using Microsoft.EntityFrameworkCore;

namespace CavendishACMISPortal.Controllers
{
    public class LecturerController : Controller
    {
        private readonly AppDbContext _context;
        public LecturerController(AppDbContext context) => _context = context;

        private int CurrentId => int.Parse(HttpContext.Session.GetString("UserId")!);

        public IActionResult Dashboard()
        {
            var lecturer = _context.Users.Find(CurrentId);
            ViewBag.Lecturer = lecturer;
            return View();
        }

        public IActionResult MyModules()
        {
            var modules = _context.LecturerAssignments
                .Include(a => a.Module)
                .ThenInclude(m => m.Course)
                .Where(a => a.LecturerId == CurrentId)
                .Select(a => a.Module)
                .ToList();
            return View(modules);
        }
    }
}