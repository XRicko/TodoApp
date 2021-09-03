using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

using ToDoList.Core.Mediator.Response;
using ToDoList.Extensions;

namespace ToDoList.Core.Services
{
    internal class AddressService : IAddressService
    {
        private readonly IGeocodingService geocodingService;
        private readonly IDistributedCache cache;

        public AddressService(IGeocodingService service, IDistributedCache distributedCache)
        {
            geocodingService = service ?? throw new ArgumentNullException(nameof(service));
            cache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
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

            if (todoItemResponse.GeoPoint is null)
                return todoItemResponse;

            string recordKey = $"Address_{todoItemResponse.GeoPoint.Latitude}_{todoItemResponse.GeoPoint.Longitude}";
            string address = await cache.GetRecordAsync<string>(recordKey);

            if (address is null)
            {
                address = await geocodingService.GetAddressAsync(todoItemResponse.GeoPoint.Latitude, todoItemResponse.GeoPoint.Longitude);
                await cache.SetRecordAsync(recordKey, address, TimeSpan.FromDays(3));
            }

            return todoItemResponse with { Address = address };
        }
    }
}
