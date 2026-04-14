using Microsoft.AspNetCore.Mvc;
using CavendishACMISPortal.Data;
using CavendishACMISPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace CavendishACMISPortal.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;
        public StudentController(AppDbContext context) => _context = context;

        private int CurrentUserId => int.Parse(HttpContext.Session.GetString("UserId")!);

        public IActionResult Dashboard()
        {
            var user = _context.Users.Find(CurrentUserId);
            ViewBag.User = user;
            return View();
        }

        public IActionResult BioData()
        {
            var user = _context.Users.Find(CurrentUserId);
            ViewBag.User = user;
            return View();
        }

        [HttpPost]
        public IActionResult UpdateProfile(string phone, string email)
        {
            var user = _context.Users.Find(CurrentUserId);
            if (user != null)
            {
                user.Phone = phone;
                user.Email = email;
                _context.SaveChanges();
            }
            return RedirectToAction("BioData");
        }

        public IActionResult GeneratePRN()
        {
            var user = _context.Users.Find(CurrentUserId);
            var prns = _context.GeneratedPRNs.Where(p => p.UserId == CurrentUserId).ToList();
            ViewBag.User = user;
            ViewBag.PRNs = prns;
            return View();
        }

        [HttpPost]
        public IActionResult GenerateNewPRN(decimal amount, string purpose)
        {
            var prn = new GeneratedPRN
            {
                UserId = CurrentUserId,
                PRNNumber = "PRN-" + DateTime.Now.Ticks,
                Amount = amount,
                Purpose = purpose
            };
            _context.GeneratedPRNs.Add(prn);
            _context.SaveChanges();
            return RedirectToAction("GeneratePRN");
        }

        public IActionResult Results()
        {
            var results = _context.Results
                .Include(r => r.Module)
                .Where(r => r.UserId == CurrentUserId)
                .ToList();
            ViewBag.Results = results;
            return View();
        }

        public IActionResult Enrolment()
        {
            var modules = _context.Modules.Include(m => m.Course).ToList();
            ViewBag.Modules = modules;
            return View();
        }

        [HttpPost]
        public IActionResult RegisterModule(int moduleId)
        {
            var reg = new StudentModuleRegistration { UserId = CurrentUserId, ModuleId = moduleId };
            _context.Registrations.Add(reg);
            _context.SaveChanges();
            return RedirectToAction("Enrolment");
        }
    }
}