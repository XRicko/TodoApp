using System.Collections.Generic;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class Category : BaseEntity
    {
        public Category() : base()
        {
            TodoItems = new HashSet<TodoItem>();
        }

        public ICollection<TodoItem> TodoItems { get; set; }
    }
}
