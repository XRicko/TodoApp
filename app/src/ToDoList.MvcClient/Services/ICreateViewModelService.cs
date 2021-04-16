using System.Threading.Tasks;

using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.ViewModels;

namespace ToDoList.MvcClient.Services
{
    public interface ICreateViewModelService
    {
        Task<IndexViewModel> CreateIndexViewModelAsync(string categoryName = null, string statusName = null);
        Task<CreateTodoItemViewModel> CreateViewModelCreateOrUpdateTodoItemAsync(TodoItemModel todoItemModel);
    }
}
