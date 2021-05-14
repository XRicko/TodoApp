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
using ToDoList.Core.Mediator.Handlers.TodoItems;
using ToDoList.Core.Mediator.Requests.Create;

using Xunit;

namespace Core.Tests.Handlers.TodoItems
{
    public class AddTodoItemCommandHandlerTests : HandlerBaseForTests
    {
        private readonly AddTodoItemCommandHandler addTodoItemHandler;

        private readonly string name;
        private readonly int checklistId;

        private readonly Expression<Func<TodoItem, bool>> expression;

        public AddTodoItemCommandHandlerTests() : base()
        {
            addTodoItemHandler = new AddTodoItemCommandHandler(UnitOfWorkMock.Object, Mapper);

            name = "Buy everyting needed";
            checklistId = 3;

            expression = e => e.Name == name && e.ChecklistId == checklistId;
        }

        [Fact]
        public async Task Handle_AddsTodoItemGivenNew()
        {
            // Arrange
            var todoItems = new List<TodoItem> { new TodoItem { Id = 3256, Name = "Sommething", ChecklistId = 12, StatusId = 1 } };
            var newRequest = new TodoItemCreateRequest(name, checklistId, 1);

            var todoItemsMock = todoItems.AsQueryable().BuildMock();

            RepoMock.Setup(x => x.GetAll<TodoItem>())
                    .Returns(todoItemsMock.Object)
                    .Verifiable();

            // Act
            await addTodoItemHandler.Handle(new AddCommand<TodoItemCreateRequest>(newRequest), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Add(It.Is(expression)), Times.Once);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DoesntAddTodoItemGivenExisting()
        {
            // Arrange
            var existingTodoItem = new TodoItem { Id = 3, Name = name, ChecklistId = checklistId, StatusId = 2 };
            var request = new TodoItemCreateRequest(name, checklistId, 2);

            var todoItems = new List<TodoItem> { existingTodoItem };
            var todoItemsMock = todoItems.AsQueryable().BuildMock();

            RepoMock.Setup(x => x.GetAll<TodoItem>())
                    .Returns(todoItemsMock.Object)
                    .Verifiable();

            // Act
            await addTodoItemHandler.Handle(new AddCommand<TodoItemCreateRequest>(request), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Add(It.Is(expression)), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }
    }
}
