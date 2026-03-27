using Lost_Found.DBContext;
using Lost_Found.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Lost_Found.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly Lost_FoundDB _db;

        public NotificationsController(Lost_FoundDB db)
        {
            _db = db;
        }

        // User Notifications Page
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "User");

            var notifications = _db.Notifications
                .Where(n => n.user_id == userId && !n.isRead)
                .OrderByDescending(n => n.created_at)
                .ToList();

            return View(notifications);
        }

        // Admin Notifications Page
        public IActionResult Admin()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("UserRole");

            if (userId == null || role != "Admin") return RedirectToAction("Login", "User");

            var notifications = _db.Notifications
                .Where(n => n.user_id == userId && !n.isRead)
                .OrderByDescending(n => n.created_at)
                .ToList();

            return View(notifications);
        }

        // Dismiss Notification (Mark as read)
        [HttpPost]
        public IActionResult Dismiss(int id)
        {
            var notification = _db.Notifications.FirstOrDefault(n => n.notification_id == id);
            if (notification != null)
            {
                notification.isRead = true;
                _db.SaveChanges();
            }
            return Ok();
        }

        // Get unread count for badge
        [HttpGet]
        public IActionResult GetUnreadCount()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(0);

            var count = _db.Notifications.Count(n => n.user_id == userId && !n.isRead);
            return Json(count);
        }
    }
}
