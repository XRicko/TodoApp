using ToDoList.SharedKernel;

namespace ToDoList.MvcClient.Models
{
    public class ChecklistModel : BaseEntity
    {
        public int UserId { get; set; }

        public string UserName { get; set; }
    }
}
