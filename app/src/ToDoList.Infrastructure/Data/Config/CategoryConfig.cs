using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ToDoList.Core.Entities;

namespace ToDoList.Infrastructure.Data.Config
{
    [ExcludeFromCodeCoverage]
    internal class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Category");

            builder.HasIndex(e => e.Name)
                   .IsUnique();

            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(75);
        }
    }
}
