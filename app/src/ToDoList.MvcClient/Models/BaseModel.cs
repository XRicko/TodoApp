using System.ComponentModel.DataAnnotations;

namespace ToDoList.MvcClient.Models
{
    public class BaseModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Provide a name")]
        public string Name { get; set; }
    }
}
