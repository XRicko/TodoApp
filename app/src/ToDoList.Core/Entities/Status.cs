using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    [ExcludeFromCodeCoverage]
    public class Status : BaseEntity
    {
        public bool IsDone { get; set; }

        public Status() : base()
        {
            TodoItems = new HashSet<TodoItem>();
        }

        public virtual ICollection<TodoItem> TodoItems { get; set; }
    }
}
