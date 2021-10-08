using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using ToDoList.BlazorClient.State;
using ToDoList.SharedClientLibrary;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

using Xunit;

namespace BlazorClient.Tests.State
{
    public class StateContainerTests
    {
        private readonly Mock<IApiInvoker> apiInvokerMock;

        private readonly StateContainer state;

        public StateContainerTests()
        {
            apiInvokerMock = new Mock<IApiInvoker>();

            state = new StateContainer(apiInvokerMock.Object);
        }

        [Fact]
        public async Task InitAsync_InitializeItems()
        {
            // Arrange
            var projects = new List<ProjectModel>
            {
                new ProjectModel { Id = 1, Name = "Name", UserId = 21},
                new ProjectModel { Id = 13, Name = "Something", UserId = 21 }
            };

            var statuses = new List<StatusModel>
            {
                new StatusModel { Id = 1, Name = "Planned", IsDone = false },
                new StatusModel { Id = 2, Name = "Done", IsDone = true }
            };

            var categories = new List<CategoryModel>
            {
                new CategoryModel { Id = 134, Name = "Imp" },
                new CategoryModel { Id = 12, Name = "Unimp" }
            };

            apiInvokerMock.Setup(x => x.GetItemsAsync<ProjectModel>(ApiEndpoints.Projects))
                          .ReturnsAsync(projects)
                          .Verifiable();

            apiInvokerMock.Setup(x => x.GetItemsAsync<StatusModel>(ApiEndpoints.Statuses))
                          .ReturnsAsync(statuses)
                          .Verifiable();

            apiInvokerMock.Setup(x => x.GetItemsAsync<CategoryModel>(ApiEndpoints.Categories))
                          .ReturnsAsync(categories)
                          .Verifiable();

            // Act
            await state.InitAsync();

            // Assert
            projects.Should().Equal(projects);
            statuses.Should().Equal(statuses);
            categories.Should().Equal(categories);

            apiInvokerMock.Verify();
        }
    }
}
