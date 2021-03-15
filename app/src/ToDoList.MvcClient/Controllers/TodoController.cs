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
            IndexViewModel viewModel = await CreateViewModel();
            return View(viewModel);
        }

        public async Task<ActionResult> CreateOrUpdateAsync(int id = 0)
        {
            if (id == 0)
                return View(new TodoItemModel());

            var todoItem = await apiCallsService.GetItemAsync<TodoItemModel>("TodoItems/" + id);
            return todoItem is not null
                ? View(todoItem)
                : NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdateAsync(TodoItemModel todoItem)
        {
            if (ModelState.IsValid)
            {
                if (todoItem.Id == 0)
                {
                    if (todoItem.Image is not null)
                        await AddImageInTodoItem(todoItem);

                    await apiCallsService.PostItemAsync("TodoItems", todoItem);
                    var viewModel = await CreateViewModel();

                    return Json(new { isValid = true, html = RazorViewToStringConverter.RenderRazorViewToString(this, "_ViewAll", viewModel) });
                }

                // TODO: Update item
            }

            return Json(new { isValid = false, html = RazorViewToStringConverter.RenderRazorViewToString(this, "CreateOrUpdate", todoItem) });
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await apiCallsService.DeleteItemAsync("TodoItems/", id);
            var viewModel = await CreateViewModel();

            return Json(new { html = RazorViewToStringConverter.RenderRazorViewToString(this, "_ViewAll", viewModel) });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        public IActionResult Privacy() => View();

        private async Task<IndexViewModel> CreateViewModel()
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
