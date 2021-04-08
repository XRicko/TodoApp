using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.Services;
using ToDoList.MvcClient.Services.Api;

using Xunit;

namespace ToDoList.UnitTests.MvcClient.Services
{
    public class CreateViewModelServiceTests
    {
        private readonly Mock<IApiInvoker> apiCallsServiceMock;
        private readonly CreateViewModelService createViewModelService;

        private readonly List<ChecklistModel> checklists;

        public CreateViewModelServiceTests()
        {
            apiCallsServiceMock = new Mock<IApiInvoker>();
            createViewModelService = new CreateViewModelService(apiCallsServiceMock.Object);

            checklists = GetChecklistsSample();

            apiCallsServiceMock.Setup(x => x.GetItemsAsync<ChecklistModel>("Checklists"))
                               .ReturnsAsync(checklists)
                               .Verifiable();
        }

        [Fact]
        public async Task CreateIndexViewModel_ReturnsIndexViewModel()
        {
            // Arrange
            var activeTodoItems = new List<TodoItemModel>
            {
                new TodoItemModel {Id = 5, Name = "Prepare for exam", ChecklistId = 1, StatusId = 1 },
                new TodoItemModel {Id = 10, Name = "Prepare a gift", ChecklistId = 2, StatusId = 2 },
                new TodoItemModel {Id = 15, Name = "Clean up the room", ChecklistId = 3, StatusId = 1 }
            };

            var doneTodoItems = new List<TodoItemModel>
            {
                new TodoItemModel {Id = 1, Name = "Visit", ChecklistId = 1, StatusId = 3 },
                new TodoItemModel {Id = 9, Name = "Choose some gift", ChecklistId = 2, StatusId = 3 },
                new TodoItemModel {Id = 14, Name = "Change smth", ChecklistId = 3, StatusId = 3 }
            };

            apiCallsServiceMock.Setup(x => x.GetItemsAsync<TodoItemModel>("TodoItems/GetActiveOrDone/" + false))
                               .ReturnsAsync(activeTodoItems)
                               .Verifiable();
            apiCallsServiceMock.Setup(x => x.GetItemsAsync<TodoItemModel>("TodoItems/GetActiveOrDone/" + true))
                               .ReturnsAsync(doneTodoItems)
                               .Verifiable();

            // Act
            var indexViewModel = await createViewModelService.CreateIndexViewModelAsync();

            // Assert
            Assert.Equal(activeTodoItems, indexViewModel.ActiveTodoItems);
            Assert.Equal(doneTodoItems, indexViewModel.DoneTodoItems);
            Assert.Equal(checklists, indexViewModel.ChecklistModels);

            apiCallsServiceMock.Verify();
        }

        [Fact]
        public async Task CreateViewModelCreateOrUpdateTodoItem_ReturnsViewModelBasedOnTodoItem()
        {
            // Arrange
            var statuses = GetStatusesSample();
            var categorires = GetCategoriesSample();

            int selectedChecklist = 2;
            int selectedCategory = 3;

            var statusPlanned = statuses.SingleOrDefault(s => s.Name == "Planned");

            var todoItem = new TodoItemModel { Name = "Do something", ChecklistId = selectedChecklist, CategoryId = selectedCategory };

            apiCallsServiceMock.Setup(x => x.GetItemsAsync<CategoryModel>("Categories"))
                               .ReturnsAsync(categorires)
                               .Verifiable();
            apiCallsServiceMock.Setup(x => x.GetItemsAsync<StatusModel>("Statuses"))
                               .ReturnsAsync(statuses)
                               .Verifiable();

            apiCallsServiceMock.Setup(x => x.GetItemAsync<StatusModel>("Statuses/GetByName/Planned"))
                               .ReturnsAsync(statusPlanned)
                               .Verifiable();

            // Act
            var viewModel = await createViewModelService.CreateViewModelCreateOrUpdateTodoItemAsync(todoItem);

            // Assert
            Assert.Equal(checklists, viewModel.ChecklistModels);
            Assert.Equal(statuses, viewModel.StatusModels);
            Assert.Equal(categorires, viewModel.CategoryModels);

            Assert.Equal(selectedCategory, viewModel.SelectedCategoryId);
            Assert.Equal(selectedChecklist, viewModel.SelectedChecklistId);
            Assert.Equal(statusPlanned.Id, viewModel.SelectedStatusId);

            apiCallsServiceMock.Verify();
        }

        private static List<ChecklistModel> GetChecklistsSample()
        {
            return new List<ChecklistModel>
            {
                new ChecklistModel { Id = 1, Name = "University", UserId = 1 },
                new ChecklistModel { Id = 2, Name = "Nirhtday", UserId = 1 },
                new ChecklistModel { Id = 3, Name = "Chores", UserId = 1 }
            };
        }


        private static List<CategoryModel> GetCategoriesSample()
        {
            return new List<CategoryModel>
            {
                new CategoryModel { Id = 1, Name = "Improtatnt" },
                new CategoryModel { Id = 2, Name = "Not improntatn" },
                new CategoryModel { Id = 3, Name = "Smth else" }
            };
        }

        private static List<StatusModel> GetStatusesSample()
        {
            return new List<StatusModel>
            {
                new StatusModel {Id = 1, Name = "Planned", IsDone = false },
                new StatusModel {Id = 2, Name = "Ongoing", IsDone = false },
                new StatusModel {Id = 3, Name = "Done", IsDone = true },
            };
        }
    }
}
