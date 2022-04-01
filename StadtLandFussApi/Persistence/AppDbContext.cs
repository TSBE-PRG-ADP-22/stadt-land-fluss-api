using Microsoft.EntityFrameworkCore;
using StadtLandFussApi.Models;
using StadtLandFussApi.Persistence.Converters;

namespace StadtLandFussApi.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Answer> Answers { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Lobby> Lobbies { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyUtcDateTimeConverterToAllDateTimeProperties();
        }
    }
}
