using System.Threading.Tasks;

using Blazored.Modal.Services;

using Microsoft.AspNetCore.Components;

using ToDoList.BlazorClient.Services;
using ToDoList.BlazorClient.Shared;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.BlazorClient.Components.TodoItem
{
    public partial class TodoItem
    {
        [CascadingParameter]
        private IModalService Modal { get; set; }

        private bool? IsChecked => TodoItemModel.StatusName?.Equals("Done");

        [Inject]
        private IApiInvoker ApiInvoker { get; set; }
        [Inject]
        private Notifier Notifier { get; set; }

        [Parameter]
        public TodoItemModel TodoItemModel { get; set; } = new();

        private async Task Delete()
        {
            var result = await Modal.Show<Confirm>("Confirmation").Result;

            if (result.Cancelled)
                return;

            await ApiInvoker.DeleteItemAsync("TodoItems/", TodoItemModel.Id);
            await Notifier.UpdateChecklist(TodoItemModel.ChecklistId);
        }

        private async Task Update() => await ApiInvoker.PutItemAsync("TodoItems", TodoItemModel);

        private async Task ChangeStatus(ChangeEventArgs args)
        {
            StatusModel statusModel;

            bool isDone = (bool)args.Value;

            statusModel = isDone
                ? await ApiInvoker.GetItemAsync<StatusModel>("Statuses/GetByName/Done")
                : await ApiInvoker.GetItemAsync<StatusModel>("Statuses/GetByName/Ongoing");

            TodoItemModel.StatusId = statusModel.Id;
            TodoItemModel.StatusName = statusModel.Name;

            await ApiInvoker.PutItemAsync("TodoItems", TodoItemModel);
            await Notifier.UpdateChecklist(TodoItemModel.ChecklistId);
        }
    }
}
