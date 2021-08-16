﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using ToDoList.BlazorClient.Services;
using ToDoList.SharedClientLibrary;
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

        private async Task Submit()
        {
            if (ChecklistModel.Id == 0)
                await ApiInvoker.PostItemAsync(ApiEndpoints.Checklists, ChecklistModel);
            if (ChecklistModel.Id > 0)
                await ApiInvoker.PutItemAsync(ApiEndpoints.Checklists, ChecklistModel);
        }

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

                await ApiInvoker.PutItemAsync(ApiEndpoints.TodoItems, Container.DraggedTodoItem);
                await LoadTodoItems();

                await Notifier.OnChecklistChanged(oldChecklist);
            }

            HandleDragLeave();
        }

        private async Task LoadTodoItems()
        {
            todoItemModels = await ApiInvoker.GetItemsAsync<TodoItemModel>($"{ApiEndpoints.TodoItemByChecklistId}/{ChecklistModel.Id}");
        }

        private void Collapse() => collapsed = !collapsed;
    }
}
