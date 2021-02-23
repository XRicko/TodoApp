using System.Collections.Generic;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class Checklist : BaseEntity
    {
        public int UserId { get; set; }

        public Checklist() : base()
        {
            ChecklistItems = new HashSet<ChecklistItem>();
        }

        public virtual User User { get; set; }
        public virtual ICollection<ChecklistItem> ChecklistItems { get; set; }
    }
}
