using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Handlers.Users;
using ToDoList.Core.Mediator.Requests;
using ToDoList.Core.Services;

using Xunit;

namespace Core.Tests.Handlers.Users
{
    public class AddUserCommandHandlerTests : HandlerBaseForTests
    {
        private readonly PasswordHasher passwordHasher;
        private readonly AddUserCommandHandler addUserHandler;

        private readonly string username;
        private readonly string password;

        public AddUserCommandHandlerTests() : base()
        {
            passwordHasher = new PasswordHasher();
            addUserHandler = new AddUserCommandHandler(UnitOfWorkMock.Object, Mapper, passwordHasher);

            username = "admin";
            password = "qwerty";
        }

        [Fact]
        public async Task AddUserGivenNew()
        {
            // Arrange
            var users = GetSampleUsers();
            var newUser = new UserRequest("new_user", password);

            RepoMock.Setup(x => x.GetAllAsync<User>())
                    .ReturnsAsync(users);

            // Act
            await addUserHandler.Handle(new AddCommand<UserRequest>(newUser), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.GetAllAsync<User>(), Times.Once);
            RepoMock.Verify(x => x.AddAsync(It.Is<User>(u => u.Name == newUser.Name
                                                             && passwordHasher.VerifyPassword(newUser.Password, u.Password))), Times.Once);
            RepoMock.Verify(x => x.AddAsync(It.Is<Checklist>(l => l.Name == "Untitled")), Times.Once);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Exactly(2));
        }

        [Fact]
        public async Task DoesntAddUserGivenExisting()
        {
            // Arrange
            var existingUser = new UserRequest(username, password);
            var users = GetSampleUsers();

            RepoMock.Setup(x => x.GetAllAsync<User>())
              .ReturnsAsync(users);

            // Act
            await addUserHandler.Handle(new AddCommand<UserRequest>(existingUser), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.GetAllAsync<User>(), Times.Once);
            RepoMock.Verify(x => x.AddAsync(It.Is<User>(u => u.Name == existingUser.Name
                                                             && passwordHasher.VerifyPassword(existingUser.Password, u.Password))), Times.Never);
            RepoMock.Verify(x => x.AddAsync(It.Is<Checklist>(l => l.Name == "Untitled")), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }

        private IEnumerable<User> GetSampleUsers()
        {
            return new List<User>
            {
                new User { Name = username, Password = passwordHasher.Hash(password) },
                new User { Name = "qwerty", Password = passwordHasher.Hash("admin") },
                new User { Name = "anonim", Password = passwordHasher.Hash("123456") },
                new User { Name = "anonim", Password = passwordHasher.Hash("asjdnj") }
            };
        }
    }
}
