using System.ComponentModel.DataAnnotations;

namespace ToDoList.Core
{
    internal class ApiKeys
    {
        public const string Apis = "Apis";

        [Required]
        [MinLength(30)]
        public string GoogleApiKey { get; init; }
    }
}
