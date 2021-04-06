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
        private readonly ICreateViewModelService viewModelService;
        private readonly IApiCallsService apiCallsService;
        private readonly IImageAddingService imageAddingService;

        public TodoController(IApiCallsService apiService, IImageAddingService addingService, ICreateViewModelService modelService) : base()
        {
            apiCallsService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            imageAddingService = addingService ?? throw new ArgumentNullException(nameof(addingService));
            viewModelService = modelService ?? throw new ArgumentNullException(nameof(modelService));
        }

        public async Task<ActionResult> IndexAsync()
        {
            IndexViewModel viewModel = await viewModelService.CreateIndexViewModelAsync();
            return View("Index", viewModel);
        }

        public async Task<ActionResult> CreateOrUpdateAsync(int checklistId, int todoItemId = 0, int parentId = 0)
        {
            string viewName = "CreateOrUpdate";

            if (todoItemId == 0)
            {
                TodoItemModel todoItem = new() { ChecklistId = checklistId };

                if (parentId == 0)
                    return View(viewName, await viewModelService.CreateViewModelCreateOrUpdateTodoItemAsync(todoItem));

                todoItem.ParentId = parentId;
                return View(viewName, await viewModelService.CreateViewModelCreateOrUpdateTodoItemAsync(todoItem));
            }

            var todoItemModel = await apiCallsService.GetItemAsync<TodoItemModel>("TodoItems/" + todoItemId);

            if (todoItemModel?.GeoPoint is not null)
            {
                todoItemModel.Latitude = todoItemModel.GeoPoint.Latitude.ToString();
                todoItemModel.Longitude = todoItemModel.GeoPoint.Longitude.ToString();
            }

            return todoItemModel is not null
                ? View(viewName, await viewModelService.CreateViewModelCreateOrUpdateTodoItemAsync(todoItemModel))
                : NotFound();
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

            if (createViewModel.TodoItemModel.Id == 0)
                await AddTodoItem(createViewModel.TodoItemModel);

            if (createViewModel.TodoItemModel.Id != 0)
                await UpdateTodoItem(createViewModel.TodoItemModel);

            IndexViewModel viewModel = await viewModelService.CreateIndexViewModelAsync();
            return PartialView("_ViewAll", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await apiCallsService.DeleteItemAsync("TodoItems/", id);
            var viewModel = await viewModelService.CreateIndexViewModelAsync();

            return PartialView("_ViewAll", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> MarkTodoItemAsync(int id, bool isDone)
        {
            var todoItemModel = await apiCallsService.GetItemAsync<TodoItemModel>("TodoItems/" + id);

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
            var status = await apiCallsService.GetItemAsync<StatusModel>("Statuses/GetByName/" + statusName);
            todoItemModel.StatusId = status.Id;

            await apiCallsService.PutItemAsync("TodoItems", todoItemModel);
        }

        private static void AddGeoPoint(TodoItemModel todoItemModel)
        {
            double latitude = double.Parse(todoItemModel.Latitude.Replace(',', '.'), CultureInfo.InvariantCulture);
            double longitude = double.Parse(todoItemModel.Longitude.Replace(',', '.'), CultureInfo.InvariantCulture);

            todoItemModel.GeoPoint = new(longitude, latitude);
        }

        private async Task AddCategory(TodoItemModel todoItemModel)
        {
            await apiCallsService.PostItemAsync("Categories", new CategoryModel { Name = todoItemModel.CategoryName });

            var category = await apiCallsService.GetItemAsync<CategoryModel>("Categories/GetByName/" + todoItemModel.CategoryName);
            todoItemModel.CategoryId = category.Id;
        }

        private static bool LatLongExists(TodoItemModel todoItemModel)
        {
            return !string.IsNullOrWhiteSpace(todoItemModel.Latitude)
                   && !string.IsNullOrWhiteSpace(todoItemModel.Latitude);
        }

        private async Task AddTodoItem(TodoItemModel todoItemModel)
        {
            if (todoItemModel.Image is not null)
                await imageAddingService.AddImageInTodoItemAsync(todoItemModel);

            await apiCallsService.PostItemAsync("TodoItems", todoItemModel);
        }

        private async Task UpdateTodoItem(TodoItemModel todoItemModel)
        {
            if (todoItemModel.Image is not null)
                await imageAddingService.AddImageInTodoItemAsync(todoItemModel);

            await apiCallsService.PutItemAsync("TodoItems", todoItemModel);
        }
    }
}
