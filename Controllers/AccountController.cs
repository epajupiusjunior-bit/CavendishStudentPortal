using Microsoft.AspNetCore.Mvc;
using CavendishACMISPortal.Data;
using CavendishACMISPortal.Models;

namespace CavendishACMISPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context) => _context = context;

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("FullName", user.FullName);
                HttpContext.Session.SetString("Role", user.Role);

                return user.Role == "Admin"
                    ? RedirectToAction("Dashboard", "Admin")
                    : RedirectToAction("Dashboard", "Student");
            }
            ViewBag.Error = "Invalid credentials";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}