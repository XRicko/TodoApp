using System.Collections.Generic;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class Checklist : BaseEntity
    {
        public Checklist(string name, int userId) : base(name)
        {
            UserId = userId;
        }

        public int UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<ChecklistItem> ChecklistItems { get; set; }
    }
}
