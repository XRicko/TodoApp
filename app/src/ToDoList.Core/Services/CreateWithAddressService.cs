﻿using System.Collections.Generic;
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

        public async Task<TodoItemResponse> GetItemWithAddressAsync(TodoItemResponse todoItemResponse)
        {
            if (todoItemResponse.GeoPoint is not null)
            {
                string address = await geocodingService.GetAddressAsync(todoItemResponse.GeoPoint.Latitude, todoItemResponse.GeoPoint.Longitude);
                var todoItemResponseWithAddress = todoItemResponse with { Address = address };

                return todoItemResponseWithAddress;
            }

            return todoItemResponse;
        }

        public async Task<IEnumerable<TodoItemResponse>> GetItemsWithAddressAsync(IEnumerable<TodoItemResponse> todoItemResponses)
        {
            List<TodoItemResponse> todoItemResponsesWithAddress = new();

            foreach (var item in todoItemResponses)
            {
                todoItemResponsesWithAddress.Add(await GetItemWithAddressAsync(item));
            }

            return todoItemResponsesWithAddress;
        }
    }
}