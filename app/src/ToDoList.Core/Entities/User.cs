using System.Collections.Generic;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class User : BaseEntity
    {
        public User() : base()
        {
            Projects = new HashSet<Project>();
            RefreshTokens = new HashSet<RefreshToken>();
        }

        public string Password { get; set; }

        public ICollection<Project> Projects { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
