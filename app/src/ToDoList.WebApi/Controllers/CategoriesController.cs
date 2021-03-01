using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Commands;
using ToDoList.Core.DTOs;
using ToDoList.Core.Entities;
using ToDoList.Core.Queries;

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
        public async Task<IEnumerable<CategoryDTO>> Get()
        {
            IEnumerable<Category> categories = await mediator.Send(new GetAllQuery<Category>());
            return mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        [HttpGet("{id}")]
        public async Task<CategoryDTO> Get(int id)
        {
            Category category = await mediator.Send(new GetByIdQuery<Category>(id));
            return mapper.Map<CategoryDTO>(category);
        }

        [HttpPost]
        public async Task Add([FromBody] CategoryDTO categoryDTO)
        {
            Category category = mapper.Map<Category>(categoryDTO);
            await mediator.Send(new AddCommand<Category>(category));
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await mediator.Send(new RemoveCommand<Category>(id));
    }
}
