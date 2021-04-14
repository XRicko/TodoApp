using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using ToDoList.MvcClient.Resources.Validaton;

namespace ToDoList.MvcClient.Models
{
    [ExcludeFromCodeCoverage]
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
