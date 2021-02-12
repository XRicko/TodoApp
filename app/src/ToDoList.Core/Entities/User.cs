using System.Collections.Generic;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class User : BaseEntity
    {
        public User(string name) : base(name) { }

        public virtual ICollection<Checklist> Checklists { get; set; }
    }
}
