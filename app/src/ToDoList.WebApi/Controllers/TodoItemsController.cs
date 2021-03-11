using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Commands;
using ToDoList.Core.Entities;
using ToDoList.Core.Queries;
using ToDoList.Core.Response;
using ToDoList.WebApi.Requests.Create;
using ToDoList.WebApi.Requests.Update;
using ToDoList.WebApi.Services;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : Base
    {
        private readonly GeocodingService geocodingService;

        public TodoItemsController(IMediator mediator, IMapper mapper, GeocodingService codingService) : base(mediator, mapper)
        {
            geocodingService = codingService;
        }

        [HttpGet]
        public async Task<IEnumerable<TodoItemResponse>> Get()
        {
            IEnumerable<TodoItem> todoItems = await Mediator.Send(new GetAllQuery<TodoItem>());
            IEnumerable<TodoItemResponse> todoItemResponses = Mapper.Map<IEnumerable<TodoItemResponse>>(todoItems);

            IEnumerable<TodoItemResponse> todoItemResponsesWithAddress = await GetTodoItemResponsesWithAddress(todoItemResponses);

            return todoItemResponsesWithAddress;
        }

        [HttpGet("{id}")]
        public async Task<TodoItemResponse> Get(int id)
        {
            TodoItem todoItem = await Mediator.Send(new GetByIdQuery<TodoItem>(id));
            TodoItemResponse todoItemResponse = Mapper.Map<TodoItemResponse>(todoItem);

            TodoItemResponse todoItemResponseWithAddress = await GetTodoItemResponseWithAddress(todoItemResponse);

            return todoItemResponseWithAddress;
        }

        [HttpPost]
        public async Task Add([FromBody] TodoItemCreateRequest createRequest)
        {
            TodoItem todoItem = Mapper.Map<TodoItem>(createRequest);
            todoItem.GeoPoint.SRID = 4326;

            await Mediator.Send(new AddCommand<TodoItem>(todoItem));
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await Mediator.Send(new RemoveCommand<TodoItem>(id));

        [HttpPut]
        public async Task Update([FromBody] TodoItemUpdateRequest updateRequest)
        {
            TodoItem todoItem = Mapper.Map<TodoItem>(updateRequest);
            await Mediator.Send(new UpdateCommand<TodoItem>(todoItem));
        }

        private async Task<TodoItemResponse> GetTodoItemResponseWithAddress(TodoItemResponse todoItemResponse)
        {
            string address = await geocodingService.GetAddressAsync(todoItemResponse.GeoPoint.Latitude, todoItemResponse.GeoPoint.Longitude);
            TodoItemResponse todoItemResponseWithAddress = todoItemResponse with { Address = address };

            return todoItemResponseWithAddress;
        }

        private async Task<IEnumerable<TodoItemResponse>> GetTodoItemResponsesWithAddress(IEnumerable<TodoItemResponse> todoItemResponses)
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
