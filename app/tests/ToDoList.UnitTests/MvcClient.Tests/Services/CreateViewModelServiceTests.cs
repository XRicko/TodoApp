using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.Services;
using ToDoList.SharedClientLibrary;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

using Xunit;

namespace MvcClient.Tests.Services
{
    public class CreateViewModelServiceTests
    {
        private readonly Mock<IApiInvoker> apiInvokerMock;
        private readonly ViewModelService createViewModelService;

        private readonly List<ChecklistModel> checklists;

        public CreateViewModelServiceTests()
        {
            apiInvokerMock = new Mock<IApiInvoker>();
            createViewModelService = new ViewModelService(apiInvokerMock.Object);

            checklists = GetChecklistsSample();

            apiInvokerMock.Setup(x => x.GetItemsAsync<ChecklistModel>(ApiEndpoints.Checklists))
                          .ReturnsAsync(checklists)
                          .Verifiable();
        }

        [Fact]
        public async Task CreateIndexViewModel_ReturnsIndexViewModelWithAll()
        {
            // Arrange
            var todoItems = GetActiveTodoItems();

            apiInvokerMock.Setup(x => x.GetItemsAsync<TodoItemModelWithFile>(ApiEndpoints.TodoItems))
                          .ReturnsAsync(todoItems)
                          .Verifiable();

            // Act
            var indexViewModel = await createViewModelService.CreateIndexViewModelAsync();

            // Assert
            indexViewModel.TodoItems.Should().Equal(todoItems);
            indexViewModel.ChecklistModels.Should().Equal(checklists);

            apiInvokerMock.Verify();
        }

        [Fact]
        public async Task CreateIndexViewModel_ReturnsIndexViewModelByCategoryGivenCategoryName()
        {
            // Arrange
            string categoryName = "Important";

            var todoItems = GetActiveTodoItems();

            apiInvokerMock.Setup(x => x.GetItemsAsync<TodoItemModelWithFile>(ApiEndpoints.TodoItems))
                          .ReturnsAsync(todoItems)
                          .Verifiable();

            // Act
            var indexViewModel = await createViewModelService.CreateIndexViewModelAsync(categoryName);

            // Assert
            indexViewModel.TodoItems.Should().OnlyContain(x => x.CategoryName == categoryName);

            indexViewModel.ChecklistModels.Should().Equal(checklists);
            indexViewModel.SelectedCategory.Should().Be(categoryName);

            apiInvokerMock.Verify();
        }

        [Fact]
        public async Task CreateIndexViewModel_ReturnsIndexViewModelByStatusGivenStatusName()
        {
            // Arrange
            string statusName = "Planned";

            var todoItems = GetActiveTodoItems();

            apiInvokerMock.Setup(x => x.GetItemsAsync<TodoItemModelWithFile>(ApiEndpoints.TodoItems))
                          .ReturnsAsync(todoItems)
                          .Verifiable();

            // Act
            var indexViewModel = await createViewModelService.CreateIndexViewModelAsync(statusName: statusName);

            // Assert
            indexViewModel.TodoItems.Should().OnlyContain(x => x.StatusName == statusName);

            indexViewModel.ChecklistModels.Should().Equal(checklists);
            indexViewModel.SelectedStatus.Should().Be(statusName);

            apiInvokerMock.Verify();
        }

        [Fact]
        public async Task CreateIndexViewModel_ReturnsIndexViewModelByCategoryAndStatusGivenBoth()
        {
            // Arrange
            string categoryName = "Important";
            string statusName = "Planned";

            var todoItems = GetActiveTodoItems();

            apiInvokerMock.Setup(x => x.GetItemsAsync<TodoItemModelWithFile>(ApiEndpoints.TodoItems))
                          .ReturnsAsync(todoItems)
                          .Verifiable();

            // Act
            var indexViewModel = await createViewModelService.CreateIndexViewModelAsync(categoryName, statusName);

            // Assert
            indexViewModel.TodoItems.Should().Contain(x => x.CategoryName == categoryName);
            indexViewModel.TodoItems.Should().Contain(x => x.StatusName == statusName);

            indexViewModel.ChecklistModels.Should().Equal(checklists);
            indexViewModel.SelectedCategory.Should().Be(categoryName);
            indexViewModel.SelectedStatus.Should().Be(statusName);

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

            var todoItem = new TodoItemModelWithFile { Name = "Do something", ChecklistId = selectedChecklist, CategoryId = selectedCategory };

            apiInvokerMock.Setup(x => x.GetItemsAsync<CategoryModel>(ApiEndpoints.Categories))
                          .ReturnsAsync(categorires)
                          .Verifiable();
            apiInvokerMock.Setup(x => x.GetItemsAsync<StatusModel>(ApiEndpoints.Statuses))
                          .ReturnsAsync(statuses)
                          .Verifiable();

            // Act
            var viewModel = await createViewModelService.CreateViewModelCreateOrUpdateTodoItemAsync(todoItem);

            // Assert
            viewModel.ChecklistModels.Should().Equal(checklists);
            viewModel.StatusModels.Should().Equal(statuses);
            viewModel.StatusModels.Should().Equal(statuses);

            viewModel.SelectedCategoryId.Should().Be(selectedCategory);
            viewModel.SelectedChecklistId.Should().Be(selectedChecklist);
            viewModel.SelectedStatusId.Should().Be(statusPlanned.Id);

            apiInvokerMock.Verify();
        }

        private static List<TodoItemModelWithFile> GetActiveTodoItems()
        {
            return new List<TodoItemModelWithFile>
            {
                new TodoItemModelWithFile {Id = 5, Name = "Prepare for exam", ChecklistId = 1, StatusId = 1, StatusName = "Planned", CategoryName = "Important" },
                new TodoItemModelWithFile {Id = 10, Name = "Prepare a gift", ChecklistId = 2, StatusId = 2, StatusName = "Ongoing" },
                new TodoItemModelWithFile {Id = 15, Name = "Clean up the room", ChecklistId = 3, StatusId = 1, StatusName = "Planned", CategoryName = "Important" }
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
