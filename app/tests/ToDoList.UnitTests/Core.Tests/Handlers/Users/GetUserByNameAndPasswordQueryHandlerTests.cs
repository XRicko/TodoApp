using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Users;
using ToDoList.Core.Mediator.Queries.Users;
using ToDoList.Core.Services;

using Xunit;

namespace Core.Tests.Handlers.Users
{
    public class GetUserByNameAndPasswordQueryHandlerTests : HandlerBaseForTests
    {
        private readonly PasswordHasher passwordHasher;
        private readonly GetUserByNameAndPasswordQueryHandler getUserByNameAndPasswordHandler;

        private readonly string username;
        private readonly string password;

        public GetUserByNameAndPasswordQueryHandlerTests() : base()
        {
            passwordHasher = new PasswordHasher();
            getUserByNameAndPasswordHandler = new GetUserByNameAndPasswordQueryHandler(UnitOfWorkMock.Object, Mapper, passwordHasher);

            username = "admin";
            password = "qwerty";
        }

        [Fact]
        public async Task Handle_ReturnsUserGivenProperCredentials()
        {
            // Arrange
            var user = new User { Id = 31, Name = username, Password = passwordHasher.Hash(password) };

            RepoMock.Setup(x => x.GetByNameAsync<User>(username))
                    .ReturnsAsync(() => user)
                    .Verifiable();

            // Act
            var actual = await getUserByNameAndPasswordHandler.Handle(new GetUserByNameAndPasswordQuery(username, password), new CancellationToken());

            // Assert
            Assert.Equal(username, actual.Name);
            Assert.True(passwordHasher.VerifyPassword(password, actual.Password));

            RepoMock.Verify();
        }

        [Fact]
        public async Task Handle_ReturnsNullGivenInvalidCredentials()
        {
            // Arrange
            RepoMock.Setup(x => x.GetByNameAsync<User>(username))
                    .ReturnsAsync(() => null)
                    .Verifiable();

            // Act
            var actual = await getUserByNameAndPasswordHandler.Handle(new GetUserByNameAndPasswordQuery(username, password), new CancellationToken());

            // Assert
            Assert.Null(actual);
            RepoMock.Verify();
        }
    }
}
