using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ToDoList.Core.Entities;

namespace ToDoList.Infrastructure.Data.Config
{
    internal class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");

            builder.HasIndex(e => e.Name)
                   .IsUnique();

            builder.Property(e => e.Name)
                   .HasMaxLength(125);

            builder.Property(e => e.Password)
                   .HasMaxLength(256);
        }
    }
}
