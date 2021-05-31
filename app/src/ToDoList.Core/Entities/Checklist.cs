using System.Collections.Generic;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class Checklist : BaseEntity
    {
        public int UserId { get; set; }

        public Checklist() : base()
        {
            TodoItems = new HashSet<TodoItem>();
        }

        public User User { get; set; }
        public ICollection<TodoItem> TodoItems { get; set; }
    }
}
