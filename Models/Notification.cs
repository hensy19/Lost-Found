using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lost_Found.Models
{
    public class Notification
    {
        [Key]
        public int notification_id { get; set; }

        [Required(ErrorMessage = "User is required")]
        public int user_id { get; set; }

        [ForeignKey("user_id")]
        public User User { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
        public string message { get; set; }

        [Required]
        public bool isRead { get; set; } = false;

        public DateTime created_at { get; set; } = DateTime.Now;
    }
}