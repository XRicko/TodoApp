using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Configuration;

namespace ToDoList.Extensions
{
    public static class ConfigValidationExtensions
    {
        public static T GetValid<T>(this IConfiguration configuration)
        {
            var obj = configuration.Get<T>();
            Validator.ValidateObject(obj, new ValidationContext(obj), true);

            return obj;
        }
    }
}
