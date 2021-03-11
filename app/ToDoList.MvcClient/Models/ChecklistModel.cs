using ToDoList.SharedKernel;

namespace ToDoList.MvcClient.Models
{
    public class ChecklistModel : BaseModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }
    }
}
