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

        public IActionResult Index()
        {
            if (!_db.Items.Any())
            {
                // Seed default items if table is empty
                _db.Items.AddRange(
                    new Item {
                        Title = "MacBook Pro Charger",
                        Category = "Electronics",
                        Type = "Found",
                        Location = "Library",
                        Description = "Found a MacBook Pro Charger near the computer section.",
                        Date = DateTime.Now.AddHours(-5),
                        Status = "Active",
                        ImagePath = "/images/macbook-charger.png"
                    },
                    new Item {
                        Title = "Calculus Textbook",
                        Category = "Books",
                        Type = "Found",
                        Location = "Cafeteria",
                        Description = "Found Calculus textbook. Looks brand new.",
                        Date = DateTime.Now.AddDays(-7),
                        Status = "Active",
                        ImagePath = "/images/calculus-textbook.png"
                    },
                    new Item {
                        Title = "Analog Wristwatch",
                        Category = "Accessories",
                        Type = "Found",
                        Location = "Gymnasium",
                        Description = "Silver analog wristwatch found in the locker room.",
                        Date = DateTime.Now.AddHours(-8),
                        Status = "Active",
                        ImagePath = "/images/analog-wristwatch.png"
                    },
                    new Item {
                        Title = "Blue Herschel Bag",
                        Category = "Bags",
                        Type = "Found",
                        Location = "Student Union",
                        Description = "Found a Blue Herschel Bag left unattended.",
                        Date = DateTime.Now.AddHours(-9),
                        Status = "Active",
                        ImagePath = "/images/blue-herschel-bag.png"
                    }
                );
                _db.SaveChanges();
            }

            var allItems = _db.Items.OrderByDescending(i => i.Id).ToList();
            return View(allItems);
        }

        public IActionResult ReportItem()
        {
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

        public IActionResult Delete(int id)
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
                // Revert to Active so user can claim again, but set history flag
                item.Status = "Active";
                item.claimed_by_user_id = null;
                item.WasRejected = true;
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
                // Final state: Resolved
                item.Status = "Resolved";
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

                // Handle replacement of an active image
                if (imageFile != null)
                {
                    // Erase old image file to save database bulk storage
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

            // Successfully processed edit mapping out
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
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
