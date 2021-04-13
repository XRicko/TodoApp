using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.TodoItems;
using ToDoList.Core.Mediator.Queries.TodoItems;
using ToDoList.Core.Mediator.Response;
using ToDoList.Core.Services;

using Xunit;

namespace Core.Tests.Handlers.TodoItems
{
    public class GetActiveOrDoneTodoItemsByUserQueryHandlerTests : HandlerBaseForTests
    {
        private readonly Mock<ICreateWithAddressService> addressServiceMock;
        private readonly GetActiveOrDoneTodoItemsByUserQueryHandler getTodoItemsHandler;

        public GetActiveOrDoneTodoItemsByUserQueryHandlerTests()
        {
            addressServiceMock = new Mock<ICreateWithAddressService>();

            getTodoItemsHandler = new GetActiveOrDoneTodoItemsByUserQueryHandler(UnitOfWorkMock.Object, Mapper, addressServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsActiveTodoItems()
        {
            await TestGetActiveOrDone(false);
        }

        [Fact]
        public async Task Handle_ReturnsDoneTodoItems()
        {
            await TestGetActiveOrDone(true);
        }

        private async Task TestGetActiveOrDone(bool isDone)
        {
            // Arrange
            var todoItems = GetSampleTodoItems();
            var activeOrDoneTodoItems = todoItems.Where(i => i.Checklist.UserId == 1
                                                       && i.Status.IsDone == isDone);

            var responses = Mapper.Map<IEnumerable<TodoItemResponse>>(activeOrDoneTodoItems);
            var expected = GetWithAddress(responses);

            RepoMock.Setup(x => x.GetAllAsync<TodoItem>())
                    .ReturnsAsync(todoItems);

            addressServiceMock.Setup(x => x.GetItemsWithAddressAsync(It.IsAny<IEnumerable<TodoItemResponse>>()))
                              .ReturnsAsync(expected);

            // Act
            var actual = await getTodoItemsHandler.Handle(new GetActiveOrDoneTodoItemsByUserQuery(1, isDone), new CancellationToken());

            // Assert
            Assert.Equal(expected, actual);

            RepoMock.Verify(x => x.GetAllAsync<TodoItem>(), Times.Once);
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
            var personal = new Checklist { Id = 1, Name = "Personal", UserId = 1 };
            var birthday = new Checklist { Id = 2, Name = "Birthday", UserId = 1 };

            var statusUndone = new Status { Id = 1, Name = "Ongoing", IsDone = false };
            var statusDone = new Status { Id = 2, Name = "Done", IsDone = true };

            var todoItems = new List<TodoItem>
            {
                new TodoItem
                {
                    Name = "Complete some course",

                    ChecklistId = personal.Id,
                    Checklist = personal,

                    StatusId = statusUndone.Id,
                    Status = statusUndone,

                    GeoPoint = new NetTopologySuite.Geometries.Point(33.42041, 49.06802)
                },
                new TodoItem
                {
                    Name = "Invite friends",

                    ChecklistId = birthday.Id,
                    Checklist = birthday,

                    StatusId = statusDone.Id,
                    Status = statusDone,

                    GeoPoint = new NetTopologySuite.Geometries.Point(33.42041, 49.06802)
                },
                new TodoItem
                {
                    Name = "Prepare a party",

                    ChecklistId = birthday.Id,
                    Checklist = birthday,

                    StatusId = statusUndone.Id,
                    Status = statusUndone,                },
                new TodoItem
                {
                    Name = "Finish the book",
                    ChecklistId = personal.Id,
                    Checklist = personal,

                    StatusId = statusDone.Id,
                    Status = statusDone,
                }
            };

            return todoItems;
        }
    }
}
