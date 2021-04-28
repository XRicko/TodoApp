using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    [ExcludeFromCodeCoverage]
    public class Image : BaseEntity
    {
        public string Path { get; set; }

        public Image() : base()
        {
            TodoItems = new HashSet<TodoItem>();
        }

        public ICollection<TodoItem> TodoItems { get; set; }
    }
}
