using MediatR;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Mediator.Queries.Checklists
{
    public record GetChecklistByNameAndUserIdQuery(string Name, int UserId) : IRequest<ChecklistResponse>;
}
