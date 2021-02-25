using System.Collections.Generic;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class Status : BaseEntity
    {
        public Status() : base()
        {
            ChecklistItems = new HashSet<ChecklistItem>();
        }

        public virtual ICollection<ChecklistItem> ChecklistItems { get; set; }
    }
}
