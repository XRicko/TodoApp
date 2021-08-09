using System;
using System.Linq;
using System.Threading.Tasks;

using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.ViewModels;
using ToDoList.SharedClientLibrary;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.MvcClient.Services
{
    public class ViewModelService : IViewModelService
    {
        private readonly IApiInvoker apiInvoker;

        public ViewModelService(IApiInvoker invoker)
        {
            apiInvoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
        }

        public async Task<IndexViewModel> CreateIndexViewModelAsync(string categoryName = null, string statusName = null)
        {
            var todoItems = await apiInvoker.GetItemsAsync<TodoItemModelWithFile>(ApiEndpoints.TodoItems);
            var checklists = await apiInvoker.GetItemsAsync<ChecklistModel>(ApiEndpoints.Checklists);

            if (!string.IsNullOrWhiteSpace(categoryName))
                todoItems = todoItems.Where(x => string.Equals(x.CategoryName, categoryName, StringComparison.CurrentCultureIgnoreCase));
            if (!string.IsNullOrWhiteSpace(statusName))
                todoItems = todoItems.Where(x => string.Equals(x.StatusName, statusName, StringComparison.CurrentCultureIgnoreCase));

            IndexViewModel viewModel = new()
            {
                TodoItems = todoItems,
                ChecklistModels = checklists,

                SelectedCategory = categoryName,
                SelectedStatus = statusName
            };

            return viewModel;
        }

        public async Task<CreateTodoItemViewModel> CreateViewModelCreateOrUpdateTodoItemAsync(TodoItemModelWithFile todoItemModel)
        {
            _ = todoItemModel ?? throw new ArgumentNullException(nameof(todoItemModel));

            var checklists = await apiInvoker.GetItemsAsync<ChecklistModel>(ApiEndpoints.Checklists);
            var categories = await apiInvoker.GetItemsAsync<CategoryModel>(ApiEndpoints.Categories);
            var statuses = await apiInvoker.GetItemsAsync<StatusModel>(ApiEndpoints.Statuses);

            int selectedChecklist = todoItemModel.ChecklistId;
            int? selectedCategory = todoItemModel.CategoryId;
            int selectedStatus = todoItemModel.StatusId;

            if (todoItemModel.StatusId is 0)
            {
                var plannedStatus = statuses.SingleOrDefault(x => x.Name == "Planned");
                selectedStatus = plannedStatus.Id;
            }

            CreateTodoItemViewModel viewModel = new()
            {
                TodoItemModel = todoItemModel,

                ChecklistModels = checklists,
                CategoryModels = categories,
                StatusModels = statuses,

                SelectedChecklistId = selectedChecklist,
                SelectedCategoryId = selectedCategory,
                SelectedStatusId = selectedStatus
            };

            return viewModel;
        }
    }
}
