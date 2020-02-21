using Microsoft.EntityFrameworkCore;

namespace ActivityEvent.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ActPart> ActParts { get; set; }
        public DbSet<Act> Acts { get; set; }
    }
}