using Lost_Found.Models;
using Lost_Found.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System;

namespace Lost_Found.Controllers
{
    public class ItemsController : Controller
    {
        private readonly Lost_FoundDB _db;

        public ItemsController(Lost_FoundDB db)
        {
            _db = db;
        }

        public IActionResult Index(string category)
        {
            ViewBag.Category = category;

            // --- ONE-TIME DATA CLEAR (Runs once if table is not empty) ---
            if (_db.Items.Any() && HttpContext.Session.GetInt32("DataWipeComplete") == null)
            {
                _db.Items.RemoveRange(_db.Items);
                _db.Notifications.RemoveRange(_db.Notifications);
                _db.SaveChanges();
                HttpContext.Session.SetInt32("DataWipeComplete", 1);
            }

            var allItems = _db.Items.OrderByDescending(i => i.Id).ToList();
            return View(allItems);
        }

        public IActionResult ReportItem(string type)
        {
            ViewBag.Type = type;
            return View();
        }

        public IActionResult Details(int id)
        {
            var item = _db.Items.FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Edit(int id)
        {
            var item = _db.Items.FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Delete(int id, string returnUrl = null)
        {
            var item = _db.Items.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                // Verify the item isn't using a mocked URL prior to physical removal action
                if (!string.IsNullOrEmpty(item.ImagePath) && item.ImagePath != "/images/default.png" && !item.ImagePath.StartsWith("http"))
                {
                    var fileToDelete = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", item.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(fileToDelete))
                    {
                        System.IO.File.Delete(fileToDelete);
                    }
                }

                _db.Items.Remove(item);
                _db.SaveChanges();
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RequestClaim(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var item = _db.Items.FirstOrDefault(x => x.Id == id);
            if (item != null && item.Status == "Active")
            {
                item.Status = "Under Review";
                if (userId != null)
                {
                    item.claimed_by_user_id = userId;
                }
                
                // 1. Notify Original Reporter
                if (item.user_id != null)
                {
                    _db.Notifications.Add(new Notification {
                        user_id = item.user_id.Value,
                        message = $"Someone has requested to claim your item: '{item.Title}'.",
                        isRead = false,
                        created_at = DateTime.Now
                    });
                }

                // 2. Notify ALL Admins
                var admins = _db.Users.Where(u => u.role == UserRole.Admin).ToList();
                foreach (var admin in admins)
                {
                    _db.Notifications.Add(new Notification {
                        user_id = admin.user_id,
                        message = $"NEW CLAIM REQUEST for item: '{item.Title}' (ID: {item.Id}).",
                        isRead = false,
                        created_at = DateTime.Now
                    });
                }

                _db.SaveChanges();
            }
            return Ok();
        }

        public IActionResult MyClaims()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "User");

            var claimedItems = _db.Items.Where(x => x.claimed_by_user_id == userId).OrderByDescending(i => i.Id).ToList();
            return View(claimedItems);
        }

        public IActionResult AdminDashboard()
        {
            var pendingItems = _db.Items.Where(x => x.Status == "Under Review").ToList();
            return View(pendingItems);
        }

        [HttpPost]
        public IActionResult ApproveClaim(int id)
        {
            var item = _db.Items.FirstOrDefault(x => x.Id == id);
            if (item != null && item.Status == "Under Review")
            {
                item.Status = "Claimed";
                
                // Notify Claimant
                if (item.claimed_by_user_id != null)
                {
                    _db.Notifications.Add(new Notification {
                        user_id = item.claimed_by_user_id.Value,
                        message = $"CONGRATULATIONS! Your claim for '{item.Title}' has been APPROVED by the admin team.",
                        isRead = false,
                        created_at = DateTime.Now
                    });
                }
                
                _db.SaveChanges();
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult RejectClaim(int id)
        {
            var item = _db.Items.FirstOrDefault(x => x.Id == id);
            if (item != null && item.Status == "Under Review")
            {
                var claimantId = item.claimed_by_user_id;

                // Revert to Active
                item.Status = "Active";
                item.claimed_by_user_id = null;
                item.WasRejected = true;
                
                // Notify Claimant
                if (claimantId != null)
                {
                    _db.Notifications.Add(new Notification {
                        user_id = claimantId.Value,
                        message = $"UPDATE: Your claim request for '{item.Title}' was rejected. Please contact support if you believe this is an error.",
                        isRead = false,
                        created_at = DateTime.Now
                    });
                }
                
                _db.SaveChanges();
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult ResolveItem(int id)
        {
            var item = _db.Items.FirstOrDefault(x => x.Id == id);
            if (item != null && item.Status == "Claimed")
            {
                item.Status = "Resolved";

                // Notify Claimant
                if (item.claimed_by_user_id != null)
                {
                    _db.Notifications.Add(new Notification {
                        user_id = item.claimed_by_user_id.Value,
                        message = $"FINALIZED: Your claim for '{item.Title}' has been RESOLVED and closed.",
                        isRead = false,
                        created_at = DateTime.Now
                    });
                }

                // Notify Admin (optional but good for tracking)
                var admins = _db.Users.Where(u => u.role == UserRole.Admin).ToList();
                foreach (var admin in admins)
                {
                    _db.Notifications.Add(new Notification {
                        user_id = admin.user_id,
                        message = $"RESOLVED: Item ID {item.Id} ('{item.Title}') has been successfully resolved.",
                        isRead = false,
                        created_at = DateTime.Now
                    });
                }

                _db.SaveChanges();
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult Edit(Item updated, IFormFile imageFile)
        {
            var item = _db.Items.FirstOrDefault(x => x.Id == updated.Id);

            if (item != null)
            {
                item.Title = updated.Title;
                item.Category = updated.Category;
                item.Location = updated.Location;
                item.Description = updated.Description;

                if (imageFile != null)
                {
                    if (!string.IsNullOrEmpty(item.ImagePath) && item.ImagePath != "/images/default.png" && !item.ImagePath.StartsWith("http"))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", item.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    var fileName = Path.GetFileName(imageFile.FileName);
                    var newPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(newPath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    item.ImagePath = "/images/" + fileName;
                }
                
                _db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = updated.Id });
        }

        [HttpPost]
        public IActionResult ReportItem(Item item, IFormFile imageFile)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                item.user_id = userId;
            }

            item.Date = DateTime.Now;
            item.Status = "Active";

            if (imageFile != null)
            {
                var fileName = Path.GetFileName(imageFile.FileName);
                var path = Path.Combine("wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                item.ImagePath = "/images/" + fileName;
            }

            _db.Items.Add(item);
            _db.SaveChanges(); // Save item first

            // --- BROADCAST NOTIFICATION TO EVERYONE ---
            try 
            {
                var allUsers = _db.Users.ToList();
                var alertType = item.Type?.ToUpper() ?? "NEW";
                var alertTitle = item.Title ?? "Item";
                var alertLocation = item.Location ?? "Campus";
                
                var alertMessage = $"📢 {alertType} ALERT: '{alertTitle}' was reported at {alertLocation}. Check it out!";

                // Ensure message doesn't exceed 500 characters
                if (alertMessage.Length > 500) alertMessage = alertMessage.Substring(0, 497) + "...";

                var notifications = allUsers.Select(user => new Notification {
                    user_id = user.user_id,
                    message = alertMessage,
                    isRead = false,
                    created_at = DateTime.Now
                }).ToList();

                _db.Notifications.AddRange(notifications);
                _db.SaveChanges();
                
                Console.WriteLine($"[DEBUG] Broadcasted {notifications.Count} notifications for item '{alertTitle}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to broadcast notifications: {ex.Message}");
            }

            return RedirectToAction("Index");
        }
    }
}
