using Lost_Found.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lost_Found.Controllers
{
    public class ItemsController : Controller
    {
        static List<Item> items = new List<Item>()
{
    new Item {
        Id = 1,
        Title = "iPhone 13 Pro",
        Category = "Electronics",
        Type = "Lost",
        Location = "Library",
        Description = "Lost my iPhone near study area",
        Date = DateTime.Now.AddDays(-2),
        Status = "Active",
        ImagePath = "/images/phone.jpg"
    },
    new Item {
        Id = 2,
        Title = "Blue Backpack",
        Category = "Accessories",
        Type = "Found",
        Location = "Cafeteria",
        Description = "Found near table",
        Date = DateTime.Now.AddDays(-1),
        Status = "Claimed",
        ImagePath = "/images/bag.png"
    }
}; static int idCounter = 1;

        public IActionResult Index()
        {
            return View(items);
        }

        public IActionResult ReportItem()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            var item = items.FirstOrDefault(x => x.Id == id);
            return View(item);
        }
        public IActionResult Edit(int id)
        {
            var item = items.FirstOrDefault(x => x.Id == id);
            return View(item);
        }
        public IActionResult Delete(int id)
        {
            var item = items.FirstOrDefault(x => x.Id == id);
            if (item != null)
                items.Remove(item);

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult ClaimItem(int itemId, string itemName)
        {
            var item = items.FirstOrDefault(x => x.Id == itemId);

            if (item != null)
            {
                item.Status = "Claimed";
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult Edit([FromBody] Item updated)
        {
            var item = items.FirstOrDefault(x => x.Id == updated.Id);

            if (item != null)
            {
                item.Title = updated.Title;
                item.Category = updated.Category;
                item.Location = updated.Location;
                item.Description = updated.Description;
            }

            return Ok();
        }
        [HttpPost]
        public IActionResult EditWithImage(Item updated, IFormFile imageFile)
        {
            var item = items.FirstOrDefault(x => x.Id == updated.Id);

            if (item != null)
            {
                item.Title = updated.Title;
                item.Category = updated.Category;
                item.Location = updated.Location;
                item.Description = updated.Description;

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
            }

            return Ok();
        }
        [HttpPost]
        public IActionResult ReportItem(Item item, IFormFile imageFile)
        {
            item.Id = idCounter++;
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

            items.Add(item);

            return RedirectToAction("Index");
        }
    }
}
