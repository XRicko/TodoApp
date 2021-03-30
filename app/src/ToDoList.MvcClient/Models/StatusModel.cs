using System.Diagnostics.CodeAnalysis;

namespace ToDoList.MvcClient.Models
{
    [ExcludeFromCodeCoverage]
    public class StatusModel : BaseModel
    {
        public bool IsDone { get; set; }
    }
}
