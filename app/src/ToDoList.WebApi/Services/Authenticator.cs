using System;
using System.Threading.Tasks;

using MediatR;

using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Jwt.Models;

namespace ToDoList.WebApi.Services
{
    public class Authenticator : IAuthenticator
    {
        private readonly ITokenGenerator tokenGenerator;
        private readonly IMediator mediator;

        public Authenticator(ITokenGenerator generator, IMediator mediatr)
        {
            tokenGenerator = generator ?? throw new ArgumentNullException(nameof(generator));
            mediator = mediatr ?? throw new ArgumentNullException(nameof(mediatr));
        }

        public async Task<AuthenticatedResponse> AuthenticateAsync(UserResponse user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));

            string accessToken = tokenGenerator.GenerateAccessToken(user);
            string refreshToken = tokenGenerator.GenerateRefreshToken();

            RefreshTokenCreateRequest createRequest = new(refreshToken, user.Id);
            await mediator.Send(new AddCommand<RefreshTokenCreateRequest>(createRequest));

            return new AuthenticatedResponse(accessToken, refreshToken);
        }
    }
}
