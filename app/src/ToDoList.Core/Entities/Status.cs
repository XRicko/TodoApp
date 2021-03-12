using System.Collections.Generic;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class Status : BaseEntity
    {
        public Status() : base()
        {
            TodoItems = new HashSet<TodoItem>();
        }

        public virtual ICollection<TodoItem> TodoItems { get; set; }
    }
}
