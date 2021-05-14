using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using MockQueryable.Moq;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
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

        private readonly Expression<Func<User, bool>> expression;

        public AddUserCommandHandlerTests() : base()
        {
            passwordHasher = new PasswordHasher();
            addUserHandler = new AddUserCommandHandler(UnitOfWorkMock.Object, Mapper, passwordHasher);

            username = "admin";
            password = "qwerty";

            expression = u => u.Name == username && passwordHasher.VerifyPassword(password, u.Password);
        }

        [Fact]
        public async Task Handle_AddUserGivenNew()
        {
            // Arrange
            var users = new List<User> { new User { Id = 341, Name = "root", Password = "$MYHASH$V1$1000$jeqnfoqeigjq" } };
            var newUser = new UserRequest(username, password);

            var usersMock = users.AsQueryable().BuildMock();

            RepoMock.Setup(x => x.GetAll<User>())
                  .Returns(usersMock.Object)
                  .Verifiable();

            // Act
            await addUserHandler.Handle(new AddCommand<UserRequest>(newUser), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Add(It.Is(expression)), Times.Once);
            RepoMock.Verify(x => x.Add(It.Is<Checklist>(l => l.Name == "Untitled")), Times.Once);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_DoesntAddUserGivenExisting()
        {
            // Arrange
            var existingUser = new User { Id = 420, Name = username, Password = passwordHasher.Hash(password) };
            var request = new UserRequest(username, password);

            var users = new List<User> { existingUser };
            var usersMock = users.AsQueryable().BuildMock();

            RepoMock.Setup(x => x.GetAll<User>())
                    .Returns(usersMock.Object)
                    .Verifiable();

            // Act
            await addUserHandler.Handle(new AddCommand<UserRequest>(request), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Add(It.Is(expression)), Times.Never);
            RepoMock.Verify(x => x.Add(It.Is<Checklist>(l => l.Name == "Untitled")), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }
    }
}
