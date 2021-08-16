using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Blazored.Modal;
using Blazored.Modal.Services;

using GoogleMapsComponents.Maps;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

using ToDoList.BlazorClient.Services;
using ToDoList.BlazorClient.Shared;
using ToDoList.Extensions;
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

            return base.SetParametersAsync(parameters);
        }

        protected override async Task OnInitializedAsync() =>
            categories = await ApiInvoker.GetItemsAsync<CategoryModel>(ApiEndpoints.Categories);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            objRef = DotNetObjectReference.Create(this);

            if (firstRender)
                await Init("init");
        }

        [JSInvokable]
        public async Task UpdateCategoryAsync(string category, string categoryName)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                await SetCategory(null, null);
            }
            else if (int.TryParse(category?.ToString(), out int value))
            {
                await SetCategory(value, categoryName);
            }
            else
            {
                await ApiInvoker.PostItemAsync(ApiEndpoints.Categories, new CategoryModel { Name = category });
                var created = await ApiInvoker.GetItemAsync<CategoryModel>($"{ApiEndpoints.CategoryByName}/{category}");

                await SetCategory(created.Id, created.Name);
            }
        }

        [JSInvokable]
        public async Task ChangeDueDateAsync(string date)
        {
            if (DateTime.TryParse(date, out var result))
            {
                todoItemModel.DueDate = result;
                await Update();

                StateHasChanged();
            }
        }

        private async Task Update() => await ApiInvoker.PutItemAsync(ApiEndpoints.TodoItems, todoItemModel);

        private async Task Delete()
        {
            var result = await Modal.Show<Confirm>("Confirmation").Result;

            if (result.Cancelled)
                return;

            await ApiInvoker.DeleteItemAsync($"{ApiEndpoints.TodoItems}/{todoItemModel.Id}");
            await Notifier.OnChecklistChanged(todoItemModel.ChecklistId);
        }

        private async Task CheckTodoItem(ChangeEventArgs args)
        {
            StatusModel statusModel;

            bool done = (bool)args.Value;

            statusModel = done
                ? await ApiInvoker.GetItemAsync<StatusModel>(ApiEndpoints.StatusDone)
                : await ApiInvoker.GetItemAsync<StatusModel>(ApiEndpoints.StatusOngoing);

            await UpdateStatus(statusModel);
        }

        private async Task LoadStatuses() => statuses = await ApiInvoker.GetItemsAsync<StatusModel>(ApiEndpoints.Statuses);

        private async Task UpdateStatus(StatusModel status)
        {
            todoItemModel.StatusId = status.Id;
            todoItemModel.StatusName = status.Name;

            await Update();
            await Notifier.OnChecklistChanged(todoItemModel.ChecklistId);
        }

        private async Task ResetCategory() => await SetCategory(null, null);

        private async Task ChangeLocation()
        {
            ModalParameters parameters = new();
            parameters.Add(nameof(GeoCoordinate), todoItemModel.GeoPoint);

            var result = await Modal.Show<LocationSelector>("Choose location for the task", parameters).Result;

            if (result.Cancelled)
                return;

            var latLng = result.Data as LatLngLiteral;

            await SetLocation(latLng);
        }

        private async Task ResetLocation() => await SetLocation(null);

        private async Task ChangeImage(InputFileChangeEventArgs e)
        {
            using var stream = e.File.OpenReadStream(2000000);
            byte[] fileBytes = await stream.ToByteArrayAsync();

            string file = await ApiInvoker.PostFileAsync(ApiEndpoints.Images, e.File.Name, fileBytes);
            var image = await ApiInvoker.GetItemAsync<ImageModel>($"{ApiEndpoints.ImageByName}/{file}");

            await SetImage(image.Id, image.Content);
        }

        private async Task ResetImage() => await SetImage(null, null);

        private async Task ResetDueDate()
        {
            todoItemModel.DueDate = null;
            await Update();
        }

        private async Task SetCategory(int? categoryId, string categoryName)
        {
            todoItemModel.CategoryId = categoryId;
            todoItemModel.CategoryName = categoryName;

            await Update();
            StateHasChanged();

            await Init("initSelect");
        }

        private async Task SetImage(int? imageId, byte[] imageContent)
        {
            todoItemModel.ImageId = imageId;
            todoItemModel.ImageContent = imageContent;

            await Update();
        }

        private async Task SetLocation(LatLngLiteral latLng)
        {
            todoItemModel.GeoPoint = latLng is not null
                ? new GeoCoordinate(latLng.Lng, latLng.Lat)
                : null;
            await Update();

            todoItemModel = await ApiInvoker.GetItemAsync<TodoItemModel>($"{ApiEndpoints.TodoItems}/{todoItemModel.Id}");
        }

        private async Task Init(string method) => await JSRuntime.InvokeVoidAsync(method, objRef, todoItemModel.Id);

        public void Dispose() => objRef?.Dispose();
    }
}
