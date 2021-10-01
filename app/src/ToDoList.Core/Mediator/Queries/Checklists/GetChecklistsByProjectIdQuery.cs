using System.Collections.Generic;

using MediatR;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Mediator.Queries.Checklists
{
    public record GetChecklistsByProjectIdQuery(int ProjectId) : IRequest<IEnumerable<ChecklistResponse>>;
}
