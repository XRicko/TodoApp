using System.Collections.Generic;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class Image : BaseEntity
    {
        public string Path { get; set; }

        public Image() : base()
        {
            TodoItems = new HashSet<TodoItem>();
        }

        public virtual ICollection<TodoItem> TodoItems { get; set; }
    }
}
