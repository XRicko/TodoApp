using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.Services;
using ToDoList.MvcClient.ViewModels;

namespace ToDoList.MvcClient.Controllers
{
    public class TodoController : Controller
    {
        private readonly IApiCallsService apiCallsService;
        private readonly IImageService imageFileService;

        public TodoController(IApiCallsService apiService, IImageService fileService)
        {
            apiCallsService = apiService;
            imageFileService = fileService;
        }

        public async Task<ActionResult> IndexAsync()
        {
            IndexViewModel viewModel = await CreateIndexViewModel();
            return View(viewModel);
        }

        public async Task<ActionResult> CreateOrUpdateAsync(int checklistId, int todoItemId = 0)
        {
            if (todoItemId == 0)
                return View(await CreateUpdateOrCreateViewModel(new TodoItemModel { ChecklistId = checklistId }));

            var todoItem = await apiCallsService.GetItemAsync<TodoItemModel>("TodoItems/" + todoItemId);
            return todoItem is not null 
                ? View(await CreateUpdateOrCreateViewModel(todoItem)) 
                : NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdateAsync(CreateViewModel createViewModel)
        {
            if (!ModelState.IsValid)
                return GetJsonView(createViewModel.TodoItemModel, false, "CreateOrUpdate");

            IndexViewModel viewModel = default;

            if (createViewModel.TodoItemModel.Id == 0)
            {
                await AddTodoItem(createViewModel);
                viewModel = await CreateIndexViewModel();
            }
            // TODO: Update item

            return GetJsonView(viewModel, true, "_ViewAll");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await apiCallsService.DeleteItemAsync("TodoItems/", id);
            var viewModel = await CreateIndexViewModel();

            return Json(new { html = RazorViewToStringConverter.RenderRazorViewToString(this, "_ViewAll", viewModel) });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        public IActionResult Privacy() => View();

        private async Task<CreateViewModel> CreateUpdateOrCreateViewModel(TodoItemModel todoItemModel)
        {
            var categoryModels = await apiCallsService.GetItemsAsync<CategoryModel>("Categories");
            var statusModels = await apiCallsService.GetItemsAsync<StatusModel>("Statuses");

            CreateViewModel viewModel = new() {TodoItemModel = todoItemModel, CategoryModels = categoryModels, StatusModels = statusModels };

            return viewModel;
        }

        private async Task<IndexViewModel> CreateIndexViewModel()
        {
            var todoItems = await apiCallsService.GetItemsAsync<TodoItemModel>("TodoItems");
            var checklistModels = await apiCallsService.GetItemsAsync<ChecklistModel>("Checklists");

            IndexViewModel viewModel = new() { ChecklistModels = checklistModels, TodoItemModels = todoItems };

            return viewModel;
        }

        private async Task AddTodoItem(CreateViewModel createViewModel)
        {
            if (createViewModel.TodoItemModel.Image is not null)
                await AddImageInTodoItem(createViewModel.TodoItemModel);

            await apiCallsService.PostItemAsync("TodoItems", createViewModel.TodoItemModel);
        }

        private async Task AddImageInTodoItem(TodoItemModel todoItem)
        {
            await imageFileService.AddImage(todoItem.Image);

            var image = await apiCallsService.GetItemAsync<ImageModel>("Images/GetByName/" + todoItem.Image.FileName);
            todoItem.ImageId = image.Id;
        }

        private ActionResult GetJsonView(object view, bool isValid, string viewName) =>
            Json(new { isValid, html = RazorViewToStringConverter.RenderRazorViewToString(this, viewName, view) });
    }
}
