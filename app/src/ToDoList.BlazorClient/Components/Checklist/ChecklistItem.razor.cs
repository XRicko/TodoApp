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

        private int dragCounter;
        private string dropClass;

        private bool collapsed;

        [Inject]
        private IApiInvoker ApiInvoker { get; set; }
        [Inject]
        private Notifier Notifier { get; set; }

        [CascadingParameter]
        private TodoItemContainer Container { get; set; }

        [Parameter]
        public EventCallback<int> OnDeleteCallback { get; set; }

        [Parameter]
        public ChecklistModel ChecklistModel { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            dropClass = "";

            await LoadTodoItems();
            Notifier.ChecklistChanged += OnTodoItemsChanged;
        }

        private async Task Update() => await ApiInvoker.PutItemAsync("Checklists", ChecklistModel);

        private async Task OnTodoItemsChanged(int checklistId)
        {
            await InvokeAsync(async () =>
            {
                if (ChecklistModel.Id == checklistId)
                {
                    await LoadTodoItems();
                    StateHasChanged();
                }
            });
        }

        private void HandleDragEnter()
        {
            dropClass = Container.DraggedTodoItem.ChecklistId == ChecklistModel.Id
                ? "no-drop"
                : "yes-drop";

            dragCounter++;
        }

        private void HandleDragLeave()
        {
            dragCounter--;

            if (dragCounter == 0)
                dropClass = "";
        }

        private async Task HandleDrop()
        {
            if (Container.DraggedTodoItem.ChecklistId != ChecklistModel.Id)
            {
                int oldChecklist = Container.DraggedTodoItem.ChecklistId;

                Container.DraggedTodoItem.ChecklistId = ChecklistModel.Id;

                await ApiInvoker.PutItemAsync("TodoItems", Container.DraggedTodoItem);
                await LoadTodoItems();

                await Notifier.UpdateChecklist(oldChecklist);
            }

            HandleDragLeave();
        }

        private async Task LoadTodoItems()
        {
            todoItemModels = await ApiInvoker.GetItemsAsync<TodoItemModel>($"TodoItems/GetByChecklistId/{ChecklistModel.Id}");
        }

        private void Collapse() => collapsed = !collapsed;
    }
}
