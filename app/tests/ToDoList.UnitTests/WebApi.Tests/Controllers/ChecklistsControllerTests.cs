using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

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
        private readonly ChecklistsController checklistsController;

        private readonly int projectId;

        public ChecklistsControllerTests() : base()
        {
            checklistsController = new ChecklistsController(MediatorMock.Object);

            projectId = 5;
        }

        [Fact]
        public async Task Get_ReturnsNewListOfChecklistResponsesByProject()
        {
            // Arrange
            var expected = GetSampleChecklistResponsesByProject();

            MediatorMock.Setup(x => x.Send(new GetChecklistsByProjectIdQuery(projectId), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected)
                        .Verifiable();

            // Act
            var actual = await checklistsController.GetByProjectId(projectId);

            // Assert
            actual.Should().Equal(expected);

            MediatorMock.Verify();
        }

        [Fact]
        public async Task Get_ReturnsChecklistResponseGivenExistingId()
        {
            // Arrange
            int id = 131;
            var expected = new ChecklistResponse(id, "smth", 1);

            MediatorMock.Setup(x => x.Send(new GetByIdQuery<Checklist, ChecklistResponse>(id), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected)
                        .Verifiable();

            // Act
            var actual = await checklistsController.Get(id);

            // Assert
            actual.Value.Should().Be(expected);
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Get_ReturnsNullChecklistResponseGivenInvalidId()
        {
            // Arrange
            int id = 131;

            MediatorMock.Setup(x => x.Send(new GetByIdQuery<Checklist, ChecklistResponse>(id), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => null)
                        .Verifiable();

            // Act
            var actual = await checklistsController.Get(id);

            // Assert
            actual.Value.Should().BeNull();
            MediatorMock.Verify();
        }

        [Fact]
        public async Task GetByNameAndProjectId_ReturnsNullGivenInvalidData()
        {
            // Arrange
            string name = "Name";

            MediatorMock.Setup(x => x.Send(new GetChecklistByNameAndProjectIdQuery(name, projectId), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => null)
                        .Verifiable();

            // Act
            var actual = await checklistsController.GetByProjectIdAndName(name, projectId);

            // Assert
            actual.Value.Should().BeNull();
            MediatorMock.Verify();
        }

        [Fact]
        public async Task GetByNameAndProjectId_ReturnsChecklistResponseGivenValidData()
        {
            // Arrange
            string name = "Name";
            ChecklistResponse expected = new(42, name, projectId);

            MediatorMock.Setup(x => x.Send(new GetChecklistByNameAndProjectIdQuery(name, projectId), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected)
                        .Verifiable();

            // Act
            var actual = await checklistsController.GetByProjectIdAndName(name, projectId);

            // Assert
            actual.Value.Should().Be(expected);
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Add_SendsRequest()
        {
            // Arrange
            var createRequest = new ChecklistCreateRequest("Essential", 2);
            var responses = GetSampleChecklistResponsesByProject();

            // Act
            var result = await checklistsController.Add(createRequest);

            // Assert
            result.Should().BeAssignableTo<NoContentResult>();

            MediatorMock.Verify(x => x.Send(new AddCommand<ChecklistCreateRequest>(createRequest), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_SendsRequest()
        {
            // Arrange
            int id = 3;
            var responses = GetSampleChecklistResponsesByProject();

            // Act
            var result = await checklistsController.Delete(id);

            // Assert
            result.Should().BeAssignableTo<NoContentResult>();

            MediatorMock.Verify(x => x.Send(new RemoveByIdCommand<Checklist>(id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_SendsRequest()
        {
            // Arrange
            var updateRequest = new ChecklistUpdateRequest(4, "Chores", 9);
            var responses = GetSampleChecklistResponsesByProject();

            // Act
            var result = await checklistsController.Update(updateRequest);

            // Assert
            result.Should().BeAssignableTo<NoContentResult>();

            MediatorMock.Verify(x => x.Send(new UpdateCommand<ChecklistUpdateRequest>(updateRequest), It.IsAny<CancellationToken>()), Times.Once);
        }


        private List<ChecklistResponse> GetSampleChecklistResponsesByProject()
        {
            return new List<ChecklistResponse>
            {
                new(1, "Birthday", projectId),
                new(3, "Chores", projectId)
            };
        }
    }
}
