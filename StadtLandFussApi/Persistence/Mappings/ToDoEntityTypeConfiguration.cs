using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StadtLandFussApi.Persistence.Entities;

namespace StadtLandFussApi.Persistence.Mappings
{
    public class ToDoEntityTypeConfiguration : IEntityTypeConfiguration<ToDo>
    {
        public void Configure(EntityTypeBuilder<ToDo> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Key)
                .HasMaxLength(500)
                .IsRequired();
        }
    }
}
