using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Queries.TodoItems;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.Core.Mediator.Response;
using ToDoList.Extensions;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TodoItemsController : Base
    {
        private readonly IDistributedCache cache;

        private int UserId => Convert.ToInt32(User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
        private string RecordKey => $"TodoItems_User_{UserId}";

        public TodoItemsController(IMediator mediator, IDistributedCache distributedCache) : base(mediator)
        {
            cache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        [HttpGet]
        public async Task<IEnumerable<TodoItemResponse>> Get()
        {
            var todoItems = await cache.GetRecordAsync<IEnumerable<TodoItemResponse>>(RecordKey);

            if (todoItems is null)
            {
                todoItems = await Mediator.Send(new GetTodoItemsByUserIdQuery(UserId));
                await cache.SetRecordAsync(RecordKey, todoItems);
            }

            return todoItems;
        }

        [HttpGet("{id}")]
        public async Task<TodoItemResponse> Get(int id) =>
            await Mediator.Send(new GetByIdQuery<TodoItem, TodoItemResponse>(id));

        [HttpPost]
        public async Task Add([FromBody] TodoItemCreateRequest createRequest)
        {
            _ = createRequest ?? throw new ArgumentNullException(nameof(createRequest));

            await Mediator.Send(new AddCommand<TodoItemCreateRequest>(createRequest));
            await cache.RemoveAsync(RecordKey);
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await Mediator.Send(new RemoveCommand<TodoItem>(id));
            await cache.RemoveAsync(RecordKey);
        }

        [HttpPut]
        public async Task Update([FromBody] TodoItemUpdateRequest updateRequest)
        {
            _ = updateRequest ?? throw new ArgumentNullException(nameof(updateRequest));

            await Mediator.Send(new UpdateCommand<TodoItemUpdateRequest>(updateRequest));
            await cache.RemoveAsync(RecordKey);
        }
    }
}
