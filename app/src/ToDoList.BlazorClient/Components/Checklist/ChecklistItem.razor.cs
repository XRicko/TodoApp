using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using ToDoList.BlazorClient.Services;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.BlazorClient.Components.Checklist
{
    public partial class ChecklistItem
    {
        private IEnumerable<TodoItemModel> todoItemModels = Array.Empty<TodoItemModel>();

        private ICollection<TodoItemModel> ActiveTodoItems => todoItemModels.Where(x => x.StatusName != "Done").ToList();
        private ICollection<TodoItemModel> DoneTodoItems => todoItemModels.Where(x => x.StatusName == "Done").ToList();

        [Inject]
        private IApiInvoker ApiInvoker { get; set; }
        [Inject]
        private Notifier Notifier { get; set; }

        [Parameter]
        public EventCallback<int> OnDeleteCallback { get; set; }

        [Parameter]
        public ChecklistModel ChecklistModel { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            todoItemModels = await ApiInvoker.GetItemsAsync<TodoItemModel>("TodoItems/GetByChecklistId/" + ChecklistModel.Id);

            Notifier.ChecklistChanged += OnTodoItemsChanged;
        }

        private async Task Update() => await ApiInvoker.PutItemAsync("Checklists", ChecklistModel);

        private async Task OnTodoItemsChanged(int checklistId)
        {
            await InvokeAsync(async () =>
            {
                if (ChecklistModel.Id == checklistId)
                {
                    todoItemModels = await ApiInvoker.GetItemsAsync<TodoItemModel>("TodoItems/GetByChecklistId/" + checklistId);
                    StateHasChanged();
                }
            });
        }
    }
}
