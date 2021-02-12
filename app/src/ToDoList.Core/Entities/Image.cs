using System.Collections.Generic;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class Image : BaseEntity
    {
        public string Path { get; set; }

        public Image(string name, string path) : base(name)
        {
            Path = path;
        }

        public virtual ICollection<ChecklistItem> ChecklistItems { get; set; }
    }
}
