using System.ComponentModel.DataAnnotations;

using ToDoList.SharedClientLibrary.Resources;

namespace ToDoList.SharedClientLibrary.Models
{
    public class UserModel : BaseModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Annotations))]
        [StringLength(maximumLength: 100, MinimumLength = 6, 
            ErrorMessageResourceName = "PasswordLengthError",
            ErrorMessageResourceType = typeof(Annotations))]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Annotations))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(Annotations))]
        [Compare("Password", 
            ErrorMessageResourceName = "PasswordMatchingError",
            ErrorMessageResourceType = typeof(Annotations))]
        public string ConfirmPassword { get; set; }
    }
}
