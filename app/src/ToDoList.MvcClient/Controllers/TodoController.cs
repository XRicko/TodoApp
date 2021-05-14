using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.Services;
using ToDoList.MvcClient.Services.Api;
using ToDoList.MvcClient.ViewModels;

namespace ToDoList.MvcClient.Controllers
{
    public class TodoController : Controller
    {
        private readonly IViewModelService viewModelService;
        private readonly IApiInvoker apiInvoker;
        private readonly IImageAddingService imageAddingService;

        public TodoController(IApiInvoker invoker, IImageAddingService addingService, IViewModelService modelService) : base()
        {
            apiInvoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
            imageAddingService = addingService ?? throw new ArgumentNullException(nameof(addingService));
            viewModelService = modelService ?? throw new ArgumentNullException(nameof(modelService));
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
                TodoItemModel todoItem = new() { ChecklistId = checklistId };
                return View(viewName, await viewModelService.CreateViewModelCreateOrUpdateTodoItemAsync(todoItem));
            }

            var todoItemModel = await apiInvoker.GetItemAsync<TodoItemModel>("TodoItems/" + todoItemId);

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

            if (LatLongExists(createViewModel.TodoItemModel))
                AddGeoPoint(createViewModel.TodoItemModel);

            if (createViewModel.TodoItemModel.CategoryName is not null)
                await AddCategory(createViewModel.TodoItemModel);

            if (IsNewItem(createViewModel.TodoItemModel))
                await AddTodoItem(createViewModel.TodoItemModel);

            if (!IsNewItem(createViewModel.TodoItemModel))
                await UpdateTodoItem(createViewModel.TodoItemModel);

            IndexViewModel viewModel = await viewModelService.CreateIndexViewModelAsync();
            return PartialView("_ViewAll", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await apiInvoker.DeleteItemAsync("TodoItems/", id);
            var viewModel = await viewModelService.CreateIndexViewModelAsync();

            return PartialView("_ViewAll", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> MarkTodoItemAsync(int id, bool isDone)
        {
            var todoItemModel = await apiInvoker.GetItemAsync<TodoItemModel>("TodoItems/" + id);

            if (isDone)
                await ChangeStatusToAsync(todoItemModel, "Done");
            else
                await ChangeStatusToAsync(todoItemModel, "Ongoing");

            var viewModel = await viewModelService.CreateIndexViewModelAsync();
            return PartialView("_ViewAll", viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        private async Task ChangeStatusToAsync(TodoItemModel todoItemModel, string statusName)
        {
            var status = await apiInvoker.GetItemAsync<StatusModel>("Statuses/GetByName/" + statusName);
            todoItemModel.StatusId = status.Id;

            await apiInvoker.PutItemAsync("TodoItems", todoItemModel);
        }

        private static void AddGeoPoint(TodoItemModel todoItemModel)
        {
            double latitude = double.Parse(todoItemModel.Latitude.Replace(',', '.'), CultureInfo.InvariantCulture);
            double longitude = double.Parse(todoItemModel.Longitude.Replace(',', '.'), CultureInfo.InvariantCulture);

            todoItemModel.GeoPoint = new(longitude, latitude);
        }

        private async Task AddCategory(TodoItemModel todoItemModel)
        {
            await apiInvoker.PostItemAsync("Categories", new CategoryModel { Name = todoItemModel.CategoryName });

            var category = await apiInvoker.GetItemAsync<CategoryModel>("Categories/GetByName/" + todoItemModel.CategoryName);
            todoItemModel.CategoryId = category.Id;
        }

        private static bool LatLongExists(TodoItemModel todoItemModel)
        {
            return !string.IsNullOrWhiteSpace(todoItemModel.Latitude)
                   && !string.IsNullOrWhiteSpace(todoItemModel.Latitude);
        }

        private static bool IsNewItem(TodoItemModel todoItemModel) =>
            todoItemModel.Id is 0;

        private async Task AddTodoItem(TodoItemModel todoItemModel)
        {
            if (todoItemModel.Image is not null)
                await imageAddingService.AddImageInTodoItemAsync(todoItemModel);

            await apiInvoker.PostItemAsync("TodoItems", todoItemModel);
        }

        private async Task UpdateTodoItem(TodoItemModel todoItemModel)
        {
            if (todoItemModel.Image is not null)
                await imageAddingService.AddImageInTodoItemAsync(todoItemModel);

            await apiInvoker.PutItemAsync("TodoItems", todoItemModel);
        }
    }
}
