using System.ComponentModel.DataAnnotations;

namespace ToDoList.MvcClient.API
{
    public class WebApiConfig
    {
        [Required(AllowEmptyStrings = false)]
        public string BaseUrl { get; set; }
    }
}
