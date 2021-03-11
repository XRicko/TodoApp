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
using ToDoList.WebApi.Requests.Update;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChecklistsController : Base
    {
        public ChecklistsController(IMediator mediator, IMapper mapper) : base(mediator, mapper)
        {

        }

        [HttpGet]
        public async Task<IEnumerable<ChecklistResponse>> Get()
        {
            IEnumerable<Checklist> checklists = await mediator.Send(new GetAllQuery<Checklist>());
            return mapper.Map<IEnumerable<ChecklistResponse>>(checklists);
        }

        [HttpGet("{id}")]
        public async Task<ChecklistResponse> Get(int id)
        {
            Checklist checklist = await mediator.Send(new GetByIdQuery<Checklist>(id));
            return mapper.Map<ChecklistResponse>(checklist);
        }

        [HttpPost]
        public async Task Add([FromBody] ChecklistCreateRequest createRequest)
        {
            Checklist checklist = mapper.Map<Checklist>(createRequest);
            await mediator.Send(new AddCommand<Checklist>(checklist));
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await mediator.Send(new RemoveCommand<Checklist>(id));

        [HttpPut]
        public async Task Update([FromBody] ChecklistUpdateRequest updateRequest)
        {
            Checklist checklist = mapper.Map<Checklist>(updateRequest);
            await mediator.Send(new UpdateCommand<Checklist>(checklist));
        }
    }
}
