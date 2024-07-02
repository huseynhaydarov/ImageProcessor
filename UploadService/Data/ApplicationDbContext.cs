using Microsoft.EntityFrameworkCore;

namespace UploadService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Photo> Photos { get; set; }
    }
}