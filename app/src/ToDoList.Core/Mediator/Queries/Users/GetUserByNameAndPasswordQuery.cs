
using MediatR;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Mediator.Queries.Users
{
    public record GetUserByNameAndPasswordQuery(string Name, string Password) : IRequest<UserResponse>;
}
