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

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public TodoItemsController(IMediator m, IMapper map)
        {
            mediator = m;
            mapper = map;
        }

        [HttpGet]
        public async Task<IEnumerable<TodoItemResponse>> Get()
        {
            IEnumerable<TodoItem> todoItems = await mediator.Send(new GetAllQuery<TodoItem>());
            return mapper.Map<IEnumerable<TodoItemResponse>>(todoItems);
        }

        [HttpGet("{id}")]
        public async Task<TodoItemResponse> Get(int id)
        {
            TodoItem todoItem = await mediator.Send(new GetByIdQuery<TodoItem>(id));
            return mapper.Map<TodoItemResponse>(todoItem);
        }

        [HttpPost]
        public async Task Add([FromBody] TodoItemCreateRequest createRequest)
        {
            TodoItem todoItem = mapper.Map<TodoItem>(createRequest);
            todoItem.GeoPoint.SRID = 4326;

            await mediator.Send(new AddCommand<TodoItem>(todoItem));
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await mediator.Send(new RemoveCommand<TodoItem>(id));

        [HttpPut]
        public async Task Update([FromBody] TodoItemUpdateRequest updateRequest)
        {
            TodoItem todoItem = mapper.Map<TodoItem>(updateRequest);
            await mediator.Send(new UpdateCommand<TodoItem>(todoItem));
        }
    }
}
