using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Blazored.Modal;
using Blazored.Modal.Services;

using GoogleMapsComponents.Maps;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using ToDoList.BlazorClient.Services;
using ToDoList.BlazorClient.Shared;
using ToDoList.SharedClientLibrary;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;
using ToDoList.SharedKernel;

namespace ToDoList.BlazorClient.Components.TodoItem
{
    public partial class TodoItem : IDisposable
    {
        private IEnumerable<CategoryModel> categories = Array.Empty<CategoryModel>();
        private IEnumerable<StatusModel> statuses = Array.Empty<StatusModel>();

        private bool? Checked => todoItemModel.StatusName?.Equals("Done");

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
        private TodoItemModel todoItemModel;

        public override Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<TodoItemModel>(nameof(TodoItemModel), out var value))
                todoItemModel = value;

            Console.WriteLine("Set");
            return base.SetParametersAsync(parameters);
        }

        protected override async Task OnInitializedAsync() => 
            categories = await ApiInvoker.GetItemsAsync<CategoryModel>(ApiEndpoints.Categories);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            objRef = DotNetObjectReference.Create(this);

            if (firstRender)
                await InitSelect2();
        }

        [JSInvokable]
        public async Task UpdateCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                todoItemModel.CategoryId = null;
            }
            else if (int.TryParse(category?.ToString(), out int value))
            {
                todoItemModel.CategoryId = value;
            }
            else
            {
                await ApiInvoker.PostItemAsync(ApiEndpoints.Categories, new CategoryModel { Name = category });
                var created = await ApiInvoker.GetItemAsync<CategoryModel>($"{ApiEndpoints.CategoryByName}/{category}");

                todoItemModel.CategoryId = created.Id;
            }

            await Update();
            StateHasChanged();

            await InitSelect2();
        }

        private async Task Delete()
        {
            var result = await Modal.Show<Confirm>("Confirmation").Result;

            if (result.Cancelled)
                return;

            await ApiInvoker.DeleteItemAsync($"{ApiEndpoints.TodoItems}/{todoItemModel.Id}");
            await Notifier.UpdateChecklist(todoItemModel.ChecklistId);
        }

        private async Task Update() => await ApiInvoker.PutItemAsync(ApiEndpoints.TodoItems, todoItemModel);

        private async Task CheckTodoItem(ChangeEventArgs args)
        {
            StatusModel statusModel;

            bool done = (bool)args.Value;

            statusModel = done
                ? await ApiInvoker.GetItemAsync<StatusModel>(ApiEndpoints.StatusDone)
                : await ApiInvoker.GetItemAsync<StatusModel>(ApiEndpoints.StatusOngoing);

            todoItemModel.StatusId = statusModel.Id;
            todoItemModel.StatusName = statusModel.Name;

            await Update();
            await Notifier.UpdateChecklist(todoItemModel.ChecklistId);
        }

        private async Task LoadStatuses() => statuses = await ApiInvoker.GetItemsAsync<StatusModel>(ApiEndpoints.Statuses);

        private async Task UpdateStatus(int statusId)
        {
            todoItemModel.StatusId = statusId;

            await Update();
            await Notifier.UpdateChecklist(todoItemModel.ChecklistId);
        }

        private async Task ResetCategory()
        {
            if (todoItemModel.CategoryId is null)
                return;

            todoItemModel.CategoryId = null;
            await Update();

            StateHasChanged();
            await InitSelect2();
        }

        private async Task ChangeLocation()
        {
            ModalParameters parameters = new();
            parameters.Add(nameof(GeoCoordinate), todoItemModel.GeoPoint);

            var result = await Modal.Show<LocationSelector>("Choose location for the task", parameters).Result;

            if (result.Cancelled)
                return;

            var latLng = result.Data as LatLngLiteral;

            todoItemModel.GeoPoint = new GeoCoordinate(latLng.Lng, latLng.Lat);
            await Update();

            todoItemModel = await ApiInvoker.GetItemAsync<TodoItemModel>($"{ApiEndpoints.TodoItems}/{todoItemModel.Id}");
        }

        private async Task ResetLocation()
        {
            todoItemModel.GeoPoint = null;
            await Update();

            todoItemModel = await ApiInvoker.GetItemAsync<TodoItemModel>($"{ApiEndpoints.TodoItems}/{todoItemModel.Id}");
        }

        private async Task InitSelect2() => await JSRuntime.InvokeVoidAsync("initSelect", objRef, todoItemModel.Id);

        public void Dispose() => objRef?.Dispose();
    }
}
