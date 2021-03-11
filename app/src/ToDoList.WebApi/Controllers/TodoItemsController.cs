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
        private readonly CreateTodoItemResponseWithAddressService addressService;

        public TodoItemsController(IMediator mediator, IMapper mapper, CreateTodoItemResponseWithAddressService createService) : base(mediator, mapper)
        {
            addressService = createService;
        }

        [HttpGet]
        public async Task<IEnumerable<TodoItemResponse>> Get()
        {
            var todoItems = await Mediator.Send(new GetAllQuery<TodoItem>());
            var todoItemResponses = Mapper.Map<IEnumerable<TodoItemResponse>>(todoItems);

            var todoItemResponsesWithAddress = await addressService.GetTodoItemResponsesWithAddress(todoItemResponses);

            return todoItemResponsesWithAddress;
        }

        [HttpGet("{id}")]
        public async Task<TodoItemResponse> Get(int id)
        {
            TodoItem todoItem = await Mediator.Send(new GetByIdQuery<TodoItem>(id));
            TodoItemResponse todoItemResponse = Mapper.Map<TodoItemResponse>(todoItem);

            TodoItemResponse todoItemResponseWithAddress = await addressService.GetTodoItemResponseWithAddress(todoItemResponse);

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
    }
}
