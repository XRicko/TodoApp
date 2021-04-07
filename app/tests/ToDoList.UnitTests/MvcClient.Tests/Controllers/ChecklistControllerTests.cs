using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Moq;

using ToDoList.MvcClient.Controllers;
using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.Services;
using ToDoList.MvcClient.ViewModels;

using Xunit;

namespace ToDoList.UnitTests.MvcClient.Controllers
{
    public class ChecklistControllerTests : MvcControllerBaseForTests
    {
        private readonly Mock<ICreateViewModelService> viewModelServiceMock;

        private readonly ChecklistController checklistController;

        private readonly int userId;
        private readonly string route;

        private readonly string createOrUpdateViewName;
        private readonly string viewAllViewName;

        public ChecklistControllerTests() : base()
        {
            viewModelServiceMock = new Mock<ICreateViewModelService>();

            checklistController = new ChecklistController(ApiCallsServiceMock.Object, viewModelServiceMock.Object);

            userId = 5;
            route = "Checklists";

            createOrUpdateViewName = "CreateOrUpdate";
            viewAllViewName = "_ViewAll";
        }

        [Fact]
        public async Task CreateOrUpdate_ReturnsCreateViewWithNewChecklistGivenNewId()
        {
            // Act
            var result = await checklistController.CreateOrUpdateAsync(0, userId) as ViewResult;
            var checklist = (ChecklistModel)result.ViewData.Model;

            // Assert
            Assert.Equal(createOrUpdateViewName, result.ViewName);
            Assert.Equal(userId, checklist.UserId);
        }

        [Fact]
        public async Task Get_CreateOrUpdate_ReturnsCreateViewWithExistingChecklistGivenExistingId()
        {
            // Arrange
            int existingChecklistId = 3;
            var checklist = new ChecklistModel { Id = 3, Name = "Chores", UserId = userId };

            string routeWithId = $"{route}/{checklist.Id}";

            ApiCallsServiceMock.Setup(x => x.GetItemAsync<ChecklistModel>(routeWithId))
                               .ReturnsAsync(checklist);

            // Act
            var result = await checklistController.CreateOrUpdateAsync(existingChecklistId, userId) as ViewResult;
            var viewModel = (ChecklistModel)result.ViewData.Model;

            // Assert
            Assert.Same(checklist, viewModel);
            Assert.Equal(createOrUpdateViewName, result.ViewName);

            ApiCallsServiceMock.Verify();
        }

        [Fact]
        public async Task Get_CreateOrUpdate_ReturnsNotFoundGivenInvalidId()
        {
            // Arrange
            int invalidId = 45;
            string routeWithId = $"{route}/{invalidId}";

            ApiCallsServiceMock.Setup(x => x.GetItemAsync<ChecklistModel>(routeWithId))
                              .ReturnsAsync(() => null);

            // Act
            var result = await checklistController.CreateOrUpdateAsync(invalidId, userId) as NotFoundResult;

            // Assert
            Assert.Equal(404, result.StatusCode);
            ApiCallsServiceMock.Verify();
        }

        [Fact]
        public async Task Post_CreateOrUpdate_ReturnsSameViewGivenInvalidModel()
        {
            // Arrange
            var invalidModel = new ChecklistModel { Id = 2, UserId = 6 };
            checklistController.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await checklistController.CreateOrUpdateAsync(invalidModel) as PartialViewResult;
            var viewModel = (ChecklistModel)result.ViewData.Model;

            // Assert
            Assert.Equal(createOrUpdateViewName, result.ViewName);
            Assert.Same(invalidModel, viewModel);
        }

        [Fact]
        public async Task Post_CreateOrUpdate_ReturnsViewAllWithAddedChecklistGivenNew()
        {
            // Arrange
            var checklists = new List<ChecklistModel> { new ChecklistModel { Id = 1, Name = "Birthday", UserId = userId } };
            var newChecklist = new ChecklistModel { Id = 0, Name = "Chores", UserId = userId };

            IndexViewModel indexViewModel = new();

            ApiCallsServiceMock.Setup(x => x.PostItemAsync(route, newChecklist))
                               .Callback(() =>
                               {
                                   newChecklist.Id = checklists.Last().Id++;
                                   checklists.Add(newChecklist);
                                   indexViewModel.ChecklistModels = checklists;
                               });
            viewModelServiceMock.Setup(x => x.CreateIndexViewModelAsync())
                                .ReturnsAsync(indexViewModel);

            // Act
            var result = await checklistController.CreateOrUpdateAsync(newChecklist) as PartialViewResult;
            var viewModel = (IndexViewModel)result.ViewData.Model;

            // Assert
            Assert.Contains(newChecklist, viewModel.ChecklistModels);
            Assert.Equal(viewAllViewName, result.ViewName);

            ApiCallsServiceMock.Verify();
            viewModelServiceMock.Verify();
        }

        [Fact]
        public async Task Post_CreateOrUpdate_ReturnsViewAllWithUpdatedChecklistGivenExisting()
        {
            // Arrange
            var checklistToUpdate = new ChecklistModel { Id = 2, Name = "Chores", UserId = userId };
            var checklists = new List<ChecklistModel> { new ChecklistModel { Id = 2, Name = "Something", UserId = userId } };

            IndexViewModel indexViewModel = new();

            ApiCallsServiceMock.Setup(x => x.PutItemAsync(route, checklistToUpdate))
                               .Callback(() =>
                               {
                                   checklists.SingleOrDefault(c => c.Id == checklistToUpdate.Id).Name = checklistToUpdate.Name;
                                   indexViewModel.ChecklistModels = checklists;
                               });
            viewModelServiceMock.Setup(x => x.CreateIndexViewModelAsync())
                                .ReturnsAsync(indexViewModel);

            // Act
            var result = await checklistController.CreateOrUpdateAsync(checklistToUpdate) as PartialViewResult;
            var viewModel = (IndexViewModel)result.ViewData.Model;

            // Assert
            Assert.Equal(checklistToUpdate.Name, viewModel.ChecklistModels.SingleOrDefault(c => c.Id == checklistToUpdate.Id).Name);
            Assert.Equal(viewAllViewName, result.ViewName);

            ApiCallsServiceMock.Verify();
            viewModelServiceMock.Verify();
        }

        [Fact]
        public async Task Post_Delete_ReturnsViewAllWithDeletedChecklist()
        {
            // Arrange
            var checklists = new List<ChecklistModel>
            {
                new ChecklistModel { Id = 1, Name = "somethuing", UserId = userId },
                new ChecklistModel { Id = 2, Name = "ToBeDeleted", UserId = userId }
            };
            int idToDelete = 2;

            IndexViewModel indexViewModel = new();

            ApiCallsServiceMock.Setup(x => x.DeleteItemAsync(route + "/", idToDelete))
                               .Callback(() =>
                               {
                                   checklists.Remove(checklists.SingleOrDefault(c => c.Id == idToDelete));
                                   indexViewModel.ChecklistModels = checklists;
                               });
            viewModelServiceMock.Setup(x => x.CreateIndexViewModelAsync())
                               .ReturnsAsync(indexViewModel);

            // Act
            var result = await checklistController.DeleteAsync(idToDelete) as PartialViewResult;
            var viewModel = (IndexViewModel)result.ViewData.Model;

            Assert.Equal(viewAllViewName, result.ViewName);
            Assert.DoesNotContain(It.Is<ChecklistModel>(m => m.Id == idToDelete), viewModel.ChecklistModels);

            ApiCallsServiceMock.Verify();
            viewModelServiceMock.Verify();
        }
    }
}
