using System.ComponentModel.DataAnnotations;

namespace ToDoList.Core
{
    public class ApiOptions
    {
        public const string Apis = "Apis";

        [Required]
        [MinLength(30)]
        public string GoogleApiKey { get; init; }
    }
}
