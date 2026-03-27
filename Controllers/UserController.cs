using Microsoft.AspNetCore.Mvc;
using Lost_Found.Models;
using Lost_Found.DBContext;

namespace Lost_Found.Controllers
{
    public class UserController : Controller
    {
        private Lost_FoundDB db;

        public UserController(Lost_FoundDB db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                // Print validation errors (for debugging)
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        Console.WriteLine("ERROR: " + error.ErrorMessage);
                    }
                }
                return View(user);
            }

            // Check if email already exists
            var existingUser = db.Users.FirstOrDefault(u => u.user_email == user.user_email);
            if (existingUser != null)
            {
                ViewBag.Error = "Email already registered.";
                return View(user);
            }

            // Set default role if not specified
            if (user.role == 0)
            {
                user.role = UserRole.User;
            }

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please enter email and password.";
                return View();
            }

            var user = db.Users
                         .FirstOrDefault(u => u.user_email == email && u.password == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            // Save user session
            HttpContext.Session.SetInt32("UserId", user.user_id);
            HttpContext.Session.SetString("UserName", user.user_name ?? "User");
            HttpContext.Session.SetString("UserRole", user.role.ToString());

            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
