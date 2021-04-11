using System.Diagnostics.CodeAnalysis;

namespace ToDoList.MvcClient.Models
{
    [ExcludeFromCodeCoverage]
    public class ChecklistModel : BaseModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }
    }
}
