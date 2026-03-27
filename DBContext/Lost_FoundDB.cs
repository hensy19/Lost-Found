using Microsoft.EntityFrameworkCore;
using Lost_Found.Models;

namespace Lost_Found.DBContext
{
    public class Lost_FoundDB : DbContext
    {
        public Lost_FoundDB(DbContextOptions<Lost_FoundDB> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}
