using System.Diagnostics.CodeAnalysis;

namespace ToDoList.MvcClient.Models
{
    [ExcludeFromCodeCoverage]
    public class ImageModel : BaseModel
    {
        public string Path { get; set; }
    }
}
