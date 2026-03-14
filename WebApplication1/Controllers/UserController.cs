using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _dbContext;

        public UserController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string password, string passwordCheck)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewData["chyba"] = "Jméno i heslo musí být zadáno.";

                return View();
            }
            else if (string.IsNullOrEmpty(passwordCheck) || passwordCheck != password)
            {
                ViewData["chyba"] = "Heslo a heslo pro kontrolu se musí rovnat.";

                return View();
            }

            _dbContext.Users.Add(new User() { Name = username, Password = password });
            _dbContext.SaveChanges();

            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }
    }
}
