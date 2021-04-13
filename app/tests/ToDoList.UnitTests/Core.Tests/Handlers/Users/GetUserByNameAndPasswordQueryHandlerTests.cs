using System.Collections.Generic;
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
            var users = GetSampleUsers();

            RepoMock.Setup(x => x.GetAllAsync<User>())
                    .ReturnsAsync(users);

            // Act
            var actual = await getUserByNameAndPasswordHandler.Handle(new GetUserByNameAndPasswordQuery(username, password), new CancellationToken());

            // Assert
            Assert.Equal(username, actual.Name);
            Assert.True(passwordHasher.VerifyPassword(password, actual.Password));

            RepoMock.Verify(x => x.GetAllAsync<User>(), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsNullGivenInvalidCredentials()
        {
            // Arrange
            var users = GetSampleUsers();

            RepoMock.Setup(x => x.GetAllAsync<User>())
                    .ReturnsAsync(users);

            // Act
            var actual = await getUserByNameAndPasswordHandler.Handle(new GetUserByNameAndPasswordQuery(username, "ivalid_credentials"), new CancellationToken());

            // Assert
            Assert.Null(actual);
            RepoMock.Verify(x => x.GetAllAsync<User>(), Times.Once);
        }

        private IEnumerable<User> GetSampleUsers()
        {
            return new List<User>
            {
                new User { Name = username, Password = passwordHasher.Hash(password) },
                new User { Name = "qwerty", Password = passwordHasher.Hash("admin") },
                new User { Name = "anonim", Password = passwordHasher.Hash("123456") },
                new User { Name = "anonim", Password = passwordHasher.Hash(password) }
            };
        }
    }
}
