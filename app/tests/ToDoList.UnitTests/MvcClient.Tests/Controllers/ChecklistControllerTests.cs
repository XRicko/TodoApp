using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using Moq;

using ToDoList.MvcClient.Controllers;
using ToDoList.MvcClient.Services;
using ToDoList.MvcClient.ViewModels;
using ToDoList.SharedClientLibrary;
using ToDoList.SharedClientLibrary.Models;

using Xunit;

namespace MvcClient.Tests.Controllers
{
    public class ChecklistControllerTests : MvcControllerBaseForTests
    {
        private readonly Mock<IViewModelService> viewModelServiceMock;

        private readonly ChecklistController checklistController;

        private readonly int projectId;

        private readonly string createOrUpdateViewName;
        private readonly string viewAllViewName;

        public ChecklistControllerTests() : base()
        {
            viewModelServiceMock = new Mock<IViewModelService>();
            checklistController = new ChecklistController(ApiInvokerMock.Object, viewModelServiceMock.Object);

            projectId = 5;

            createOrUpdateViewName = "CreateOrUpdate";
            viewAllViewName = "_ViewAll";
        }

        [Fact]
        public async Task CreateOrUpdate_ReturnsCreateViewWithNewChecklistGivenNoName()
        {
            // Act
            var result = await checklistController.CreateOrUpdateAsync(projectId) as ViewResult;
            var checklist = (ChecklistModel)result.ViewData.Model;

            // Assert
            result.ViewName.Should().Be(createOrUpdateViewName);
            checklist.ProjectId.Should().Be(projectId);
        }

        [Fact]
        public async Task Get_CreateOrUpdate_ReturnsCreateViewWithExistingChecklistGivenExistingName()
        {
            // Arrange
            string name = "Chores";
            var checklist = new ChecklistModel { Id = 3, Name = name, ProjectId = projectId };

            ApiInvokerMock.Setup(x => x.GetItemAsync<ChecklistModel>(
                $"{ApiEndpoints.ChecklistByProjectIdAndName}?name={name}&projectId={projectId}"))
                          .ReturnsAsync(checklist)
                          .Verifiable();

            // Act
            var result = await checklistController.CreateOrUpdateAsync(projectId, name) as ViewResult;
            var modelFromView = (ChecklistModel)result.ViewData.Model;

            // Assert
            modelFromView.Should().BeEquivalentTo(checklist);
            result.ViewName.Should().Be(createOrUpdateViewName);

            ApiInvokerMock.Verify();
        }

        [Fact]
        public async Task Get_CreateOrUpdate_ReturnsNotFoundGivenInvalidName()
        {
            // Arrange
            string invalidName = "invalid";

            ApiInvokerMock.Setup(x => x.GetItemAsync<ChecklistModel>(
                $"{ApiEndpoints.ChecklistByProjectIdAndName}?name={invalidName}&projectId={projectId}"))
                          .ReturnsAsync(() => null)
                          .Verifiable();

            // Act
            var result = await checklistController.CreateOrUpdateAsync(projectId, invalidName) as NotFoundResult;

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            ApiInvokerMock.Verify();
        }

        [Fact]
        public async Task Post_CreateOrUpdate_ReturnsSameViewGivenInvalidModel()
        {
            // Arrange
            var invalidModel = new ChecklistModel { Id = 2, ProjectId = 6 };
            checklistController.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await checklistController.CreateOrUpdateAsync(invalidModel) as PartialViewResult;
            var modelFromView = (ChecklistModel)result.ViewData.Model;

            // Assert
            result.ViewName.Should().Be(createOrUpdateViewName);
            modelFromView.Should().BeSameAs(invalidModel);
        }

