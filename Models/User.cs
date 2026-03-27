using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lost_Found.Models
{
    public enum UserRole
    {
        Admin,
        User
    }

    public class User
    {
        [Key]
        public int user_id { get; set; }

        [Required(ErrorMessage = "User name is required")]
        [MaxLength(100, ErrorMessage = "User name cannot exceed 100 characters")]
        public string? user_name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? user_email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string? password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public UserRole role { get; set; }

        public bool IsBlocked { get; set; } = false;

        public DateTime created_at { get; set; } = DateTime.Now;

        public ICollection<Notification>? Notifications { get; set; }

        [InverseProperty("Reporter")]
        public ICollection<Item>? ReportedItems { get; set; }

        [InverseProperty("Claimer")]
        public ICollection<Item>? ClaimedItems { get; set; }
    }
}