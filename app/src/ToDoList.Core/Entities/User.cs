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

        public ICollection<Checklist> Checklists { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
