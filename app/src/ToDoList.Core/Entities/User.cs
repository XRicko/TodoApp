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

        public ICollection<Checklist> Checklists { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
