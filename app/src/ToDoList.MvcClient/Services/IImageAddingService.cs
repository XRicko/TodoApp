using System.Threading.Tasks;

using ToDoList.MvcClient.Models;

namespace ToDoList.MvcClient.Services
{
    public interface IImageAddingService
    {
        Task AddImageInTodoItemAsync(TodoItemModel todoItem);
    }
}