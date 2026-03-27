using System.ComponentModel.DataAnnotations;

namespace Lost_Found.Models
{

    public class Item
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Category { get; set; }

        public string Location { get; set; }

        public DateTime Date { get; set; }

        public string Type { get; set; } // Lost / Found

        public string Status { get; set; } = "Active";

        public string ImagePath { get; set; }
        // Active → Claimed → Verified → Resolved
    }
}
