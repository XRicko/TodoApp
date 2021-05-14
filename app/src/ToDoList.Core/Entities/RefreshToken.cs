
using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class RefreshToken : BaseEntity
    {
        public int UserId { get; set; }

        public User User { get; set; }
    }
}
