using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ToDoList.Core.Entities;

namespace ToDoList.Infrastructure.Data.Config
{
    [ExcludeFromCodeCoverage]
    internal class ChecklistConfig : IEntityTypeConfiguration<Checklist>
    {
        public void Configure(EntityTypeBuilder<Checklist> builder)
        {
            builder.ToTable("Checklist");

            builder.HasIndex(e => new { e.Name, e.UserId })
                .IsUnique();

            builder.Property(e => e.Name)
                   .HasMaxLength(75);

            builder.HasOne(d => d.User)
                   .WithMany(p => p.Checklists)
                   .HasForeignKey(d => d.UserId)
                   .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
