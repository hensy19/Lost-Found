using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Lost_Found.DBContext
{
    public class Lost_FoundDBFactory : IDesignTimeDbContextFactory<Lost_FoundDB>
    {
        public Lost_FoundDB CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Lost_FoundDB>();

            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\MSSQLLocalDB;Database=lost_found;Trusted_Connection=True"
            );

            return new Lost_FoundDB(optionsBuilder.Options);
        }
    }
}