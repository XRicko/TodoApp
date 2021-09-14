using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.RefreshTokens;
using ToDoList.Core.Mediator.Handlers.RefreshTokens;

using Xunit;

namespace Core.Tests.Handlers.RefreshTokens
{
    public class RemoveAllRefreshTokensFromUserCommandHandlerTests : HandlerBaseForTests
    {
        private readonly RemoveAllRefreshTokensFromUserCommandHandler removeAllRefreshTokensFromUserHandler;

        public RemoveAllRefreshTokensFromUserCommandHandlerTests() : base()
        {
            removeAllRefreshTokensFromUserHandler = new RemoveAllRefreshTokensFromUserCommandHandler(UnitOfWorkMock.Object, Mapper);
        }

        [Fact]
        public async Task Handle_DeletesAllRefreshTokensGivenUserId()
        {
            // Arrange
            int userId = 69;

            var refreshTokens = new List<RefreshToken>
            {
                new RefreshToken { Id = 12, Name = "fnuognweoijef84", UserId = userId },
                new RefreshToken { Id = 9, Name = "erivrehpe(#UR0jf4qiu", UserId = 420 },
                new RefreshToken { Id = 1, Name = "rnipegnp3g43ewjfmc", UserId = userId },
                new RefreshToken { Id = 90, Name = "vnirvpWKDPEJF402f9j290fj", UserId = 789 }
            };

            var refreshTokensQueryable = refreshTokens.AsQueryable();

            RepoMock.Setup(x => x.GetAll<RefreshToken>())
                    .Returns(refreshTokensQueryable)
                    .Verifiable();

            // Act
            await removeAllRefreshTokensFromUserHandler.Handle(new RemoveAllRefreshTokensFromUserCommand(userId),
                                                               new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Remove(It.IsAny<RefreshToken>()),
                            Times.Exactly(refreshTokens.Count(i => i.UserId == userId)));

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
        }
    }
}
