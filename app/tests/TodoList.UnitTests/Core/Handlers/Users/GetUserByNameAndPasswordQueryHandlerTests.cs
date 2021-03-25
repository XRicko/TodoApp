using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Users;
using ToDoList.Core.Mediator.Queries.Users;
using ToDoList.Core.Services;

using Xunit;

namespace ToDoList.UnitTests.Core.Handlers.Users
{
    public class GetUserByNameAndPasswordQueryHandlerTests : HandlerBaseForTests
    {
        private readonly GetUserByNameAndPasswordQueryHandler getUserByNameAndPasswordHandler;

        private readonly string username;
        private readonly string password;

        public GetUserByNameAndPasswordQueryHandlerTests() : base()
        {
            getUserByNameAndPasswordHandler = new GetUserByNameAndPasswordQueryHandler(UnitOfWorkMock.Object, Mapper);

            username = "admin";
            password = "qwerty";
        }

        [Fact]
        public async Task ReturnsUserGivenProperCredentials()
        {
            // Arrange
            var users = GetSampleUsers();

            RepoMock.Setup(x => x.GetAllAsync<User>())
                  .ReturnsAsync(users);

            // Act
            var actual = await getUserByNameAndPasswordHandler.Handle(new GetUserByNameAndPasswordQuery(username, password), new CancellationToken());

            // Assert
            Assert.Equal(username, actual.Name);
            Assert.True(PasswordHasher.Verify(password, actual.Password));

            RepoMock.Verify(x => x.GetAllAsync<User>(), Times.Once);
        }

        [Fact]
        public async Task ReturnsNullGivenInvalidCredentials()
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
                new User { Name = username, Password = PasswordHasher.Hash(password) },
                new User { Name = "qwerty", Password = PasswordHasher.Hash("admin") },
                new User { Name = "anonim", Password = PasswordHasher.Hash("123456") },
                new User { Name = "anonim", Password = PasswordHasher.Hash(password) }
            };
        }
    }
}
