﻿using System.Collections.Generic;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.Core.Mediator.Response;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : Base
    {
        public TodoItemsController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IEnumerable<TodoItemResponse>> Get() =>
            await Mediator.Send(new GetAllQuery<TodoItem, TodoItemResponse>());

        [HttpGet("{id}")]
        public async Task<TodoItemResponse> Get(int id) =>
            await Mediator.Send(new GetByIdQuery<TodoItem, TodoItemResponse>(id));

        [HttpPost]
        public async Task Add([FromBody] TodoItemCreateRequest createRequest) =>
            await Mediator.Send(new AddCommand<TodoItemCreateRequest>(createRequest));

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await Mediator.Send(new RemoveCommand<TodoItem>(id));

        [HttpPut]
        public async Task Update([FromBody] TodoItemUpdateRequest updateRequest) =>
            await Mediator.Send(new UpdateCommand<TodoItemUpdateRequest>(updateRequest));
    }
}
