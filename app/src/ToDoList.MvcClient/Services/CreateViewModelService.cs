using System;
using System.Linq;
using System.Threading.Tasks;

using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.Services.Api;
using ToDoList.MvcClient.ViewModels;

namespace ToDoList.MvcClient.Services
{
    public class CreateViewModelService : ICreateViewModelService
    {
        private readonly IApiInvoker apiInvoker;

        public CreateViewModelService(IApiInvoker invoker)
        {
            apiInvoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
        }

        public async Task<IndexViewModel> CreateIndexViewModelAsync(string categoryName = null, string statusName = null)
        {
            var todoItems = await apiInvoker.GetItemsAsync<TodoItemModel>("TodoItems");

            string selectedCategory = null;
            string selectedStatus = null;

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                todoItems = todoItems.Where(x => x.CategoryName?.ToLower() == categoryName.ToLower());
                selectedCategory = categoryName;
            }
            if (!string.IsNullOrWhiteSpace(statusName))
            {
                todoItems = todoItems.Where(x => x.StatusName?.ToLower() == statusName.ToLower());
                selectedStatus = statusName;
            }

            var checklistModels = await apiInvoker.GetItemsAsync<ChecklistModel>("Checklists");

            IndexViewModel viewModel = new()
            {
                TodoItems = todoItems,
                ChecklistModels = checklistModels,

                SelectedCategory = selectedCategory,
                SelectedStatus = selectedStatus
            };

            return viewModel;
        }

        public async Task<CreateTodoItemViewModel> CreateViewModelCreateOrUpdateTodoItemAsync(TodoItemModel todoItemModel)
        {
            _ = todoItemModel ?? throw new ArgumentNullException(nameof(todoItemModel));

            var checklistModels = await apiInvoker.GetItemsAsync<ChecklistModel>("Checklists");
            var categoryModels = await apiInvoker.GetItemsAsync<CategoryModel>("Categories");
            var statusModels = await apiInvoker.GetItemsAsync<StatusModel>("Statuses");

            int selectedChecklist = todoItemModel.ChecklistId;
            int? selectedCategory = todoItemModel.CategoryId;
            int selectedStatus = todoItemModel.StatusId;

            if (todoItemModel.StatusId is 0)
            {
                var plannedStatus = await apiInvoker.GetItemAsync<StatusModel>("Statuses/GetByName/Planned");
                selectedStatus = plannedStatus.Id;
            }

            CreateTodoItemViewModel viewModel = new()
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
