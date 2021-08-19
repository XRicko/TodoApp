using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ToDoList.Extensions;
using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.Services;
using ToDoList.MvcClient.ViewModels;
using ToDoList.SharedClientLibrary;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.MvcClient.Controllers
{
    public class TodoController : Controller
    {
        private readonly IViewModelService viewModelService;
        private readonly IApiInvoker apiInvoker;

        public TodoController(IViewModelService modelService, IApiInvoker invoker) : base()
        {
            viewModelService = modelService ?? throw new ArgumentNullException(nameof(modelService));
            apiInvoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
        }

        public async Task<ActionResult> IndexAsync(string categoryName = null, string statusName = null)
        {
            IndexViewModel viewModel = await viewModelService.CreateIndexViewModelAsync(categoryName, statusName);
            return View("Index", viewModel);
        }

        public async Task<ActionResult> CreateOrUpdateAsync(int checklistId, int todoItemId = 0)
        {
            string viewName = "CreateOrUpdate";

            if (todoItemId == 0)
            {
                TodoItemModelWithFile todoItem = new() { ChecklistId = checklistId };
                return View(viewName, await viewModelService.CreateViewModelCreateOrUpdateTodoItemAsync(todoItem));
            }

            var todoItemModel = await apiInvoker.GetItemAsync<TodoItemModelWithFile>($"{ApiEndpoints.TodoItems}/{todoItemId}");

            if (todoItemModel is null)
                return NotFound();

            if (todoItemModel.GeoPoint is not null)
            {
                todoItemModel.Latitude = todoItemModel.GeoPoint.Latitude.ToString();
                todoItemModel.Longitude = todoItemModel.GeoPoint.Longitude.ToString();
            }

            return View(viewName, await viewModelService.CreateViewModelCreateOrUpdateTodoItemAsync(todoItemModel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdateAsync(CreateTodoItemViewModel createViewModel)
        {
            _ = createViewModel ?? throw new ArgumentNullException(nameof(createViewModel));

            if (!ModelState.IsValid)
                return PartialView("CreateOrUpdate", createViewModel.TodoItemModel);

            if (LatLongExists())
                AddGeoPoint(createViewModel.TodoItemModel);

            if (CategoryExists())
                await AddCategory(createViewModel.TodoItemModel);

            await AttachImage(createViewModel.TodoItemModel);

            if (IsNewItem())
                await apiInvoker.PostItemAsync(ApiEndpoints.TodoItems, createViewModel.TodoItemModel);

            if (!IsNewItem())
                await apiInvoker.PutItemAsync(ApiEndpoints.TodoItems, createViewModel.TodoItemModel);

            return PartialView("_ViewAll", await viewModelService.CreateIndexViewModelAsync());

            bool LatLongExists() => !string.IsNullOrWhiteSpace(createViewModel.TodoItemModel.Latitude)
                                    && !string.IsNullOrWhiteSpace(createViewModel.TodoItemModel.Latitude);
            bool CategoryExists() => !string.IsNullOrWhiteSpace(createViewModel.TodoItemModel.CategoryName);

            bool IsNewItem() => createViewModel.TodoItemModel.Id is 0;
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await apiInvoker.DeleteItemAsync($"{ApiEndpoints.TodoItems}/{id}");
            var viewModel = await viewModelService.CreateIndexViewModelAsync();

            return PartialView("_ViewAll", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> MarkTodoItemAsync(int id, bool isDone)
        {
            var todoItemModel = await apiInvoker.GetItemAsync<TodoItemModelWithFile>($"{ApiEndpoints.TodoItems}/{id}");

            if (isDone)
                await ChangeStatusToAsync(todoItemModel, "Done");
            else
                await ChangeStatusToAsync(todoItemModel, "Ongoing");

            var viewModel = await viewModelService.CreateIndexViewModelAsync();
            return PartialView("_ViewAll", viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public ActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        private async Task ChangeStatusToAsync(TodoItemModelWithFile todoItemModel, string statusName)
        {
            var status = await apiInvoker.GetItemAsync<StatusModel>($"{ApiEndpoints.StatusByName}/{statusName}");
            todoItemModel.StatusId = status.Id;

            await apiInvoker.PutItemAsync(ApiEndpoints.TodoItems, todoItemModel);
        }

        private static void AddGeoPoint(TodoItemModelWithFile todoItemModel)
        {
            double latitude = double.Parse(todoItemModel.Latitude.Replace(',', '.'), CultureInfo.InvariantCulture);
            double longitude = double.Parse(todoItemModel.Longitude.Replace(',', '.'), CultureInfo.InvariantCulture);

            todoItemModel.GeoPoint = new(longitude, latitude);
        }

        private async Task AddCategory(TodoItemModelWithFile todoItemModel)
        {
            await apiInvoker.PostItemAsync(ApiEndpoints.Categories, new CategoryModel { Name = todoItemModel.CategoryName });

            var category = await apiInvoker.GetItemAsync<CategoryModel>($"{ApiEndpoints.CategoryByName}/{todoItemModel.CategoryName}");
            todoItemModel.CategoryId = category.Id;
        }

        private async Task AttachImage(TodoItemModelWithFile todoItemModel)
        {
            if (todoItemModel.Image is not null)
            {
                using var stream = todoItemModel.Image.OpenReadStream();
                byte[] fileBytes = await stream.ToByteArrayAsync();

                string file = await apiInvoker.PostFileAsync(ApiEndpoints.Images, todoItemModel.Image.FileName, fileBytes);
                var image = await apiInvoker.GetItemAsync<ImageModel>($"{ApiEndpoints.ImageByName}/{file}");

                todoItemModel.ImageId = image.Id;
            }
        }
    }
}
