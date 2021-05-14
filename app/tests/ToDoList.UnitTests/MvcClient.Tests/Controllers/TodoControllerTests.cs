using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Moq;

using ToDoList.MvcClient.Controllers;
using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.Services;
using ToDoList.MvcClient.ViewModels;
using ToDoList.SharedKernel;

using Xunit;

namespace MvcClient.Tests.Controllers
{
    public class TodoControllerTests : MvcControllerBaseForTests
    {
        private readonly TodoController todoController;

        private readonly Mock<IImageAddingService> imageAddingServiceMock;
        private readonly Mock<IViewModelService> createViewModelServiceMock;

        private readonly string createOrUpdateViewName;
        private readonly string viewAllViewName;

        private readonly string route;
        private readonly int checklistId;


        public TodoControllerTests()
        {
            imageAddingServiceMock = new Mock<IImageAddingService>();
            createViewModelServiceMock = new Mock<IViewModelService>();

            todoController = new TodoController(ApiCallsServiceMock.Object, imageAddingServiceMock.Object, createViewModelServiceMock.Object);

            createOrUpdateViewName = "CreateOrUpdate";
            viewAllViewName = "_ViewAll";

            route = "TodoItems";
            checklistId = 1;
        }

        [Fact]
        public async Task Get_Index_ReturnsIndexViewModelWithAll()
        {
            // Arrange
            var indexViewModel = GetIndexViewModel();

            createViewModelServiceMock.Setup(x => x.CreateIndexViewModelAsync(null, null))
                                      .ReturnsAsync(indexViewModel)
                                      .Verifiable();

            // Act
            var result = await todoController.IndexAsync() as ViewResult;
            var viewModel = (IndexViewModel)result.Model;

            // Assert
            Assert.Equal("Index", result.ViewName);
            Assert.Equal(indexViewModel, viewModel);

            createViewModelServiceMock.Verify();
        }

        [Fact]
        public async Task Get_Index_ReturnsIndexViewModelByCategory()
        {
            // Arrange
            string category = "Important";
            var indexViewModel = GetIndexViewModel(category);

            createViewModelServiceMock.Setup(x => x.CreateIndexViewModelAsync(category, null))
                                      .ReturnsAsync(indexViewModel)
                                      .Verifiable();

            // Act
            var result = await todoController.IndexAsync(category) as ViewResult;
            var viewModel = (IndexViewModel)result.Model;

            // Assert
            Assert.Equal("Index", result.ViewName);
            Assert.Equal(indexViewModel, viewModel);
            Assert.Equal(category, viewModel.SelectedCategory);
            Assert.Null(viewModel.SelectedStatus);

            createViewModelServiceMock.Verify();
        }

        [Fact]
        public async Task Get_Index_ReturnsIndexViewModelByStatus()
        {
            // Arrange
            string status = "Planned";
            var indexViewModel = GetIndexViewModel(status: status);

            createViewModelServiceMock.Setup(x => x.CreateIndexViewModelAsync(null, status))
                                      .ReturnsAsync(indexViewModel)
                                      .Verifiable();

            // Act
            var result = await todoController.IndexAsync(statusName: status) as ViewResult;
            var viewModel = (IndexViewModel)result.Model;

            // Assert
            Assert.Equal("Index", result.ViewName);
            Assert.Equal(indexViewModel, viewModel);
            Assert.Equal(status, viewModel.SelectedStatus);
            Assert.Null(viewModel.SelectedCategory);

            createViewModelServiceMock.Verify();
        }

        [Fact]
        public async Task Get_Index_ReturnsIndexViewModelByCategoryAndStatus()
        {
            // Arrange
            string category = "Important";
            string status = "Planned";

            var indexViewModel = GetIndexViewModel(category, status);

            createViewModelServiceMock.Setup(x => x.CreateIndexViewModelAsync(category, status))
                                      .ReturnsAsync(indexViewModel)
                                      .Verifiable();

            // Act
            var result = await todoController.IndexAsync(category, status) as ViewResult;
            var viewModel = (IndexViewModel)result.Model;

            // Assert
            Assert.Equal("Index", result.ViewName);
            Assert.Equal(indexViewModel, viewModel);
            Assert.Equal(status, viewModel.SelectedStatus);
            Assert.Equal(category, viewModel.SelectedCategory);

            createViewModelServiceMock.Verify();
        }

