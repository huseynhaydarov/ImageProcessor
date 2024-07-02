using Microsoft.EntityFrameworkCore;

namespace ProcessingService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public DbSet<Photo> Photos { get; set; }
    }
}