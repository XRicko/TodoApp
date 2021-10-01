using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ToDoList.Core.Entities;

namespace ToDoList.Infrastructure.Data.Config
{
    internal class ImageConfig : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.ToTable("Image");

            builder.HasIndex(e => e.Path)
                   .IsUnique();

            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(e => e.Path)
                   .IsRequired()
                   .HasMaxLength(260);
        }
    }
}
