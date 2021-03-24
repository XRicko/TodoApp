using System.Collections.Generic;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Response;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : Base
    {
        public CategoriesController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IEnumerable<CategoryResponse>> Get() =>
            await Mediator.Send(new GetAllQuery<Category, CategoryResponse>());

        [HttpGet]
        [Route("[action]/{name}")]
        public async Task<CategoryResponse> GetByName(string name) =>
           await Mediator.Send(new GetByNameQuery<Category, CategoryResponse>(name));

        [HttpPost]
        public async Task Add([FromBody] CategoryCreateRequest createRequest) =>
            await Mediator.Send(new AddCommand<CategoryCreateRequest>(createRequest));
    }
}
