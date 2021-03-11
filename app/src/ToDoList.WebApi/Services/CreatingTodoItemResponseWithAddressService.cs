using System.Collections.Generic;
using System.Threading.Tasks;

using ToDoList.Core.Response;

namespace ToDoList.WebApi.Services
{
    public class CreateTodoItemResponseWithAddressService
    {
        private readonly GeocodingService geocodingService;

        public CreateTodoItemResponseWithAddressService(GeocodingService service)
        {
            geocodingService = service;
        }

        public async Task<TodoItemResponse> GetTodoItemResponseWithAddress(TodoItemResponse todoItemResponse)
        {
            if (todoItemResponse.GeoPoint is not null)
            {
                string address = await geocodingService.GetAddressAsync(todoItemResponse.GeoPoint.Latitude, todoItemResponse.GeoPoint.Longitude);
                TodoItemResponse todoItemResponseWithAddress = todoItemResponse with { Address = address };

                return todoItemResponseWithAddress;
            }

            return todoItemResponse;
        }

        public async Task<IEnumerable<TodoItemResponse>> GetTodoItemResponsesWithAddress(IEnumerable<TodoItemResponse> todoItemResponses)
        {
            List<TodoItemResponse> todoItemResponsesWithAddress = new();

            foreach (var item in todoItemResponses)
            {
                if (item.GeoPoint is not null)
                    todoItemResponsesWithAddress.Add(await GetTodoItemResponseWithAddress(item));
                else
                    todoItemResponsesWithAddress.Add(item);
            }

            return todoItemResponsesWithAddress;
        }
    }
}
