using System.Threading.Tasks;

using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.ViewModels;

namespace ToDoList.MvcClient.Services
{
    public class CreateViewModelService : ICreateViewModelService
    {
        private readonly IApiCallsService apiCallsService;

        public CreateViewModelService(IApiCallsService apiService)
        {
            apiCallsService = apiService;
        }

        public async Task<IndexViewModel> CreateIndexViewModel()
        {
            var todoItems = await apiCallsService.GetItemsAsync<TodoItemModel>("TodoItems");
            var checklistModels = await apiCallsService.GetItemsAsync<ChecklistModel>("Checklists");

            IndexViewModel viewModel = new() { ChecklistModels = checklistModels, TodoItemModels = todoItems };

            return viewModel;
        }

        public async Task<CreateViewModel> CreateViewModelCreateOrUpdateTodoItem(TodoItemModel todoItemModel)
        {
            var checklistModels = await apiCallsService.GetItemsAsync<ChecklistModel>("Checklists");
            var categoryModels = await apiCallsService.GetItemsAsync<CategoryModel>("Categories");
            var statusModels = await apiCallsService.GetItemsAsync<StatusModel>("Statuses");

            int selectedChecklist = todoItemModel.ChecklistId;
            int? selectedCategory = todoItemModel.CategoryId;
            int? selectedStatus = default;

            if (todoItemModel.StatusId is null)
            {
                var plannedStatus = await apiCallsService.GetItemAsync<StatusModel>("Statuses/GetByName/Planned");
                selectedStatus = plannedStatus.Id;
            }

            CreateViewModel viewModel = new()
            {
                TodoItemModel = todoItemModel,

                ChecklistModels = checklistModels,
                CategoryModels = categoryModels,
                StatusModels = statusModels,

                SelectedChecklistId = selectedChecklist,
                SelectedCategoryId = selectedCategory,
                SelectedStatusId = selectedStatus
            };

            return viewModel;
        }
    }
}
