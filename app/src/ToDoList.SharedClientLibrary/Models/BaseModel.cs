using System.ComponentModel.DataAnnotations;

using ToDoList.SharedClientLibrary.Resources;

namespace ToDoList.SharedClientLibrary.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "NameError", ErrorMessageResourceType = typeof(Annotations))]
        [MinLength(2)]
        [Display(Name = "Name", ResourceType = typeof(Annotations))]
        public string Name { get; set; }
    }
}
