using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

using AutoMapper;

using FluentAssertions;

using Moq;

using NetTopologySuite.Geometries;

using ToDoList.Core.Entities;
using ToDoList.Core.Extensions;
using ToDoList.Core.MappingProfiles;

using Xunit;

namespace Core.Tests.Extensions
{
    public class IQueryableExtensionsTests
    {
        [Fact]
        public void AsTodoItemResponses_ReturnsTodoItemResponses()
        {
            // Arrange
            Mock<IFileSystem> fileSystemMock = new();

            var profile = new EntityToDtoMappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));

            var mapper = new Mapper(configuration);

            Point geoPoint = new(45.21, 74.36);
            string path = "path";

            var todoItem = new TodoItem
            {
                Id = 2,
                Name = "Visit",
                Checklist = new Checklist { Id = 13, Name = "Chores", ProjectId = 1 },
                ChecklistId = 13,
                StartDate = DateTime.Now.AddDays(-2),
                Status = new Status { Id = 1, Name = "Planned", IsDone = false },
                StatusId = 1,
                GeoPoint = geoPoint,
                Image = new Image { Id = 45, Name = "image.jpg", Path = path },
                ImageId = 45,
                Category = new Category { Id = 85, Name = "SOmethin" },
                CategoryId = 85
            };
            var todoItems = new List<TodoItem> { todoItem }.AsQueryable();

            byte[] bytes = new byte[420];
            new Random().NextBytes(bytes);

            fileSystemMock.Setup(x => x.File.ReadAllBytes(path))
                          .Returns(bytes)
                          .Verifiable();

            // Act
            var actual = todoItems.AsTodoItemResponses(mapper, fileSystemMock.Object)
                                  .SingleOrDefault();

            // Assert
            actual.Id.Should().Be(todoItem.Id);
            actual.GeoPoint.Latitude.Should().Be(geoPoint.Y);
            actual.ChecklistName.Should().Be(todoItem.Checklist.Name);
            actual.ImageContent.Should().Equal(bytes);

            fileSystemMock.Verify();
        }
    }
}
