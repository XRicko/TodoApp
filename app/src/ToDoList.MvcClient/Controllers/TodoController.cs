using System.Diagnostics;
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

        public async Task<ActionResult> CreateOrUpdateAsync(int id = 0)
        {
            if (id == 0)
                return View(await CreateUpdateOrCreateViewModel(new TodoItemModel()));

            var todoItem = await apiCallsService.GetItemAsync<TodoItemModel>("TodoItems/" + id);
            return todoItem is not null 
                ? View(await CreateUpdateOrCreateViewModel(todoItem)) 
                : (ActionResult)NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdateAsync(CreateViewModel createViewModel)
        {
            if (ModelState.IsValid)
            {
                if (createViewModel.TodoItemModel.Id == 0)
                {
                    if (createViewModel.TodoItemModel.Image is not null)
                        await AddImageInTodoItem(createViewModel.TodoItemModel);

                    await apiCallsService.PostItemAsync("TodoItems", createViewModel.TodoItemModel);
                    var viewModel = await CreateIndexViewModel();

                    return Json(new { isValid = true, html = RazorViewToStringConverter.RenderRazorViewToString(this, "_ViewAll", viewModel) });
                }

                // TODO: Update item
            }

            return Json(new { isValid = false, html = RazorViewToStringConverter.RenderRazorViewToString(this, "CreateOrUpdate", createViewModel.TodoItemModel) });
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
            var checklistModels = await apiCallsService.GetItemsAsync<ChecklistModel>("Checklists");
            var categoryModels = await apiCallsService.GetItemsAsync<CategoryModel>("Categories");
            var statusModels = await apiCallsService.GetItemsAsync<StatusModel>("Statuses");

            CreateViewModel viewModel = new() {TodoItemModel = todoItemModel, ChecklistModels = checklistModels, CategoryModels = categoryModels, StatusModels = statusModels };

            return viewModel;
        }

        private async Task<IndexViewModel> CreateIndexViewModel()
        {
            var todoItems = await apiCallsService.GetItemsAsync<TodoItemModel>("TodoItems");
            var checklistModels = await apiCallsService.GetItemsAsync<ChecklistModel>("Checklists");

            IndexViewModel viewModel = new() { ChecklistModels = checklistModels, TodoItemModels = todoItems };

            return viewModel;
        }

        private async Task AddImageInTodoItem(TodoItemModel todoItem)
        {
            await imageFileService.AddImage(todoItem.Image);

            var image = await apiCallsService.GetItemAsync<ImageModel>("Images/GetByName/" + todoItem.Image.FileName);
            todoItem.ImageId = image.Id;
        }
    }
}
