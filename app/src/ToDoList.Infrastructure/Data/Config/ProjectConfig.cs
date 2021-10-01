using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ToDoList.Core.Entities;

namespace ToDoList.Infrastructure.Data.Config
{
    internal class ProjectConfig : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Project");

            builder.HasIndex(e => new { e.Name, e.UserId })
                   .IsUnique();

            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(175);

            builder.HasOne(d => d.User)
                   .WithMany(p => p.Projects)
                   .HasForeignKey(d => d.UserId)
                   .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
