using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Lost_Found.Models;
using Lost_Found.DBContext;

namespace Lost_Found.Controllers
{
    public class AdminController : Controller
    {
        private readonly Lost_FoundDB _db;

        public AdminController(Lost_FoundDB db)
        {
            _db = db;
        }

        public IActionResult Dashboard()
        {
            ViewBag.ActivePage = "Dashboard";
            ViewBag.TotalUsers = _db.Users.Count(u => u.role == UserRole.User);
            ViewBag.TotalAdmins = _db.Users.Count(u => u.role == UserRole.Admin);
            ViewBag.TotalItems = _db.Items.Count();
            
            // Fetch items that are under review, claimed, or rejected
            var recentClaims = _db.Items.Where(i => i.Status == "Under Review" || i.Status == "Claimed" || i.Status == "Rejected")
                                       .OrderByDescending(i => i.Id)
                                       .Take(5)
                                       .ToList();
            return View(recentClaims);
        }

        public IActionResult LostItems()
        {
            ViewBag.ActivePage = "LostItems";
            var lostItems = _db.Items.Where(i => i.Type == "Lost").OrderByDescending(i => i.Id).ToList();
            return View(lostItems);
        }

        public IActionResult FoundItems()
        {
            ViewBag.ActivePage = "FoundItems";
            var foundItems = _db.Items.Where(i => i.Type == "Found").OrderByDescending(i => i.Id).ToList();
            return View(foundItems);
        }

        public IActionResult ManageUsers()
        {
            ViewBag.ActivePage = "ManageUsers";
            var users = _db.Users.OrderByDescending(u => u.user_id).ToList();
            return View(users);
        }

        [HttpPost]
        public IActionResult BlockUser(int id)
        {
            var user = _db.Users.FirstOrDefault(u => u.user_id == id);
            if (user != null)
            {
                user.IsBlocked = !user.IsBlocked;
                _db.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var user = _db.Users.FirstOrDefault(u => u.user_id == id);
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        public IActionResult ManageClaims()
        {
            ViewBag.ActivePage = "ManageClaims";
            var claims = _db.Items.Where(i => i.Status == "Under Review" || i.Status == "Claimed" || i.Status == "Resolved" || i.Status == "Rejected")
                                  .OrderByDescending(i => i.Id)
                                  .ToList();
            return View(claims);
        }

        public IActionResult AdminProfile()
        {
            ViewBag.ActivePage = "Profile";
            var userId = HttpContext.Session.GetInt32("UserId");
            var admin = _db.Users.FirstOrDefault(u => u.user_id == userId);
            return View(admin);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }

        public async Task<IActionResult> EditAdminProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var admin = await _db.Users.FindAsync(userId);
            if (admin == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(admin);
        }

        [HttpPost]
        public async Task<IActionResult> EditAdminProfile(string user_name, string password)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var admin = await _db.Users.FindAsync(userId);
            if (admin == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (!string.IsNullOrEmpty(user_name))
            {
                admin.user_name = user_name;
                HttpContext.Session.SetString("UserName", user_name);
            }

            if (!string.IsNullOrEmpty(password))
            {
                admin.password = password;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("AdminProfile");
        }
    }
}