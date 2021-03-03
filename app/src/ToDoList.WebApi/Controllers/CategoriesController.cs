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

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public CategoriesController(IMediator m, IMapper map)
        {
            mediator = m;
            mapper = map;
        }

        [HttpGet]
        public async Task<IEnumerable<CategoryResponse>> Get()
        {
            IEnumerable<Category> categories = await mediator.Send(new GetAllQuery<Category>());
            return mapper.Map<IEnumerable<CategoryResponse>>(categories);
        }

        [HttpGet("{id}")]
        public async Task<CategoryResponse> Get(int id)
        {
            Category category = await mediator.Send(new GetByIdQuery<Category>(id));
            return mapper.Map<CategoryResponse>(category);
        }

        [HttpPost]
        public async Task Add([FromBody] CategoryCreateRequest createRequest)
        {
            Category category = mapper.Map<Category>(createRequest);
            await mediator.Send(new AddCommand<Category>(category));
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await mediator.Send(new RemoveCommand<Category>(id));
    }
}
