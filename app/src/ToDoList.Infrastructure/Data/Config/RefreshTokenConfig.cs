using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ToDoList.Core.Entities;

namespace ToDoList.Infrastructure.Data.Config
{
    internal class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshToken");

            builder.Property(e => e.Name)
                   .HasColumnName("Token")
                   .IsRequired()
                   .HasMaxLength(255);

            builder.HasOne(e => e.User)
                   .WithMany(u => u.RefreshTokens)
                   .HasForeignKey(e => e.UserId);
        }
    }
}
