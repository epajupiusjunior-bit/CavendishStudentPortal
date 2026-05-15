using Microsoft.AspNetCore.Mvc;
using CavendishACMISPortal.Data;
using CavendishACMISPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace CavendishACMISPortal.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context) => _context = context;

        public IActionResult Dashboard() => View();

        public IActionResult Students() => View(_context.Users.Where(u => u.Role == "Student").ToList());
        public IActionResult AddStudent() => View();
        public IActionResult Courses() => View(_context.Courses.ToList());
        public IActionResult Modules() => View(_context.Modules.Include(m => m.Course).ToList());
        public IActionResult Results() => View(_context.Results.Include(r => r.User).Include(r => r.Module).ToList());
    }
}