using System.Collections.Generic;
using System.Threading.Tasks;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Services
{
    public interface ICreateTodoItemResponseWithAddressService
    {
        Task<IEnumerable<TodoItemResponse>> GetTodoItemResponsesWithAddress(IEnumerable<TodoItemResponse> todoItemResponses);
        Task<TodoItemResponse> GetTodoItemResponseWithAddress(TodoItemResponse todoItemResponse);
    }
}