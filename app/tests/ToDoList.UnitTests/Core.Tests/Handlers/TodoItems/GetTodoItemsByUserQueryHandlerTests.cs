using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MockQueryable.Moq;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.TodoItems;
using ToDoList.Core.Mediator.Queries.TodoItems;
using ToDoList.Core.Mediator.Response;
using ToDoList.Core.Services;

using Xunit;

namespace Core.Tests.Handlers.TodoItems
{
    public class GetTodoItemsByUserQueryHandlerTests : HandlerBaseForTests
    {
        private readonly Mock<ICreateWithAddressService> addressServiceMock;
        private readonly GetTodoItemsByUserIdQueryHandler getTodoItemsHandler;

        public GetTodoItemsByUserQueryHandlerTests()
        {
            addressServiceMock = new Mock<ICreateWithAddressService>();

            getTodoItemsHandler = new GetTodoItemsByUserIdQueryHandler(UnitOfWorkMock.Object, Mapper, addressServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsTodoItems()
        {
            // Arrange
            int userId = 12;

            var todoItems = GetSampleTodoItems();
            var todoItemsMock = todoItems.AsQueryable().BuildMock();

            var responses = Mapper.Map<IEnumerable<TodoItemResponse>>(todoItems);
            var expected = GetWithAddress(responses);

            RepoMock.Setup(x => x.GetAll<TodoItem>())
                    .Returns(todoItemsMock.Object)
                    .Verifiable();

            addressServiceMock.Setup(x => x.GetItemsWithAddressAsync(It.IsAny<IEnumerable<TodoItemResponse>>()))
                              .ReturnsAsync(expected);

            // Act
            var actual = await getTodoItemsHandler.Handle(new GetTodoItemsByUserIdQuery(userId), new CancellationToken());

            // Assert
            Assert.Equal(expected, actual);

            RepoMock.Verify();
            addressServiceMock.Verify(x => x.GetItemsWithAddressAsync(It.IsAny<IEnumerable<TodoItemResponse>>()), Times.Once);
        }

        private static IEnumerable<TodoItemResponse> GetWithAddress(IEnumerable<TodoItemResponse> responses)
        {
            List<TodoItemResponse> expected = new();

            foreach (var response in responses)
            {
                if (response.GeoPoint is not null && response.GeoPoint.Latitude == 49.06802 && response.GeoPoint.Longitude == 33.42041)
                {
                    expected.Add(response with { Address = "Khalamenyuka St, 4, Kremenchuk, Poltavs'ka oblast, Ukraine, 39600" });
                }
                else
                    expected.Add(response);
            }

            return expected;
        }

        private static IEnumerable<TodoItem> GetSampleTodoItems()
        {
            var checklist1 = new Checklist { Id = 12, Name = "Chores", UserId = 4 };
            var checklist2 = new Checklist { Id = 22, Name = "Chores", UserId = 12 };

            var todoItems = new List<TodoItem>
            {
                new TodoItem
                {
                    Name = "Complete some course",
                    ChecklistId = 1,
                    Checklist = checklist1,
                    StatusId = 1,
                    GeoPoint = new NetTopologySuite.Geometries.Point(33.42041, 49.06802)
                },
                new TodoItem
                {
                    Name = "Finish the book",
                    ChecklistId = 1,
                    Checklist = checklist2,
                    StatusId = 2,
                }
            };

            return todoItems;
        }
    }
}
