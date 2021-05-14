
using MediatR;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Mediator.Queries.RefreshTokens
{
    public record GetRefreshTokenByTokenQuery(string Token) : IRequest<RefreshTokenResponse>;
}
