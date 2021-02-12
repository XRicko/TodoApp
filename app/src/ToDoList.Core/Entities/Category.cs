using System.Collections.Generic;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class Category : BaseEntity
    {
        public Category(string name) : base(name) { }

        public virtual ICollection<ChecklistItem> ChecklistItems { get; set; }
    }
}
