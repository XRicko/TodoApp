﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using ToDoList.BlazorClient.Services;
using ToDoList.BlazorClient.State;
using ToDoList.SharedClientLibrary;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.BlazorClient.Components.Checklist
{
    public partial class ChecklistItem : IDisposable
    {
        private ChecklistModel checklistModel;

        private List<TodoItemModel> todoItemModels = new();

        private ICollection<TodoItemModel> ActiveTodoItems => todoItemModels.Where(x => x.StatusName != "Done").ToList();
        private ICollection<TodoItemModel> DoneTodoItems => todoItemModels.Where(x => x.StatusName == "Done").ToList();

        private readonly List<Func<Task>> toRunAfterRender = new();

        private int dragCounter;
        private string cardDropClass;

        private bool collapsed;

        [Inject]
        private IApiInvoker ApiInvoker { get; set; }

        [Inject]
        private Notifier Notifier { get; set; }

        [Inject]
        private StateContainer State { get; set; }

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

        protected override void OnInitialized()
        {
            Notifier.ChecklistChanged += OnTodoItemsChanged;
            Notifier.FilterChosen += OnFilterChosen;
        }

        protected override async Task OnParametersSetAsync() => await LoadTodoItems();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            foreach (var action in toRunAfterRender)
            {
                await action();
            }

            toRunAfterRender.Clear();
        }

        private async Task SubmitValid()
        {
            if (checklistModel.Id == 0)
            {
                await ApiInvoker.PostItemAsync(ApiEndpoints.Checklists, checklistModel);
                checklistModel = await ApiInvoker.GetItemAsync<ChecklistModel>(
                    $"{ApiEndpoints.ChecklistByProjectIdAndName}?name={checklistModel.Name}&projectId={checklistModel.ProjectId}");
            }
            if (checklistModel.Id > 0)
                await ApiInvoker.PutItemAsync(ApiEndpoints.Checklists, checklistModel);
        }

        private async Task Delete() => await OnDeleteCallback.InvokeAsync(checklistModel.Id);

        private void AddTodoItem()
        {
            var todoItem = todoItemModels.Find(x => x.Id == 0);

            if (todoItem is null)
            {
                todoItem = new TodoItemModel
                {
                    StartDate = DateTime.Now,
                    ChecklistId = checklistModel.Id,
                    StatusId = 1,
                    StatusName = "Planned"
                };
                todoItemModels.Add(todoItem);
            }

            toRunAfterRender.Add(() => Notifier.OnItemAdded(todoItem));
        }

        private void HandleDragEnter()
        {
            if (State.DraggedTodoItem is null)
                return;

            cardDropClass = State.DraggedTodoItem.ChecklistId == checklistModel.Id
                ? "no-drop"
                : "yes-drop";

            dragCounter++;
        }

        private void HandleDragLeave()
        {
            if (State.DraggedTodoItem is null)
                return;

            dragCounter--;

            if (dragCounter == 0)
                ResetDragAndDropClass();
        }

        private async Task HandleDrop(DragEventArgs args)
        {
            if (State.DraggedTodoItem?.ChecklistId != checklistModel.Id && State.DraggedTodoItem is not null)
            {
                int oldChecklist = State.DraggedTodoItem.ChecklistId;

                State.DraggedTodoItem.ChecklistId = checklistModel.Id;

                await ApiInvoker.PutItemAsync(ApiEndpoints.TodoItems, State.DraggedTodoItem);
                await LoadTodoItems();

                await Notifier.OnChecklistChanged(oldChecklist);
            }

            ResetDragAndDropClass();
            dragCounter--;
        }

        private void ResetDragAndDropClass() => cardDropClass = "";

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

        private async Task OnFilterChosen()
        {
            await LoadTodoItems();

            if (!string.IsNullOrWhiteSpace(State.StatusName))
                todoItemModels = todoItemModels.FindAll(x => x.StatusName == State.StatusName);
            if (!string.IsNullOrWhiteSpace(State.CategoryName))
                todoItemModels = todoItemModels.FindAll(x => x.CategoryName == State.CategoryName);
            if (!string.IsNullOrWhiteSpace(State.SearchTerm))
                todoItemModels = todoItemModels.FindAll(x => x.Name.Contains(State.SearchTerm, StringComparison.OrdinalIgnoreCase));

            StateHasChanged();
        }

        public void Dispose()
        {
            Notifier.ChecklistChanged -= OnTodoItemsChanged;
            Notifier.FilterChosen -= OnFilterChosen;
        }
    }
}