        [Fact]
        public async Task Get_CreateOrUpdate_ReturnsCreateViewWithNewTodoItemGivenNewId()
        {
            // Arrange
            var createTodoItemViewModel = GetCreateTodoItemViewModel();

            createViewModelServiceMock.Setup(x => x.CreateViewModelCreateOrUpdateTodoItemAsync(It.Is<TodoItemModel>(m => m.Id == 0)))
                                      .ReturnsAsync((TodoItemModel m) =>
                                      {
                                          createTodoItemViewModel.TodoItemModel = m;
                                          return createTodoItemViewModel;
                                      })
                                      .Verifiable();

            // Act
            var result = await todoController.CreateOrUpdateAsync(checklistId) as ViewResult;
            var viewModel = (CreateTodoItemViewModel)result.ViewData.Model;

            // Assert
            Assert.Equal(createOrUpdateViewName, result.ViewName);
            Assert.Equal(checklistId, viewModel.TodoItemModel.ChecklistId);

            createViewModelServiceMock.Verify();
        }

        [Fact]
        public async Task Get_CreateOrUpdate_ReturnsCreateViewWithExistingTodoItemGivenExistingId()
        {
            // Arrange
            int existingId = 3;
            var existingTodoItem = new TodoItemModel { Id = existingId, Name = "Somehting", ChecklistId = checklistId, StatusId = 1 };

            string routeWithId = $"{route}/{existingId}";

            var createTodoItemViewModel = GetCreateTodoItemViewModel();

            ApiCallsServiceMock.Setup(x => x.GetItemAsync<TodoItemModel>(routeWithId))
                               .ReturnsAsync(existingTodoItem)
                               .Verifiable();

            createViewModelServiceMock.Setup(x => x.CreateViewModelCreateOrUpdateTodoItemAsync(existingTodoItem))
                                      .ReturnsAsync((TodoItemModel m) =>
                                      {
                                          createTodoItemViewModel.TodoItemModel = m;
                                          return createTodoItemViewModel;
                                      })
                                      .Verifiable();

            // Act
            var result = await todoController.CreateOrUpdateAsync(checklistId, existingId) as ViewResult;
            var viewModel = (CreateTodoItemViewModel)result.ViewData.Model;

            // Assert
            Assert.Equal(createOrUpdateViewName, result.ViewName);
            Assert.Equal(checklistId, viewModel.TodoItemModel.ChecklistId);
            Assert.Equal(existingId, viewModel.TodoItemModel.Id);

            ApiCallsServiceMock.Verify();
            createViewModelServiceMock.Verify();
        }

        [Fact]
        public async Task Get_CreateOrUpdate_ReturnsCreateViewWithExistingTodoItemWithGeoPointGivenExistingId()
        {
            // Arrange
            double lat = 34.12345;
            double lng = 54.76547;

            int existingId = 3;
            var existingTodoItem = new TodoItemModel
            {
                Id = existingId,
                Name = "Somehting",
                ChecklistId = checklistId,
                StatusId = 1,
                GeoPoint = new GeoCoordinate(lng, lat)
            };

            string routeWithId = $"{route}/{existingId}";

            var createTodoItemViewModel = GetCreateTodoItemViewModel();

            ApiCallsServiceMock.Setup(x => x.GetItemAsync<TodoItemModel>(routeWithId))
                               .ReturnsAsync(existingTodoItem)
                               .Verifiable();

            createViewModelServiceMock.Setup(x => x.CreateViewModelCreateOrUpdateTodoItemAsync(existingTodoItem))
                                      .ReturnsAsync((TodoItemModel m) =>
                                      {
                                          createTodoItemViewModel.TodoItemModel = m;
                                          return createTodoItemViewModel;
                                      })
                                      .Verifiable();

            // Act
            var result = await todoController.CreateOrUpdateAsync(checklistId, existingId) as ViewResult;
            var viewModel = (CreateTodoItemViewModel)result.ViewData.Model;

            // Assert
            Assert.Equal(createOrUpdateViewName, result.ViewName);
            Assert.Equal(checklistId, viewModel.TodoItemModel.ChecklistId);

            Assert.Equal(lat.ToString(), viewModel.TodoItemModel.Latitude);
            Assert.Equal(lng.ToString(), viewModel.TodoItemModel.Longitude);

            ApiCallsServiceMock.Verify();
            createViewModelServiceMock.Verify();
        }

        [Fact]
        public async Task Get_CreateOrUpdate_ReturnsNotFoundGivenInvalidId()
        {
            // Arrange
            int invalidId = 45;
            string routeWithId = $"{route}/{invalidId}";

            ApiCallsServiceMock.Setup(x => x.GetItemAsync<TodoItemModel>(routeWithId))
                               .ReturnsAsync(() => null)
                               .Verifiable();

            // Act
            var result = await todoController.CreateOrUpdateAsync(checklistId, invalidId) as NotFoundResult;

            // Assert
            Assert.Equal(404, result.StatusCode);
            ApiCallsServiceMock.Verify();
        }

