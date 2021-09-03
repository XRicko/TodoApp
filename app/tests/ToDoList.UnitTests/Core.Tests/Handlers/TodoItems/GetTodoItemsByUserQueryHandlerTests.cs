using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using MockQueryable.Moq;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.TodoItems;
using ToDoList.Core.Mediator.Queries.TodoItems;
using ToDoList.Core.Mediator.Response;
using ToDoList.Core.Services;
using ToDoList.SharedKernel;

using Xunit;

namespace Core.Tests.Handlers.TodoItems
{
    public class GetTodoItemsByUserQueryHandlerTests : HandlerBaseForTests
    {
        private readonly GetTodoItemsByUserIdQueryHandler getTodoItemsHandler;

        private readonly Mock<IAddressService> addressServiceMock;
        private readonly Mock<IFileSystem> fileSystemMock;

        private readonly string imagePath;

        public GetTodoItemsByUserQueryHandlerTests()
        {
            addressServiceMock = new Mock<IAddressService>();
            fileSystemMock = new Mock<IFileSystem>();

            getTodoItemsHandler = new GetTodoItemsByUserIdQueryHandler(UnitOfWorkMock.Object, Mapper,
                                                                       addressServiceMock.Object, fileSystemMock.Object);

            imagePath = "C:\\";
        }

        [Fact]
        public async Task Handle_ReturnsTodoItems()
        {
            // Arrange
            int userId = 12;

            var todoItems = GetSampleTodoItems();
            var todoItemsMock = todoItems.AsQueryable().BuildMock();

            byte[] bytes = new byte[321];
            new Random().NextBytes(bytes);

            var responses = new List<TodoItemResponse>
            {
                new(21, "Complete some course", DateTime.Now, 22, "Chores",
                    2, "Planned", GeoPoint: new GeoCoordinate(33.42041, 49.06802),
                    ImageId: 12, ImageName: "rand.jpg", ImageContent: bytes)
            };

            var expected = GetWithAddress(responses);

            RepoMock.Setup(x => x.GetAll<TodoItem>())
                    .Returns(todoItemsMock.Object)
                    .Verifiable();


            fileSystemMock.Setup(x => x.File.ReadAllBytes(imagePath))
                          .Returns(bytes)
                          .Verifiable();

            addressServiceMock.Setup(x => x.GetItemsWithAddressAsync(It.IsAny<IEnumerable<TodoItemResponse>>()))
                              .ReturnsAsync(expected)
                              .Verifiable();

            // Act
            var actual = await getTodoItemsHandler.Handle(new GetTodoItemsByUserIdQuery(userId),
                                                          new CancellationToken());

            // Assert
            actual.Should().Equal(expected);
            actual.Should().Contain(x => x.ImageContent == bytes);

            RepoMock.Verify();
            fileSystemMock.Verify();
            addressServiceMock.Verify();
        }

        private static IEnumerable<TodoItemResponse> GetWithAddress(IEnumerable<TodoItemResponse> responses)
        {
            List<TodoItemResponse> expected = new();

            foreach (var response in responses)
            {
                if (response.GeoPoint is not null
                    && response.GeoPoint.Latitude == 49.06802
                    && response.GeoPoint.Longitude == 33.42041)
                {
                    expected.Add(response with { Address = "Khalamenyuka St, 4, Kremenchuk, Poltavs'ka oblast, Ukraine, 39600" });
                }
                else
                    expected.Add(response);
            }

            return expected;
        }

        private IEnumerable<TodoItem> GetSampleTodoItems()
        {
            var checklist1 = new Checklist { Id = 12, Name = "Chores", UserId = 4 };
            var checklist2 = new Checklist { Id = 22, Name = "Chores", UserId = 12 };

            var todoItems = new List<TodoItem>
            {
                new TodoItem
                {
                    Id = 21,
                    Name = "Complete some course",
                    Checklist = checklist2,
                    Status = new Status { Id = 2, Name = "Planned" },
                    Category = new Category(),
                    Image = new Image { Id = 12, Name = "rand.jpg", Path = imagePath },
                    GeoPoint = new NetTopologySuite.Geometries.Point(33.42041, 49.06802)
                },
                new TodoItem
                {
                    Id = 12,
                    Name = "Finish the book",
                    Checklist = checklist1,
                    Status = new Status(),
                    Category = new Category(),
                    Image = new Image()
                }
            };

            return todoItems;
        }
    }
}
