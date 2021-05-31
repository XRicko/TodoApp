using Microsoft.AspNetCore.Http;

using ToDoList.SharedClientLibrary.Models;

namespace ToDoList.MvcClient.Models
{
    public class TodoItemModelWithFile : TodoItemModel
    {
        public IFormFile Image { get; set; }
    }
}
