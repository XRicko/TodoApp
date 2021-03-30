using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ToDoList.MvcClient.Models
{
    [ExcludeFromCodeCoverage]
    public class BaseModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Provide a name")]
        public string Name { get; set; }
    }
}
