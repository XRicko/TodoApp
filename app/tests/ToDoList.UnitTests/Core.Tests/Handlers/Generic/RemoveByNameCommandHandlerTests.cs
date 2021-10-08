using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Handlers.RefreshTokens;

using Xunit;

namespace Core.Tests.Handlers.Generic
{
    public class RemoveByNameCommandHandlerTests : HandlerBaseForTests
    {
        private readonly RemoveRefreshTokenByNameCommandHandler handler;

        private readonly int id;
        private readonly string name;

        private readonly IQueryable<RefreshToken> refreshTokens;

        public RemoveByNameCommandHandlerTests() : base()
        {
            handler = new RemoveRefreshTokenByNameCommandHandler(UnitOfWorkMock.Object, Mapper);

            id = 420;
            name = "nwudWOIjiew.wdqwpJQIQndonWndjnx";

            refreshTokens = new List<RefreshToken>
            {
                new RefreshToken
                {
                    Id = id,
                    Name = name
                },
                new RefreshToken
                {
                    Id = 12,
                    Name = "Qjnsx.Feiqpnjznhqy"
                }
            }.AsQueryable();

            RepoMock.Setup(x => x.GetAll<RefreshToken>())
                    .Returns(refreshTokens)
                    .Verifiable();
        }

        [Fact]
        public async Task Handle_DeletesItemGivenValidName()
        {
            // Act
            await handler.Handle(new RemoveByNameCommand<RefreshToken>(name), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Remove(It.Is<RefreshToken>(x => x.Id == id)), Times.Once);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
        }


        [Fact]
        public async Task Handle_DoesntDeleteItemGivenInvalidName()
        {
            // Arrange
            string invalidName = "Qnjtp[fjqenfwib.qqwDKjmnqoc";

            // Act
            await handler.Handle(new RemoveByNameCommand<RefreshToken>(invalidName), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Remove(It.IsAny<RefreshToken>()), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }
    }
}
