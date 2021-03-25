using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    [ExcludeFromCodeCoverage]
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
