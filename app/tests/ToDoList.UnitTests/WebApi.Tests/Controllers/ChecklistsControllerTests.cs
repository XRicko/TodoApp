using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Queries.Checklists;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Controllers;

using Xunit;

namespace ToDoList.UnitTests.WebApi.Controllers
{
    public class ChecklistsControllerTests : ApiControllerBaseForTests
    {
        private readonly ChecklistsController checklistsController;

        public ChecklistsControllerTests() : base()
        {
            checklistsController = new ChecklistsController(MediatorMock.Object);
        }

        [Fact]
        public async Task Get_ReturnsListOfChecklistResponsesByUser()
        {
            // Arrange
            int userId = 2;

            var checklists = new List<ChecklistResponse>
            {
                new(1, "Birthday", userId),
                new(2, "Chores", 36),
                new(2, "Birthday", 36),
                new(3, "Chores", userId),
            };

            var expected = checklists.Where(l => l.UserId == userId);

            var contextMock = new Mock<HttpContext>();
            checklistsController.ControllerContext.HttpContext = contextMock.Object;

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };

            contextMock.SetupGet(x => x.User.Claims)
                       .Returns(claims);
            MediatorMock.Setup(x => x.Send(It.Is<GetChecklistsByUserIdQuery>(q => q.UserId == userId), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected)
                        .Verifiable();

            // Act
            var actual = await checklistsController.Get();

            // Assert
            Assert.Equal(expected, actual);
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Add_SendsRequest()
        {
            // Arrange
            var createRequest = new ChecklistCreateRequest("Essential", 2);

            MediatorMock.Setup(x => x.Send(It.Is<AddCommand<ChecklistCreateRequest>>(q => q.Request == createRequest), It.IsAny<CancellationToken>()))
                        .Verifiable();

            // Act
            await checklistsController.Add(createRequest);

            // Assert
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Delete_SendsRequest()
        {
            // Arrange
            int id = 3;

            MediatorMock.Setup(x => x.Send(It.Is<RemoveCommand<Checklist>>(q => q.Id == id), It.IsAny<CancellationToken>()))
                        .Verifiable();

            // Act
            await checklistsController.Delete(id);

            // Assert
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Update_SendsRequest()
        {
            // Arrange
            var updateRequest = new ChecklistUpdateRequest(4, "Chores", 9);

            MediatorMock.Setup(x => x.Send(It.Is<UpdateCommand<ChecklistUpdateRequest>>(q => q.Request == updateRequest), It.IsAny<CancellationToken>()))
                        .Verifiable();

            // Act
            await checklistsController.Update(updateRequest);

            // Assert
            MediatorMock.Verify();
        }
    }
}
