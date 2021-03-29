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

        public string Password { get; set; }

        public virtual ICollection<Checklist> Checklists { get; set; }
    }
}
