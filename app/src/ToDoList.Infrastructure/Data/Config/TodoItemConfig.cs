
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ToDoList.Core.Entities;

namespace ToDoList.Infrastructure.Data.Config
{
    internal class TodoItemConfig : IEntityTypeConfiguration<TodoItem>
    {
        public void Configure(EntityTypeBuilder<TodoItem> builder)
        {
            builder.ToTable("Task");

            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasIndex(e => new { e.Name, e.ChecklistId })
                   .IsUnique();

            builder.HasOne(d => d.Category)
                   .WithMany(p => p.TodoItems)
                   .HasForeignKey(d => d.CategoryId);

            builder.HasOne(d => d.Checklist)
                   .WithMany(p => p.TodoItems)
                   .HasForeignKey(d => d.ChecklistId)
                   .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Image)
                   .WithMany(p => p.TodoItems)
                   .HasForeignKey(d => d.ImageId);

            builder.HasOne(d => d.Status)
                   .WithMany(p => p.TodoItems)
                   .HasForeignKey(d => d.StatusId);
        }
    }
}
