using System.Collections.Generic;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Commands;
using ToDoList.Core.Entities;
using ToDoList.Core.Queries;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator mediator;

        public CategoriesController(IMediator m)
        {
            mediator = m;
        }

        [HttpGet]
        public async Task<IEnumerable<Category>> Get() =>
            await mediator.Send(new GetAllQuery<Category>());

        [HttpGet("{id}")]
        public async Task<Category> Get(int id) =>
            await mediator.Send(new GetByIdQuery<Category>(id));

        [HttpPost]
        public async Task Add([FromBody] Category category) =>
            await mediator.Send(new AddCommand<Category>(category));

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await mediator.Send(new RemoveCommand<Category>(id));
    }
}
