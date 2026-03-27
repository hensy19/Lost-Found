using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lost_Found.Models
{
    public class Item
    {
        [Key]
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

        public string? ImagePath { get; set; }

        public int? user_id { get; set; }
        [ForeignKey("user_id")]
        public User? Reporter { get; set; }

        public int? claimed_by_user_id { get; set; }
        [ForeignKey("claimed_by_user_id")]
        public User? Claimer { get; set; }

        public bool WasRejected { get; set; } = false;
        // Active → Claimed → Verified → Resolved
    }
}
