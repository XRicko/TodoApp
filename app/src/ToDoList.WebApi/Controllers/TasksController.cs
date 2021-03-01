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
    public class TasksController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public TasksController(IMediator m, IMapper map)
        {
            mediator = m;
            mapper = map;
        }

        [HttpGet]
        public async Task<IEnumerable<ChecklistItemDTO>> Get()
        {
            IEnumerable<ChecklistItem> checklistItems = await mediator.Send(new GetAllQuery<ChecklistItem>());
            return mapper.Map<IEnumerable<ChecklistItemDTO>>(checklistItems);
        }

        [HttpGet("{id}")]
        public async Task<ChecklistItemDTO> Get(int id)
        {
            ChecklistItem checklistItem = await mediator.Send(new GetByIdQuery<ChecklistItem>(id));
            return mapper.Map<ChecklistItemDTO>(checklistItem);
        }

        [HttpPost]
        public async Task Add([FromBody] ChecklistItemDTO checklistItemDTO)
        {
            ChecklistItem checklistItem = mapper.Map<ChecklistItem>(checklistItemDTO);
            await mediator.Send(new AddCommand<ChecklistItem>(checklistItem));
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await mediator.Send(new RemoveCommand<ChecklistItem>(id));

        [HttpPut]
        public async Task Update([FromBody] ChecklistItemDTO checklistItemDTO)
        {
            ChecklistItem checklistItem = mapper.Map<ChecklistItem>(checklistItemDTO);
            await mediator.Send(new UpdateCommand<ChecklistItem>(checklistItem));
        }
    }
}
