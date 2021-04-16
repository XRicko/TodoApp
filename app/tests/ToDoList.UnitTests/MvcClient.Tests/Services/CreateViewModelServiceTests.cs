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
        private readonly Mock<IApiInvoker> apiInvokerMock;
        private readonly CreateViewModelService createViewModelService;

        private readonly List<ChecklistModel> checklists;

        public CreateViewModelServiceTests()
        {
            apiInvokerMock = new Mock<IApiInvoker>();
            createViewModelService = new CreateViewModelService(apiInvokerMock.Object);

            checklists = GetChecklistsSample();

            apiInvokerMock.Setup(x => x.GetItemsAsync<ChecklistModel>("Checklists"))
                               .ReturnsAsync(checklists)
                               .Verifiable();
        }

        [Fact]
        public async Task CreateIndexViewModel_ReturnsIndexViewModelWithAll()
        {
            // Arrange
            var todoItems = GetActiveTodoItems();

            apiInvokerMock.Setup(x => x.GetItemsAsync<TodoItemModel>("TodoItems"))
                   .ReturnsAsync(todoItems)
                   .Verifiable();

            // Act
            var indexViewModel = await createViewModelService.CreateIndexViewModelAsync();

            // Assert
            Assert.Equal(todoItems, indexViewModel.TodoItems);
            Assert.Equal(checklists, indexViewModel.ChecklistModels);

            apiInvokerMock.Verify();
        }

        [Fact]
        public async Task CreateIndexViewModel_ReturnsIndexViewModelByCategoryGivenCategoryName()
        {
            // Arrange
            string categoryName = "Important";

            var todoItems = GetActiveTodoItems();

            apiInvokerMock.Setup(x => x.GetItemsAsync<TodoItemModel>("TodoItems"))
                   .ReturnsAsync(todoItems)
                   .Verifiable();

            // Act
            var indexViewModel = await createViewModelService.CreateIndexViewModelAsync(categoryName);

            // Assert
            Assert.All(indexViewModel.TodoItems, x => Assert.Equal(x.CategoryName, categoryName));

            Assert.Equal(checklists, indexViewModel.ChecklistModels);
            Assert.Equal(categoryName, indexViewModel.SelectedCategory);

            apiInvokerMock.Verify();
        }

        [Fact]
        public async Task CreateIndexViewModel_ReturnsIndexViewModelByStatusGivenStatusName()
        {
            // Arrange
            string statusName = "Planned";

            var todoItems = GetActiveTodoItems();

            apiInvokerMock.Setup(x => x.GetItemsAsync<TodoItemModel>("TodoItems"))
                   .ReturnsAsync(todoItems)
                   .Verifiable();

            // Act
            var indexViewModel = await createViewModelService.CreateIndexViewModelAsync(statusName: statusName);

            // Assert
            Assert.All(indexViewModel.TodoItems, x => Assert.Equal(x.StatusName, statusName));

            Assert.Equal(checklists, indexViewModel.ChecklistModels);
            Assert.Equal(statusName, indexViewModel.SelectedStatus);

            apiInvokerMock.Verify();
        }

        [Fact]
        public async Task CreateIndexViewModel_ReturnsIndexViewModelByCategoryAndStatusGivenBoth()
        {
            // Arrange
            string categoryName = "Important";
            string statusName = "Planned";

            var todoItems = GetActiveTodoItems();

            apiInvokerMock.Setup(x => x.GetItemsAsync<TodoItemModel>("TodoItems"))
                   .ReturnsAsync(todoItems)
                   .Verifiable();

            // Act
            var indexViewModel = await createViewModelService.CreateIndexViewModelAsync(categoryName, statusName);

            // Assert
            Assert.All(indexViewModel.TodoItems, x => Assert.Equal(x.CategoryName, categoryName));
            Assert.All(indexViewModel.TodoItems, x => Assert.Equal(x.StatusName, statusName));

            Assert.Equal(checklists, indexViewModel.ChecklistModels);
            Assert.Equal(categoryName, indexViewModel.SelectedCategory);
            Assert.Equal(statusName, indexViewModel.SelectedStatus);

            apiInvokerMock.Verify();
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

            apiInvokerMock.Setup(x => x.GetItemsAsync<CategoryModel>("Categories"))
                               .ReturnsAsync(categorires)
                               .Verifiable();
            apiInvokerMock.Setup(x => x.GetItemsAsync<StatusModel>("Statuses"))
                               .ReturnsAsync(statuses)
                               .Verifiable();

            apiInvokerMock.Setup(x => x.GetItemAsync<StatusModel>("Statuses/GetByName/Planned"))
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

            apiInvokerMock.Verify();
        }

        private static List<TodoItemModel> GetActiveTodoItems()
        {
            return new List<TodoItemModel>
            {
                new TodoItemModel {Id = 5, Name = "Prepare for exam", ChecklistId = 1, StatusId = 1, StatusName = "Planned", CategoryName = "Important" },
                new TodoItemModel {Id = 10, Name = "Prepare a gift", ChecklistId = 2, StatusId = 2, StatusName = "Ongoing" },
                new TodoItemModel {Id = 15, Name = "Clean up the room", ChecklistId = 3, StatusId = 1, StatusName = "Planned", CategoryName = "Important" }
            };
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
