using MediatR;

namespace ToDoList.Core.Mediator.Commands.RefreshTokens
{
    public record RemoveAllRefreshTokensFromUserCommand(int UserId) : IRequest;
}
