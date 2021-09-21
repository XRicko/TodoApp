using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using ToDoList.BlazorClient.Services;
using ToDoList.SharedClientLibrary;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.BlazorClient.Components.Checklist
{
    public partial class ChecklistItem
    {
        private ChecklistModel checklistModel;

        private List<TodoItemModel> todoItemModels = new();

        private ICollection<TodoItemModel> ActiveTodoItems => todoItemModels.Where(x => x.StatusName != "Done").ToList();
        private ICollection<TodoItemModel> DoneTodoItems => todoItemModels.Where(x => x.StatusName == "Done").ToList();

        private readonly List<Func<Task>> toRunAfterRender = new();

        private int dragCounter;

        private string cardDropClass;
        private string overlayClass;

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

        public override Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<ChecklistModel>(nameof(ChecklistModel), out var value))
                checklistModel = value;

            return base.SetParametersAsync(parameters);
        }

        protected override async Task OnInitializedAsync()
        {
            overlayClass = "";

            await LoadTodoItems();
            Notifier.ChecklistChanged += OnTodoItemsChanged;
            Notifier.FilterChosen += OnFilterChosen;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            foreach (var action in toRunAfterRender)
            {
                await action();
            }

            toRunAfterRender.Clear();
        }

        private async Task Submit()
        {
            if (checklistModel.Id == 0)
            {
                await ApiInvoker.PostItemAsync(ApiEndpoints.Checklists, checklistModel);
                checklistModel = await ApiInvoker.GetItemAsync<ChecklistModel>($"{ApiEndpoints.ChecklistByNameAndUserId}/{checklistModel.Name}");
            }
            if (checklistModel.Id > 0)
                await ApiInvoker.PutItemAsync(ApiEndpoints.Checklists, checklistModel);
        }

        private void AddTodoItem()
        {
            var todoItem = new TodoItemModel
            {
                StartDate = DateTime.Now,
                ChecklistId = checklistModel.Id,
                StatusId = 1,
                StatusName = "Planned"
            };
            todoItemModels.Add(todoItem);

            toRunAfterRender.Add(() => Notifier.OnItemAdded(todoItem));
        }

        private void HandleDragEnter()
        {
            if (Container.DraggedTodoItem is null)
                return;

            cardDropClass = "drop-area";
            overlayClass = Container.DraggedTodoItem.ChecklistId == checklistModel.Id
                ? "no-drop"
                : "yes-drop";

            dragCounter++;
        }

        private void HandleDragLeave()
        {
            if (Container.DraggedTodoItem is null)
                return;

            dragCounter--;

            if (dragCounter == 0)
                ResetDragAndDropClasses();
        }

        private async Task HandleDrop(DragEventArgs args)
        {
            if (Container.DraggedTodoItem?.ChecklistId != checklistModel.Id && Container.DraggedTodoItem is not null)
            {
                int oldChecklist = Container.DraggedTodoItem.ChecklistId;

                Container.DraggedTodoItem.ChecklistId = checklistModel.Id;

                await ApiInvoker.PutItemAsync(ApiEndpoints.TodoItems, Container.DraggedTodoItem);
                await LoadTodoItems();

                await Notifier.OnChecklistChanged(oldChecklist);
            }

            ResetDragAndDropClasses();
            dragCounter--;
        }

        private void ResetDragAndDropClasses()
        {
            cardDropClass = "";
            overlayClass = "";
        }

        private async Task LoadTodoItems() =>
            todoItemModels = (await ApiInvoker.GetItemsAsync<TodoItemModel>($"{ApiEndpoints.TodoItemsByChecklistId}/{checklistModel.Id}")).ToList();

        private void Collapse() => collapsed = !collapsed;

        private async Task OnTodoItemsChanged(int checklistId)
        {
            await InvokeAsync(async () =>
            {
                if (checklistModel.Id == checklistId)
                {
                    await LoadTodoItems();
                    StateHasChanged();
                }
            });
        }

        private async Task OnFilterChosen(string statusName, string categoryName)
        {
            await LoadTodoItems();
            IEnumerable<TodoItemModel> filtered = todoItemModels;

            if (!string.IsNullOrWhiteSpace(statusName))
                filtered = filtered.Where(x => x.StatusName == statusName);
            if (!string.IsNullOrWhiteSpace(categoryName))
                filtered = filtered.Where(x => x.CategoryName == categoryName);

            todoItemModels = filtered.ToList();

            StateHasChanged();
        }
    }
}
