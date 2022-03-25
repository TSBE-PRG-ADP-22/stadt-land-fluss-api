using Microsoft.EntityFrameworkCore;
using StadtLandFussApi.Persistence.Converters;
using StadtLandFussApi.Persistence.Entities;

namespace StadtLandFussApi.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ToDo> Todoes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyUtcDateTimeConverterToAllDateTimeProperties();
        }
    }
}
