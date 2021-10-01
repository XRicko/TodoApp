using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ToDoList.Core.Entities;

namespace ToDoList.Infrastructure.Data.Config
{
    internal class ChecklistConfig : IEntityTypeConfiguration<Checklist>
    {
        public void Configure(EntityTypeBuilder<Checklist> builder)
        {
            builder.ToTable("Checklist");

            builder.HasIndex(e => new { e.Name, e.ProjectId })
                   .IsUnique();

            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(75);

            builder.HasOne(d => d.Project)
                   .WithMany(p => p.Checklists)
                   .HasForeignKey(d => d.ProjectId)
                   .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
