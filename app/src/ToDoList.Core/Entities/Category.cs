using System.Collections.Generic;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class Category : BaseEntity
    {
        public Category() : base()
        {
            ChecklistItems = new HashSet<ChecklistItem>();
        }

        public virtual ICollection<ChecklistItem> ChecklistItems { get; set; }
    }
}
