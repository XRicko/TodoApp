using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Blazored.Modal.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using ToDoList.BlazorClient.Services;
using ToDoList.BlazorClient.Shared;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.BlazorClient.Components.TodoItem
{
    public partial class TodoItem : IDisposable
    {
        private IEnumerable<CategoryModel> categories = Array.Empty<CategoryModel>();

        private bool? Checked => TodoItemModel.StatusName?.Equals("Done");

        [CascadingParameter]
        private IModalService Modal { get; set; }

        [Inject]
        private IApiInvoker ApiInvoker { get; set; }
        [Inject]
        private Notifier Notifier { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        private DotNetObjectReference<TodoItem> objRef;

        [Parameter]
        public TodoItemModel TodoItemModel { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            categories = await ApiInvoker.GetItemsAsync<CategoryModel>("Categories");

            objRef = DotNetObjectReference.Create(this);
            await InitSelect2();
        }

        [JSInvokable]
        public async Task UpdateCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                TodoItemModel.CategoryId = null;
            }
            else if (int.TryParse(category?.ToString(), out int value))
            {
                TodoItemModel.CategoryId = value;
            }
            else
            {
                await ApiInvoker.PostItemAsync("Categories", new CategoryModel { Name = category });
                var created = await ApiInvoker.GetItemAsync<CategoryModel>("Categories/GetByName/" + category);

                TodoItemModel.CategoryId = created.Id;
            }

            await Update();
            //await InitSelect2();

            StateHasChanged();
        }

        private async Task InitSelect2() => await JSRuntime.InvokeVoidAsync("initSelect", objRef, TodoItemModel.Id);

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

            bool done = (bool)args.Value;

            statusModel = done
                ? await ApiInvoker.GetItemAsync<StatusModel>("Statuses/GetByName/Done")
                : await ApiInvoker.GetItemAsync<StatusModel>("Statuses/GetByName/Ongoing");

            TodoItemModel.StatusId = statusModel.Id;
            TodoItemModel.StatusName = statusModel.Name;

            await ApiInvoker.PutItemAsync("TodoItems", TodoItemModel);
            await Notifier.UpdateChecklist(TodoItemModel.ChecklistId);
        }

        public void Dispose() => objRef?.Dispose();
    }
}
