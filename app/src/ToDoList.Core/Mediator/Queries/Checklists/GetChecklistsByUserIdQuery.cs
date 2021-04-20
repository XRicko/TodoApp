using System.Collections.Generic;

using MediatR;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Mediator.Queries.Checklists
{
    public record GetChecklistsByUserIdQuery(int UserId) : IRequest<IEnumerable<ChecklistResponse>>;
}