        [Fact]
        public async Task Post_CreateOrUpdate_ReturnsViewAllWithAddedChecklistGivenNew()
        {
            // Arrange
            var checklists = new List<ChecklistModel> { new ChecklistModel { Id = 1, Name = "Birthday", ProjectId = projectId } };
            var newChecklist = new ChecklistModel { Id = 0, Name = "Chores", ProjectId = projectId };

            IndexViewModel indexViewModel = new();

            ApiInvokerMock.Setup(x => x.PostItemAsync(ApiEndpoints.Checklists, newChecklist))
                          .Callback(() =>
                          {
                              newChecklist.Id = checklists.Last().Id++;
                              checklists.Add(newChecklist);
                              indexViewModel.ChecklistModels = checklists;
                          });
            viewModelServiceMock.Setup(x => x.CreateIndexViewModelAsync(null, null))
                                .ReturnsAsync(indexViewModel)
                                .Verifiable();

            // Act
            var result = await checklistController.CreateOrUpdateAsync(newChecklist) as PartialViewResult;
            var viewModel = (IndexViewModel)result.ViewData.Model;

            // Assert
            viewModel.ChecklistModels.Should().Contain(newChecklist);
            result.ViewName.Should().Be(viewAllViewName);

            ApiInvokerMock.VerifyAll();
            viewModelServiceMock.Verify();
        }

        [Fact]
        public async Task Post_CreateOrUpdate_ReturnsViewAllWithUpdatedChecklistGivenExisting()
        {
            // Arrange
            var checklistToUpdate = new ChecklistModel { Id = 2, Name = "Chores", ProjectId = projectId };
            var checklists = new List<ChecklistModel> { new ChecklistModel { Id = 2, Name = "Something", ProjectId = projectId } };

            IndexViewModel indexViewModel = new();

            ApiInvokerMock.Setup(x => x.PutItemAsync(ApiEndpoints.Checklists, checklistToUpdate))
                          .Callback(() =>
                          {
                              checklists.SingleOrDefault(c => c.Id == checklistToUpdate.Id).Name = checklistToUpdate.Name;
                              indexViewModel.ChecklistModels = checklists;
                          });
            viewModelServiceMock.Setup(x => x.CreateIndexViewModelAsync(null, null))
                                .ReturnsAsync(indexViewModel)
                                .Verifiable();

            // Act
            var result = await checklistController.CreateOrUpdateAsync(checklistToUpdate) as PartialViewResult;
            var viewModel = (IndexViewModel)result.ViewData.Model;

            // Assert
            viewModel.ChecklistModels.Should().ContainEquivalentOf(checklistToUpdate);
            result.ViewName.Should().Be(viewAllViewName);

            ApiInvokerMock.VerifyAll();
            viewModelServiceMock.Verify();
        }

        [Fact]
        public async Task Post_Delete_ReturnsViewAllWithDeletedChecklist()
        {
            // Arrange
            var checklists = new List<ChecklistModel>
            {
                new ChecklistModel { Id = 1, Name = "somethuing", ProjectId = projectId },
                new ChecklistModel { Id = 2, Name = "ToBeDeleted", ProjectId = projectId }
            };
            int idToDelete = 2;

            IndexViewModel indexViewModel = new();

            ApiInvokerMock.Setup(x => x.DeleteItemAsync($"{ApiEndpoints.Checklists}/{idToDelete}"))
                          .Callback(() =>
                          {
                              checklists.Remove(checklists.SingleOrDefault(c => c.Id == idToDelete));
                              indexViewModel.ChecklistModels = checklists;
                          });
            viewModelServiceMock.Setup(x => x.CreateIndexViewModelAsync(null, null)).ReturnsAsync(indexViewModel).Verifiable();

            // Act
            var result = await checklistController.DeleteAsync(idToDelete) as PartialViewResult;
            var viewModel = (IndexViewModel)result.ViewData.Model;

            result.ViewName.Should().Be(viewAllViewName);
            viewModel.ChecklistModels.Should().NotContain(x => x.Id == idToDelete);

            ApiInvokerMock.VerifyAll();
            viewModelServiceMock.Verify();
        }
    }
}
