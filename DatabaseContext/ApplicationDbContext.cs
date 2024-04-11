using Microsoft.EntityFrameworkCore;

namespace MoviesApp.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options): base(options)
        {
            
        }
    }
}
