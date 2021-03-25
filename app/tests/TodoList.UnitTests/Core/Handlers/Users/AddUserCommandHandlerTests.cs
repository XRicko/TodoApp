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

namespace ToDoList.UnitTests.Core.Handlers.Users
{
    public class AddUserCommandHandlerTests : HandlerBaseForTests
    {
        private readonly AddUserCommandHandler addUserHandler;

        private readonly string userName;
        private readonly string password;
        private readonly string hashedPassword;

        public AddUserCommandHandlerTests() : base()
        {
            addUserHandler = new AddUserCommandHandler(UnitOfWorkMock.Object, Mapper);

            userName = "admin";
            password = "qwerty";
            hashedPassword = PasswordHasher.Hash(password);
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
                                                             && PasswordHasher.Verify(newUser.Password, hashedPassword))), Times.Once);
            RepoMock.Verify(x => x.AddAsync(It.Is<Checklist>(l => l.Name == "Untitled")), Times.Once);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Exactly(2));
        }

        [Fact]
        public async Task DoesntAddUserGivenExisting()
        {
            // Arrange
            var existingUser = new UserRequest(userName, hashedPassword);
            var users = GetSampleUsers();

            RepoMock.Setup(x => x.GetAllAsync<User>())
              .ReturnsAsync(users);

            // Act
            await addUserHandler.Handle(new AddCommand<UserRequest>(existingUser), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.GetAllAsync<User>(), Times.Once);
            RepoMock.Verify(x => x.AddAsync(It.Is<User>(u => u.Name == existingUser.Name
                                                             && PasswordHasher.Verify(existingUser.Password, hashedPassword))), Times.Never);
            RepoMock.Verify(x => x.AddAsync(It.Is<Checklist>(l => l.Name == "Untitled")), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }

        private IEnumerable<User> GetSampleUsers()
        {
            return new List<User>
            {
                new User { Name = userName, Password = PasswordHasher.Hash(password) },
                new User { Name = "qwerty", Password = PasswordHasher.Hash("admin") },
                new User { Name = "anonim", Password = PasswordHasher.Hash("123456") },
                new User { Name = "anonim", Password = PasswordHasher.Hash("asjdnj") }
            };
        }
    }
}
