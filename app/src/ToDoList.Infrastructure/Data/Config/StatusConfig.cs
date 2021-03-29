using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ToDoList.Core.Entities;

namespace ToDoList.Infrastructure.Data.Config
{
    [ExcludeFromCodeCoverage]
    internal class StatusConfig : IEntityTypeConfiguration<Status>
    {
        public void Configure(EntityTypeBuilder<Status> builder)
        {
            builder.ToTable("Status");

            builder.HasIndex(e => e.Name)
                .IsUnique();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(75);

            builder.HasData(new Status { Id = 1, Name = "Planned", IsDone = false },
                            new Status { Id = 2, Name = "Ongoing", IsDone = false },
                            new Status { Id = 3, Name = "Done", IsDone = true });
        }
    }
}
