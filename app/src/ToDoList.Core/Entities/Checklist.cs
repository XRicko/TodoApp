using System.Collections.Generic;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class Checklist : BaseEntity
    {
        public int ProjectId { get; set; }

        public Checklist() : base()
        {
            TodoItems = new HashSet<TodoItem>();
        }

        public Project Project { get; set; }
        public ICollection<TodoItem> TodoItems { get; set; }
    }
}
