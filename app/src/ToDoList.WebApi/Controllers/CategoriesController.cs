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
    public class CategoriesController : Base
    {
        public CategoriesController(IMediator mediator, IMapper mapper) : base(mediator, mapper)
        {

        }

        [HttpGet]
        public async Task<IEnumerable<CategoryResponse>> Get()
        {
            IEnumerable<Category> categories = await Mediator.Send(new GetAllQuery<Category>());
            return Mapper.Map<IEnumerable<CategoryResponse>>(categories);
        }

        [HttpGet("{id}")]
        public async Task<CategoryResponse> Get(int id)
        {
            Category category = await Mediator.Send(new GetByIdQuery<Category>(id));
            return Mapper.Map<CategoryResponse>(category);
        }

        [HttpPost]
        public async Task Add([FromBody] CategoryCreateRequest createRequest)
        {
            Category category = Mapper.Map<Category>(createRequest);
            await Mediator.Send(new AddCommand<Category>(category));
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await Mediator.Send(new RemoveCommand<Category>(id));
    }
}
