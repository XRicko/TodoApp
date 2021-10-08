using System.Collections.Generic;
using System.Linq;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Extensions;
using ToDoList.SharedKernel.Interfaces;

using Xunit;

namespace Core.Tests.Extensions
{
    public class IRepositoryExtensionsTests
    {
        private readonly Mock<IRepository> repoMock;

        private readonly int checklistId;
        private readonly Checklist checklist;

        public IRepositoryExtensionsTests()
        {
            repoMock = new Mock<IRepository>();

            checklistId = 69;
            checklist = new Checklist
            {
                Id = checklistId,
                Name = "Something",
                TodoItems = new List<TodoItem>
                    {
                        new TodoItem { Id = 42, ChecklistId = checklistId},
                        new TodoItem { Id = 420, ChecklistId = checklistId }
                    }
            };

            var checklists = new List<Checklist>
            {
                new Checklist
                {
                    Id = 21,
                    Name = "Chores",
                    TodoItems = new List<TodoItem>
                    {
                        new TodoItem { Id = 5, ChecklistId = 21 },
                        new TodoItem { Id = 8, ChecklistId = 21 }
                    }
                },
                checklist
            }.AsQueryable();

            repoMock.Setup(x => x.GetAll<Checklist>())
                    .Returns(checklists)
                    .Verifiable();
        }

        [Fact]
        public void RemoveChecklist_RemovesChecklistWithTodoItemsGivenValidId()
        {
            // Arrange
            int validId = checklistId;

            // Act
            repoMock.Object.RemoveChecklist(validId);

            // Assert
            repoMock.Verify();

            repoMock.Verify(x => x.Remove(It.Is<Checklist>(c => c.Id == checklistId)), Times.Once);
            repoMock.Verify(x => x.Remove(It.Is<TodoItem>(t => t.ChecklistId == checklistId)), Times.Exactly(checklist.TodoItems.Count));
        }

        [Fact]
        public void RemoveChecklist_DoesntRemoveGivenInvalidId()
        {
            // Arrange
            int invalidId = 100;

            // Act
            repoMock.Object.RemoveChecklist(invalidId);

            // Assert
            repoMock.Verify();

            repoMock.Verify(x => x.Remove(It.IsAny<Checklist>()), Times.Never);
            repoMock.Verify(x => x.Remove(It.IsAny<TodoItem>()), Times.Never);
        }
    }
}
