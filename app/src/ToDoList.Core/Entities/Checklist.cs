using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    [ExcludeFromCodeCoverage]
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
