using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using ToDoList.MvcClient.Resources.Validaton;

namespace ToDoList.MvcClient.Models
{
    [ExcludeFromCodeCoverage]
    public class UserModel : BaseModel
    {
        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(AnnotationResources))]
        [StringLength(
            100,
            ErrorMessageResourceName = "PasswordLengthError",
            ErrorMessageResourceType = typeof(AnnotationResources),
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        [Compare(
            "Password",
            ErrorMessageResourceName = "PasswordMatchingError",
            ErrorMessageResourceType = typeof(AnnotationResources))]
        public string ConfirmPassword { get; set; }
    }
}
