using System.Collections.Generic;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class Project : BaseEntity
    {
        public int UserId { get; set; }

        public Project()
        {
            Checklists = new HashSet<Checklist>();
        }

        public User User { get; set; }
        public ICollection<Checklist> Checklists { get; set; }
    }
}
