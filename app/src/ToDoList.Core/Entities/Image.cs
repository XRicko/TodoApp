using System.Collections.Generic;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class Image : BaseEntity
    {
        public string Path { get; set; }

        public Image() : base()
        {
            ChecklistItems = new HashSet<ChecklistItem>();
        }

        public virtual ICollection<ChecklistItem> ChecklistItems { get; set; }
    }
}
