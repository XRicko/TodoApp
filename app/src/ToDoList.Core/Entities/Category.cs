using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    [ExcludeFromCodeCoverage]
    public class Category : BaseEntity
    {
        public Category() : base()
        {
            TodoItems = new HashSet<TodoItem>();
        }

        public ICollection<TodoItem> TodoItems { get; set; }
    }
}