        [Fact]
        public async Task Post_CreateOrUpdate_ReturnsViewAllWithAddedTodoItemGivenNew()
        {
            // Arrange
            double lat = 34.12345;
            double lng = 54.76547;

            GeoCoordinate geoCoordinate = new(lng, lat);

            var category = new CategoryModel { Id = 1, Name = "Important" };

            var todoItems = new List<TodoItemModel>
            {
                new TodoItemModel { Id = 1, Name = "Read a book", ChecklistId = checklistId }
            };

            var newTodoItem = new TodoItemModel
            {
                Id = 0,
                Name = "Wath some film",
                ChecklistId = checklistId,

                CategoryId = category.Id,
                CategoryName = category.Name,

                Latitude = lat.ToString(),
                Longitude = lng.ToString()
            };

            var createTodoItemViewModel = GetCreateTodoItemViewModel();
            createTodoItemViewModel.TodoItemModel = newTodoItem;

            IndexViewModel indexViewModel = new();

            ApiCallsServiceMock.Setup(x => x.PostItemAsync(route, newTodoItem))
                               .Callback(() =>
                               {
                                   newTodoItem.Id = todoItems.Last().Id++;
                                   todoItems.Add(newTodoItem);
                                   indexViewModel.TodoItems = todoItems;
                               });
            ApiCallsServiceMock.Setup(x => x.GetItemAsync<CategoryModel>("Categories/GetByName/" + category.Name))
                               .ReturnsAsync(category);

            createViewModelServiceMock.Setup(x => x.CreateIndexViewModelAsync(null, null))
                                      .ReturnsAsync(indexViewModel)
                                      .Verifiable();

            // Act
            var result = await todoController.CreateOrUpdateAsync(createTodoItemViewModel) as PartialViewResult;
            var viewModel = (IndexViewModel)result.ViewData.Model;

            var todoItemWithGeoPoint = viewModel.TodoItems.SingleOrDefault(i => i.GeoPoint is not null);

            // Assert
            Assert.Contains(newTodoItem, viewModel.TodoItems);

            Assert.Equal(viewAllViewName, result.ViewName);
            Assert.Equal(category.Id, newTodoItem.CategoryId);
            Assert.Equal(geoCoordinate, todoItemWithGeoPoint.GeoPoint);

            ApiCallsServiceMock.VerifyAll();
            ApiCallsServiceMock.Verify(x => x.PostItemAsync("Categories", It.Is<CategoryModel>(m => m.Name == category.Name)), Times.Once);

            createViewModelServiceMock.Verify();
        }

        [Fact]
        public async Task Post_CreateOrUpdate_ReturnsViewAllWithUpdatedTodoItemGivenExisting()
        {
            // Arrange
            var todoItemToUpdate = new TodoItemModel { Id = 2, Name = "Clean my room", ChecklistId = checklistId };
            var todoItems = new List<TodoItemModel> { new TodoItemModel { Id = 2, Name = "Do nothing", ChecklistId = checklistId } };

            var createTodoItemViewModel = GetCreateTodoItemViewModel();
            createTodoItemViewModel.TodoItemModel = todoItemToUpdate;

            IndexViewModel indexViewModel = new();

            ApiCallsServiceMock.Setup(x => x.PutItemAsync(route, todoItemToUpdate))
                               .Callback(() =>
                               {
                                   todoItems.SingleOrDefault(c => c.Id == todoItemToUpdate.Id).Name = todoItemToUpdate.Name;
                                   indexViewModel.TodoItems = todoItems;
                               });

            createViewModelServiceMock.Setup(x => x.CreateIndexViewModelAsync(null, null))
                                      .ReturnsAsync(indexViewModel)
                                      .Verifiable();

            // Act
            var result = await todoController.CreateOrUpdateAsync(createTodoItemViewModel) as PartialViewResult;
            var viewModel = (IndexViewModel)result.ViewData.Model;

            // Assert
            Assert.Equal(todoItemToUpdate.Name, viewModel.TodoItems.SingleOrDefault(c => c.Id == todoItemToUpdate.Id).Name);
            Assert.Equal(viewAllViewName, result.ViewName);

            ApiCallsServiceMock.VerifyAll();
            createViewModelServiceMock.Verify();
        }

