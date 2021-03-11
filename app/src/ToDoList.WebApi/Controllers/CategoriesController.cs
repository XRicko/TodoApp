using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Queries;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Response;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : Base
    {
        public CategoriesController(IMediator mediator, IMapper mapper) : base(mediator, mapper) { }

        [HttpGet]
        public async Task<IEnumerable<CategoryResponse>> Get() =>
            await Mediator.Send(new GetAllQuery<Category, CategoryResponse>());

        [HttpGet("{id}")]
        public async Task<CategoryResponse> Get(int id) =>
            await Mediator.Send(new GetByIdQuery<Category, CategoryResponse>(id));

        [HttpPost]
        public async Task Add([FromBody] CategoryCreateRequest createRequest) =>
            await Mediator.Send(new AddCommand<CategoryCreateRequest>(createRequest));

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await Mediator.Send(new RemoveCommand<Category>(id));
    }
}
