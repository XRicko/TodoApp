using System.ComponentModel.DataAnnotations;

using ToDoList.SharedClientLibrary.Resources.Validaton;

namespace ToDoList.SharedClientLibrary.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }

        [Required(
            ErrorMessageResourceName = "NameError",
            ErrorMessageResourceType = typeof(AnnotationResources))]
        [Display(
            Name = "Name",
            ResourceType = typeof(AnnotationResources))]
        public string Name { get; set; }
    }
}