        [Fact]
        public async Task Post_Delete_ReturnsViewAllWithDeletedTodoItem()
        {
            // Arrange
            var todoItems = new List<TodoItemModel>
            {
                new TodoItemModel { Id = 1, Name = "somethuing", ChecklistId = checklistId },
                new TodoItemModel { Id = 2, Name = "ToBeDeleted", ChecklistId = checklistId }
            };
            int idToDelete = 2;

            IndexViewModel indexViewModel = new();

            ApiCallsServiceMock.Setup(x => x.DeleteItemAsync(route + "/", idToDelete))
                               .Callback(() =>
                               {
                                   todoItems.Remove(todoItems.SingleOrDefault(c => c.Id == idToDelete));
                                   indexViewModel.TodoItems = todoItems;
                               });

            createViewModelServiceMock.Setup(x => x.CreateIndexViewModelAsync(null, null))
                                      .ReturnsAsync(indexViewModel)
                                      .Verifiable();

            // Act
            var result = await todoController.DeleteAsync(idToDelete) as PartialViewResult;
            var viewModel = (IndexViewModel)result.ViewData.Model;

            Assert.Same(indexViewModel, viewModel);
            Assert.Equal(viewAllViewName, result.ViewName);
            Assert.DoesNotContain(It.Is<TodoItemModel>(m => m.Id == idToDelete), todoItems);

            ApiCallsServiceMock.VerifyAll();
            createViewModelServiceMock.Verify();
        }

        [Fact]
        public async Task Post_MarkTodoItem_ReturnsViewAllWithDoneTodoItemsGivenTrue()
        {
            await TestMarkTodoItem(true);
        }

        [Fact]
        public async Task Post_MarkTodoItem_ReturnsViewAllWithActiveTodoItemsGivenFalse()
        {
            await TestMarkTodoItem(false);
        }

        private async Task TestMarkTodoItem(bool isDone)
        {
            // Arrange
            var todoItem = new TodoItemModel { Id = 1, Name = "Read a book", ChecklistId = checklistId };
            var todoItems = new List<TodoItemModel> { todoItem };

            string statusName = isDone ? "Done" : "Ongoing";
            var status = new StatusModel { Id = 3, Name = statusName, IsDone = isDone };

            string routeWithId = $"{route}/{todoItem.Id}";
            string routeForStatus = "Statuses/GetByName/" + status.Name;

            IndexViewModel indexViewModel = new();

            ApiCallsServiceMock.Setup(x => x.GetItemAsync<TodoItemModel>(routeWithId))
                               .ReturnsAsync(todoItem);
            ApiCallsServiceMock.Setup(x => x.GetItemAsync<StatusModel>(routeForStatus))
                               .ReturnsAsync(status);

            ApiCallsServiceMock.Setup(x => x.PutItemAsync(route, todoItem))
                             .Callback((string s, TodoItemModel m) =>
                             {
                                 todoItems.SingleOrDefault(c => c.Id == todoItem.Id).StatusId = m.StatusId;
                                 indexViewModel.TodoItems = todoItems;
                             });

            createViewModelServiceMock.Setup(x => x.CreateIndexViewModelAsync(null, null))
                                      .ReturnsAsync(indexViewModel)
                                      .Verifiable();

            // Act
            var result = await todoController.MarkTodoItemAsync(todoItem.Id, isDone) as PartialViewResult;
            var viewModel = (IndexViewModel)result.ViewData.Model;

            Assert.Same(indexViewModel, viewModel);
            Assert.Equal(viewAllViewName, result.ViewName);

            Assert.NotNull(indexViewModel.TodoItems);
            Assert.NotEmpty(indexViewModel.TodoItems);

            ApiCallsServiceMock.VerifyAll();
            createViewModelServiceMock.Verify();
        }

        private static IndexViewModel GetIndexViewModel(string category = null, string status = null)
        {
            return new IndexViewModel
            {
                TodoItems = new List<TodoItemModel>(),
                ChecklistModels = new List<ChecklistModel> { new ChecklistModel { Id = 1, Name = "Chores" } },

                SelectedCategory = category,
                SelectedStatus = status
            };
        }

        private CreateTodoItemViewModel GetCreateTodoItemViewModel()
        {
            var checklists = new List<ChecklistModel> { new ChecklistModel { Id = checklistId, Name = "Chores" } };
            var categories = new List<CategoryModel> { new CategoryModel { Id = 2, Name = "Insad" } };
            var statuses = new List<StatusModel> { new StatusModel { Id = 1, Name = "Planned", IsDone = false } };

            var createTodoItemViewModel = new CreateTodoItemViewModel
            {
                ChecklistModels = checklists,
                CategoryModels = categories,
                StatusModels = statuses,
                SelectedChecklistId = checklistId,
                SelectedStatusId = 1
            };

            return createTodoItemViewModel;
        }
    }
}
