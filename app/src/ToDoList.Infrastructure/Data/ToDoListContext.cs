using Microsoft.EntityFrameworkCore;

using ToDoList.Core.Entities;
using ToDoList.Infrastructure.Data.Config;

namespace ToDoList.Infrastructure.Data
{
    public class ToDoListContext : DbContext
    {
        public ToDoListContext(DbContextOptions<ToDoListContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Checklist> Checklists { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<ChecklistItem> ChecklistItems { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ImageConfig());
            modelBuilder.ApplyConfiguration(new CategoryConfig());
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new StatusConfig());
            modelBuilder.ApplyConfiguration(new ChecklistConfig());
            modelBuilder.ApplyConfiguration(new ChecklistConfig());
            modelBuilder.ApplyConfiguration(new ChecklistItemConfig());
        }
    }
}
