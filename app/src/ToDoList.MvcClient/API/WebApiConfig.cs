using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ToDoList.MvcClient.API
{
    [ExcludeFromCodeCoverage]
    public class WebApiConfig
    {
        [Required(AllowEmptyStrings = false)]
        public string BaseUrl { get; set; }
    }
}
