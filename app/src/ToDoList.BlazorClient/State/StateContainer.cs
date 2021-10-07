using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ToDoList.SharedClientLibrary;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.BlazorClient.State
{
    public class StateContainer
    {
        private readonly IApiInvoker apiInvoker;
        public StateContainer(IApiInvoker invoker)
        {
            apiInvoker = invoker ?? throw new System.ArgumentNullException(nameof(invoker));
        }

        public string StatusName { get; set; }
        public string CategoryName { get; set; }

        public string SearchTerm { get; set; }

        public TodoItemModel DraggedTodoItem { get; set; }

        public List<ProjectModel> Projects { get; set; } = new();

        public List<StatusModel> Statuses { get; set; } = new();
        public List<CategoryModel> Categories { get; set; } = new();

        public async Task InitAsync()
        {
            Projects = (await apiInvoker.GetItemsAsync<ProjectModel>(ApiEndpoints.Projects)).ToList();

            Statuses = (await apiInvoker.GetItemsAsync<StatusModel>(ApiEndpoints.Statuses)).ToList();
            Categories = (await apiInvoker.GetItemsAsync<CategoryModel>(ApiEndpoints.Categories)).ToList();
        }
    }
}
