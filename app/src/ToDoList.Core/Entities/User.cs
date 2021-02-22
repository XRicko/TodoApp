using System.Collections.Generic;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class User : BaseEntity
    {
        public User() : base()
        {
            Checklists = new HashSet<Checklist>();
        }

        public virtual ICollection<Checklist> Checklists { get; set; }
    }
}
