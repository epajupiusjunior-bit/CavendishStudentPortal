using Microsoft.AspNetCore.Mvc;
using CavendishACMISPortal.Data;

namespace CavendishACMISPortal.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;
        public StudentController(AppDbContext context) => _context = context;

        public IActionResult Dashboard() => View();
        public IActionResult BioData() => View();
        public IActionResult Results() => View();
    }
}