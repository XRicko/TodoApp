using System.Collections.Generic;
using System.Security.Claims;
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
using ToDoList.Core.Mediator.Queries.Checklists;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Controllers;

using Xunit;

namespace WebApi.Tests.Controllers
{
    public class ChecklistsControllerTests : ApiControllerBaseForTests
    {
        private readonly IDistributedCache cache;
        private readonly ChecklistsController checklistsController;

        private readonly int userId;
        private readonly string recordKey;

        public ChecklistsControllerTests() : base()
        {
            var opts = Options.Create(new MemoryDistributedCacheOptions());
            cache = new MemoryDistributedCache(opts);

            checklistsController = new ChecklistsController(MediatorMock.Object, cache);

            var contextMock = new Mock<HttpContext>();
            checklistsController.ControllerContext.HttpContext = contextMock.Object;

            userId = 2;
            recordKey = $"Checklists_User_{userId}";

            var claim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());

            contextMock.Setup(x => x.User.FindFirst(ClaimTypes.NameIdentifier))
                       .Returns(claim);
        }

        [Fact]
        public async Task Get_ReturnsNewListOfChecklistResponsesByUserAndSetsCache()
        {
            // Arrange
            var expected = GetSampleChecklistResponsesByUser();

            MediatorMock.Setup(x => x.Send(It.Is<GetChecklistsByUserIdQuery>(q => q.UserId == userId),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected)
                        .Verifiable();

            // Act
            var actual = await checklistsController.Get();
            var cached = JsonSerializer.Deserialize<List<ChecklistResponse>>(cache.GetString(recordKey));

            // Assert
            actual.Should().Equal(expected);
            cached.Should().Equal(expected);

            MediatorMock.Verify();
        }

        [Fact]
        public async Task Get_ReturnsListOfChecklistResponsesByUserFromCache()
        {
            // Arrange
            var expected = GetSampleChecklistResponsesByUser();

            cache.SetString(recordKey, JsonSerializer.Serialize(expected));

            // Act
            var actual = await checklistsController.Get();

            // Assert
            actual.Should().Equal(expected);
            MediatorMock.Verify(x => x.Send(It.Is<GetChecklistsByUserIdQuery>(q => q.UserId == userId),
                                            It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Get_ReturnsChecklistResponseGivenExistingId()
        {
            // Arrange
            int id = 131;
            var expected = new ChecklistResponse(id, "smth", 1);

            MediatorMock.Setup(x => x.Send(It.Is<GetByIdQuery<Checklist, ChecklistResponse>>(q => q.Id == id),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected)
                        .Verifiable();

            // Act
            var actual = await checklistsController.Get(id);

            // Assert
            actual.Value.Should().Be(expected);
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Get_ReturnsChecklistResponseGivenInvalidId()
        {
            // Arrange
            int id = 131;

            MediatorMock.Setup(x => x.Send(It.Is<GetByIdQuery<Checklist, ChecklistResponse>>(q => q.Id == id),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => null)
                        .Verifiable();

            // Act
            var actual = await checklistsController.Get(id);

            // Assert
            actual.Value.Should().BeNull();
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Add_SendsRequest()
        {
            // Arrange
            var createRequest = new ChecklistCreateRequest("Essential", 2);
            var responses = GetSampleChecklistResponsesByUser();

            cache.SetString(recordKey, JsonSerializer.Serialize(responses));

            // Act
            var result = await checklistsController.Add(createRequest);

            // Assert
            cache.Get(recordKey).Should().BeNull();
            result.Should().BeAssignableTo<NoContentResult>();

            MediatorMock.Verify(x => x.Send(It.Is<AddCommand<ChecklistCreateRequest>>(q => q.Request == createRequest),
                                            It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_SendsRequest()
        {
            // Arrange
            int id = 3;
            var responses = GetSampleChecklistResponsesByUser();

            cache.SetString(recordKey, JsonSerializer.Serialize(responses));

            // Act
            var result = await checklistsController.Delete(id);

            // Assert
            cache.Get(recordKey).Should().BeNull();
            result.Should().BeAssignableTo<NoContentResult>();

            MediatorMock.Verify(x => x.Send(It.Is<RemoveCommand<Checklist>>(q => q.Id == id),
                                            It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_SendsRequest()
        {
            // Arrange
            var updateRequest = new ChecklistUpdateRequest(4, "Chores", 9);
            var responses = GetSampleChecklistResponsesByUser();

            cache.SetString(recordKey, JsonSerializer.Serialize(responses));

            // Act
            var result = await checklistsController.Update(updateRequest);

            // Assert
            cache.Get(recordKey).Should().BeNull();
            result.Should().BeAssignableTo<NoContentResult>();

            MediatorMock.Verify(x => x.Send(It.Is<UpdateCommand<ChecklistUpdateRequest>>(q => q.Request == updateRequest),
                                            It.IsAny<CancellationToken>()), Times.Once);
        }


        private List<ChecklistResponse> GetSampleChecklistResponsesByUser()
        {
            return new List<ChecklistResponse>
            {
                new(1, "Birthday", userId),
                new(3, "Chores", userId)
            };
        }
    }
}
