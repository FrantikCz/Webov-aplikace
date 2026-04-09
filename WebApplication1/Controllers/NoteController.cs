using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class NoteController : Controller
    {
        private readonly AppDbContext _context;

        public NoteController(AppDbContext context)
        {
            _context = context;
        }

        // Ověření přihlášení
        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetString("User") != null;
        }

        // Získání aktuálního uživatele
        private User GetCurrentUser()
        {
            var username = HttpContext.Session.GetString("User");
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        // SEZNAM VŠECH POZNÁMEK
        public IActionResult Index()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "User");

            var user = GetCurrentUser();
            var notes = _context.Notes
                .Where(n => n.UserId == user.Id)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return View(notes);
        }

        // VYTVOŘENÍ POZNÁMKY
        public IActionResult Create()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "User");

            return View();
        }

        [HttpPost]
        public IActionResult Create(string title, string content)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "User");

            if (string.IsNullOrWhiteSpace(title))
            {
                ViewBag.Error = "Nadpis je povinný";
                return View();
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                ViewBag.Error = "Obsah poznámky je povinný";
                return View();
            }

            var user = GetCurrentUser();

            var note = new Note
            {
                Title = title.Trim(),
                Content = content.Trim(),
                UserId = user.Id,
                CreatedAt = DateTime.Now
            };

            _context.Notes.Add(note);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // ÚPRAVA POZNÁMKY
        public IActionResult Edit(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "User");

            var note = _context.Notes.FirstOrDefault(n => n.Id == id);

            if (note == null)
                return NotFound();

            var user = GetCurrentUser();

            if (note.UserId != user.Id)
                return Forbid();

            return View(note);
        }

        [HttpPost]
        public IActionResult Edit(int id, string title, string content)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "User");

            var note = _context.Notes.FirstOrDefault(n => n.Id == id);

            if (note == null)
                return NotFound();

            var user = GetCurrentUser();

            if (note.UserId != user.Id)
                return Forbid();

            if (string.IsNullOrWhiteSpace(title))
            {
                ViewBag.Error = "Nadpis je povinný";
                return View(note);
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                ViewBag.Error = "Obsah poznámky je povinný";
                return View(note);
            }

            note.Title = title.Trim();
            note.Content = content.Trim();
            note.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // SMAZÁNÍ POZNÁMKY
        public IActionResult Delete(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "User");

            var note = _context.Notes.FirstOrDefault(n => n.Id == id);

            if (note == null)
                return NotFound();

            var user = GetCurrentUser();

            if (note.UserId != user.Id)
                return Forbid();

            _context.Notes.Remove(note);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
