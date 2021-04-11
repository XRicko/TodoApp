using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ToDoList.Core
{
    [ExcludeFromCodeCoverage]
    public class ApiOptions
    {
        public const string Apis = "Apis";

        [Required]
        [MinLength(30)]
        public string GoogleApiKey { get; init; }
    }
}
