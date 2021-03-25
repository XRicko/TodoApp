using System.Collections.Generic;
using System.Threading.Tasks;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Services
{
    public interface ICreateWithAddressService
    {
        Task<IEnumerable<TodoItemResponse>> GetItemsWithAddressAsync(IEnumerable<TodoItemResponse> todoItemResponses);
    }
}