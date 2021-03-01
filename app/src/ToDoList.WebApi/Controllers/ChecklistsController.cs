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
    public class ChecklistsController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public ChecklistsController(IMediator m, IMapper map)
        {
            mediator = m;
            mapper = map;
        }

        [HttpGet]
        public async Task<IEnumerable<ChecklistDTO>> Get()
        {
            IEnumerable<Checklist> checklists = await mediator.Send(new GetAllQuery<Checklist>());
            return mapper.Map<IEnumerable<ChecklistDTO>>(checklists);
        }

        [HttpGet("{id}")]
        public async Task<ChecklistDTO> Get(int id)
        {
            Checklist checklist = await mediator.Send(new GetByIdQuery<Checklist>(id));
            return mapper.Map<ChecklistDTO>(checklist);
        }

        [HttpPost]
        public async Task Add([FromBody] ChecklistDTO checklistDTO)
        {
            Checklist checklist = mapper.Map<Checklist>(checklistDTO);
            await mediator.Send(new AddCommand<Checklist>(checklist));
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await mediator.Send(new RemoveCommand<Checklist>(id));

        [HttpPut]
        public async Task Update([FromBody] ChecklistDTO checklistDTO)
        {
            Checklist checklist = mapper.Map<Checklist>(checklistDTO);
            await mediator.Send(new UpdateCommand<Checklist>(checklist));
        }
    }
}
