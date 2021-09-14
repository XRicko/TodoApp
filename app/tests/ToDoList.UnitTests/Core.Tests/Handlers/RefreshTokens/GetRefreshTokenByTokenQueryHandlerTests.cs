using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.RefreshTokens;
using ToDoList.Core.Mediator.Queries.RefreshTokens;
using ToDoList.Core.Mediator.Response;

using Xunit;

namespace Core.Tests.Handlers.RefreshTokens
{
    public class GetRefreshTokenByTokenQueryHandlerTests : HandlerBaseForTests
    {
        private readonly GetRefreshTokenByTokenQueryHandler getRefreshTokenByTokenHandler;

        public GetRefreshTokenByTokenQueryHandlerTests() : base()
        {
            getRefreshTokenByTokenHandler = new GetRefreshTokenByTokenQueryHandler(UnitOfWorkMock.Object, Mapper);
        }

        [Fact]
        public async Task Handle_ReturnsRefreshTokenResponseByToken()
        {
            // Arrange
            RefreshTokenResponse expected = new(5, "neoWRWQUOfjoi}DW[", 1);


            var refreshTokens = new List<RefreshToken>
            {
                new RefreshToken { Id = 12, Name = "fnuognweoijef84", UserId = 5 },
                new RefreshToken { Id = expected.Id, Name = expected.Name, UserId = expected.UserId }
            }.AsQueryable();

            RepoMock.Setup(x => x.GetAll<RefreshToken>())
                    .Returns(refreshTokens)
                    .Verifiable();

            // Act
            var actual = await getRefreshTokenByTokenHandler.Handle(new GetRefreshTokenByTokenQuery(expected.Name),
                                                                         new CancellationToken());

            // Assert
            actual.Should().Be(expected);
            RepoMock.Verify();
        }
    }
}
