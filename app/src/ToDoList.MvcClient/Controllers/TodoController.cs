using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.Services;
using ToDoList.MvcClient.ViewModels;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.MvcClient.Controllers
{
    public class TodoController : Controller
    {
        private readonly IViewModelService viewModelService;
        private readonly IFileConverter fileConverter;

        private readonly IApiInvoker apiInvoker;

        public TodoController(IViewModelService modelService, IFileConverter converter, IApiInvoker invoker) : base()
        {
            viewModelService = modelService ?? throw new ArgumentNullException(nameof(modelService));
            fileConverter = converter ?? throw new ArgumentNullException(nameof(converter));

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

            var todoItemModel = await apiInvoker.GetItemAsync<TodoItemModelWithFile>("TodoItems/" + todoItemId);

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

            if (IsNewItem())
                await AddTodoItem(createViewModel.TodoItemModel);

            if (!IsNewItem())
                await UpdateTodoItem(createViewModel.TodoItemModel);

            return PartialView("_ViewAll", await viewModelService.CreateIndexViewModelAsync());

            bool LatLongExists() => !string.IsNullOrWhiteSpace(createViewModel.TodoItemModel.Latitude)
                                    && !string.IsNullOrWhiteSpace(createViewModel.TodoItemModel.Latitude);
            bool CategoryExists() => !string.IsNullOrWhiteSpace(createViewModel.TodoItemModel.CategoryName);

            bool IsNewItem() => createViewModel.TodoItemModel.Id is 0;
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await apiInvoker.DeleteItemAsync("TodoItems/", id);
            var viewModel = await viewModelService.CreateIndexViewModelAsync();

            return PartialView("_ViewAll", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> MarkTodoItemAsync(int id, bool isDone)
        {
            var todoItemModel = await apiInvoker.GetItemAsync<TodoItemModelWithFile>("TodoItems/" + id);

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
            var status = await apiInvoker.GetItemAsync<StatusModel>("Statuses/GetByName/" + statusName);
            todoItemModel.StatusId = status.Id;

            await apiInvoker.PutItemAsync("TodoItems", todoItemModel);
        }

        private static void AddGeoPoint(TodoItemModelWithFile todoItemModel)
        {
            double latitude = double.Parse(todoItemModel.Latitude.Replace(',', '.'), CultureInfo.InvariantCulture);
            double longitude = double.Parse(todoItemModel.Longitude.Replace(',', '.'), CultureInfo.InvariantCulture);

            todoItemModel.GeoPoint = new(longitude, latitude);
        }

        private async Task AddCategory(TodoItemModelWithFile todoItemModel)
        {
            await apiInvoker.PostItemAsync("Categories", new CategoryModel { Name = todoItemModel.CategoryName });

            var category = await apiInvoker.GetItemAsync<CategoryModel>("Categories/GetByName/" + todoItemModel.CategoryName);
            todoItemModel.CategoryId = category.Id;
        }

        private async Task AddTodoItem(TodoItemModelWithFile todoItemModel)
        {
            await AttachImage(todoItemModel);
            await apiInvoker.PostItemAsync("TodoItems", todoItemModel);
        }

        private async Task UpdateTodoItem(TodoItemModelWithFile todoItemModel)
        {
            await AttachImage(todoItemModel);
            await apiInvoker.PutItemAsync("TodoItems", todoItemModel);
        }

        private async Task AttachImage(TodoItemModelWithFile todoItemModel)
        {
            if (todoItemModel.Image is not null)
            {
                using var fileStream = todoItemModel.Image.OpenReadStream();
                byte[] fileBytes = await fileConverter.ConvertToByteArrayAsync(fileStream);

                string file = await apiInvoker.PostFileAsync("Images", todoItemModel.Image.FileName, fileBytes);
                var image = await apiInvoker.GetItemAsync<ImageModel>("Images/GetByName/" + file);

                todoItemModel.ImageId = image.Id;
            }
        }
    }
}
