using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Queries.Projects;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Controllers;

using Xunit;

namespace WebApi.Tests.Controllers
{
    public class ProjectsControllerTests : ApiControllerBaseForTests
    {
        private readonly IDistributedCache cache;
        private readonly ProjectsController controller;

        private readonly int userId;
        private readonly string recordKey;

        public ProjectsControllerTests()
        {
            var opts = Options.Create(new MemoryDistributedCacheOptions());
            cache = new MemoryDistributedCache(opts);

            controller = new ProjectsController(MediatorMock.Object, cache);

            userId = 420;
            recordKey = $"Projects_User_{userId}";

            var contextMock = new Mock<HttpContext>();
            controller.ControllerContext.HttpContext = contextMock.Object;

            var claim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());

            contextMock.Setup(x => x.User.FindFirst(ClaimTypes.NameIdentifier))
                       .Returns(claim);
        }

        [Fact]
        public async Task Get_ReturnsNewListOfProjectResponsesByUserAndSetsCache()
        {
            var expected = GetSampleProjectResponsesByUser();

            MediatorMock.Setup(x => x.Send(new GetProjectsByUserIdQuery(userId), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected)
                        .Verifiable();

            // Act
            var actual = await controller.Get();
            var cached = JsonSerializer.Deserialize<List<ProjectResponse>>(cache.GetString(recordKey));

            // Assert
            actual.Should().Equal(expected);
            cached.Should().Equal(expected);

            MediatorMock.Verify();
        }

        [Fact]
        public async Task Get_ReturnsListOfProjectsResponsesByUserFromCache()
        {
            // Arrange
            var expected = GetSampleProjectResponsesByUser();

            cache.SetString(recordKey, JsonSerializer.Serialize(expected));

            // Act
            var actual = await controller.Get();

            // Assert
            actual.Should().Equal(expected);
            MediatorMock.Verify(x => x.Send(new GetProjectsByUserIdQuery(userId), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetByName_ReturnsNullGivenInvalidData()
        {
            // Arrange
            string name = "invalid";

            MediatorMock.Setup(x => x.Send(new GetProjectByNameAndUserIdQuery(name, userId), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => null)
                        .Verifiable();

            // Act
            var actual = await controller.GetByName(name);

            // Assert
            actual.Value.Should().BeNull();
            MediatorMock.Verify();
        }

        [Fact]
        public async Task GetByName_ReturnsProjectResponseGivenValidData()
        {
            // Arrange
            string name = "Name";
            ProjectResponse expected = new(12, name, userId);

            MediatorMock.Setup(x => x.Send(new GetProjectByNameAndUserIdQuery(name, userId), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected)
                        .Verifiable();

            // Act
            var actual = await controller.GetByName(name);

            // Assert
            actual.Value.Should().Be(expected);
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Add_SendsRequest()
        {
            // Arrange
            var createRequest = new ProjectCreateRequest("Other", userId);
            var responses = GetSampleProjectResponsesByUser();

            cache.SetString(recordKey, JsonSerializer.Serialize(responses));

            // Act
            var result = await controller.Add(createRequest);

            // Assert
            cache.Get(recordKey).Should().BeNull();
            result.Should().BeAssignableTo<NoContentResult>();

            MediatorMock.Verify(x => x.Send(It.Is<AddCommand<ProjectCreateRequest>>(q => q.Request == createRequest),
                                            It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_SendsRequest()
        {
            // Arrange
            int id = 3;
            var responses = GetSampleProjectResponsesByUser();

            cache.SetString(recordKey, JsonSerializer.Serialize(responses));

            // Act
            var result = await controller.Delete(id);

            // Assert
            cache.Get(recordKey).Should().BeNull();
            result.Should().BeAssignableTo<NoContentResult>();

            MediatorMock.Verify(x => x.Send(It.Is<RemoveByIdCommand<Project>>(q => q.Id == id),
                                            It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_SendsRequest()
        {
            // Arrange
            var updateRequest = new ProjectUpdateRequest(5, "Inc", userId);
            var responses = GetSampleProjectResponsesByUser();

            cache.SetString(recordKey, JsonSerializer.Serialize(responses));

            // Act
            var result = await controller.Update(updateRequest);

            // Assert
            cache.Get(recordKey).Should().BeNull();
            result.Should().BeAssignableTo<NoContentResult>();

            MediatorMock.Verify(x => x.Send(It.Is<UpdateCommand<ProjectUpdateRequest>>(q => q.Request == updateRequest),
                                            It.IsAny<CancellationToken>()), Times.Once);
        }

        private List<ProjectResponse> GetSampleProjectResponsesByUser()
        {
            return new List<ProjectResponse>
            {
                new(2, "Something", userId),
                new(5, "Draft", userId)
            };
        }
    }
}
