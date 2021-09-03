using System.Collections.Generic;
using System.Threading.Tasks;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Services
{
    public interface IAddressService
    {
        Task<IEnumerable<TodoItemResponse>> GetItemsWithAddressAsync(IEnumerable<TodoItemResponse> todoItemResponses);
        Task<TodoItemResponse> GetItemWithAddressAsync(TodoItemResponse todoItemResponse);
    }
}