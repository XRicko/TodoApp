using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoList.Core.Entities;

namespace ToDoList.Infrastructure.Data.Config
{
    internal class ChecklistItemConfig : IEntityTypeConfiguration<ChecklistItem>
    {
        public void Configure(EntityTypeBuilder<ChecklistItem> builder)
        {
            builder.ToTable("Task");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasOne(d => d.Category)
                .WithMany(p => p.ChecklistItems)
                .HasForeignKey(d => d.CategoryId);

            builder.HasOne(d => d.Checklist)
                .WithMany(p => p.ChecklistItems)
                .HasForeignKey(d => d.ChecklistId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Image)
                .WithMany(p => p.ChecklistItems)
                .HasForeignKey(d => d.ImageId);

            builder.HasOne(d => d.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(d => d.ParentId);

            builder.HasOne(d => d.Status)
                .WithMany(p => p.ChecklistItems)
                .HasForeignKey(d => d.StatusId);
        }
    }
}
