using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Services
{
    public class CreateWithAddressService : ICreateWithAddressService
    {
        private readonly IGeocodingService geocodingService;

        public CreateWithAddressService(IGeocodingService service)
        {
            geocodingService = service;
        }

        public async Task<IEnumerable<TodoItemResponse>> GetItemsWithAddressAsync(IEnumerable<TodoItemResponse> todoItemResponses)
        {
            if (todoItemResponses is null)
                throw new ArgumentNullException(nameof(todoItemResponses));

            List<TodoItemResponse> todoItemResponsesWithAddress = new();

            foreach (var item in todoItemResponses)
            {
                todoItemResponsesWithAddress.Add(await GetItemWithAddressAsync(item));
            }

            return todoItemResponsesWithAddress;
        }

        public async Task<TodoItemResponse> GetItemWithAddressAsync(TodoItemResponse todoItemResponse)
        {
            _ = todoItemResponse ?? throw new ArgumentNullException(nameof(todoItemResponse));

            //if (todoItemResponse.GeoPoint is not null)
            //{
            //    string address = await geocodingService.GetAddressAsync(todoItemResponse.GeoPoint.Latitude, todoItemResponse.GeoPoint.Longitude);
            //    var todoItemResponseWithAddress = todoItemResponse with { Address = address };

            //    return todoItemResponseWithAddress;
            //}

            return todoItemResponse;
        }
    }
}
