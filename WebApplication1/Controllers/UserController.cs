using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // HASH
        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // REGISTER
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Uživatelské jméno a heslo jsou povinné";
                return View();
            }

            if (password.Length < 6)
            {
                ViewBag.Error = "Heslo musí mít alespoň 6 znaků";
                return View();
            }

            // Zkontroluj, zda uživatel již existuje
            if (_context.Users.Any(u => u.Username == username))
            {
                ViewBag.Error = "Uživatel s tímto jménem již existuje";
                return View();
            }

            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password)
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // LOGIN
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Uživatelské jméno a heslo jsou povinné";
                return View();
            }

            var hash = HashPassword(password);

            var user = _context.Users
                .FirstOrDefault(u => u.Username == username && u.PasswordHash == hash);

            if (user != null)
            {
                HttpContext.Session.SetString("User", user.Username);
                return RedirectToAction("Profile");
            }

            ViewBag.Error = "Špatné údaje";
            return View();
        }

        // PROFILE (jen přihlášený)
        public IActionResult Profile()
        {
            if (HttpContext.Session.GetString("User") == null)
                return RedirectToAction("Login");

            return View();
        }

        // LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}