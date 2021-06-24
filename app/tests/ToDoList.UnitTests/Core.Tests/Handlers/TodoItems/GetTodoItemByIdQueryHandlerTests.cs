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
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.Core.Services;
using ToDoList.SharedKernel;

using Xunit;

namespace Core.Tests.Handlers.TodoItems
{
    public class GetTodoItemByIdQueryHandlerTests : HandlerBaseForTests
    {
        private readonly GetTodoItemByIdQueryHandler handler;

        private readonly Mock<ICreateWithAddressService> createAddressServiceMock;
        private readonly Mock<IFileSystem> fileSystemMock;

        public GetTodoItemByIdQueryHandlerTests()
        {
            createAddressServiceMock = new Mock<ICreateWithAddressService>();
            fileSystemMock = new Mock<IFileSystem>();

            handler = new GetTodoItemByIdQueryHandler(UnitOfWorkMock.Object, Mapper,
                                                           createAddressServiceMock.Object, fileSystemMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsTodoItemById()
        {
            // Arrange
            int id = 21;
            string imagePath = "c:\\";

            var todoItems = new List<TodoItem>
            {
                new TodoItem
                {
                    Id = id,
                    Name = "Complete some course",
                    Checklist = new Checklist { Id = 12, Name = "Chores", UserId = 4 },
                    Category = new Category(),
                    Status = new Status { Id = 2, Name = "Planned" },
                    Image = new Image { Id = 12, Name = "rand.jpg", Path = imagePath },
                    GeoPoint = new NetTopologySuite.Geometries.Point(33.42041, 49.06802),

                },
                new TodoItem
                {
                    Id = 12,
                    Name = "Finish the book",
                    Checklist = new Checklist(),
                    Status = new Status(),
                    Category = new Category(),
                    Image = new Image()
                }
            };

            var todoItemsMock = todoItems.AsQueryable().BuildMock();

            byte[] bytes = new byte[321];
            new Random().NextBytes(bytes);

            TodoItemResponse response = new(21, "Complete some course", DateTime.Now, 22,
                                            "Chores", 2, "Planned",
                                            GeoPoint: new GeoCoordinate(33.42041, 49.06802),
                                            ImageId: 12, ImageName: "rand.jpg", ImageContent: bytes);

            var expected = response with
            {
                Address = "Khalamenyuka St, 4, Kremenchuk, Poltavs'ka oblast, Ukraine, 39600"
            };

            RepoMock.Setup(x => x.GetAll<TodoItem>())
                    .Returns(todoItemsMock.Object)
                    .Verifiable();

            fileSystemMock.Setup(x => x.File.ReadAllBytes(imagePath))
                          .Returns(bytes)
                          .Verifiable();

            createAddressServiceMock.Setup(x => x.GetItemWithAddressAsync(It.Is<TodoItemResponse>(x => x.Id == id)))
                                    .ReturnsAsync(expected)
                                    .Verifiable();

            // Act
            var actual = await handler.Handle(new GetByIdQuery<TodoItem, TodoItemResponse>(id),
                                              new CancellationToken());

            // Assert
            actual.Should().Be(expected);

            RepoMock.Verify();
            fileSystemMock.Verify();
            createAddressServiceMock.Verify();
        }
    }
}
