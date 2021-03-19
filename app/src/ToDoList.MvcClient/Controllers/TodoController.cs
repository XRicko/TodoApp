using System.Diagnostics;
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

        public TodoController(IApiCallsService apiService, IImageAddingService addingService, ICreateViewModelService modelService)
        {
            apiCallsService = apiService;
            imageAddingService = addingService;
            viewModelService = modelService;
        }

        public async Task<ActionResult> IndexAsync()
        {
            IndexViewModel viewModel = await viewModelService.CreateIndexViewModel();
            return View(viewModel);
        }

        public async Task<ActionResult> CreateOrUpdateAsync(int checklistId, int todoItemId = 0, int parentId = 0)
        {
            if (todoItemId == 0)
            {
                TodoItemModel todoItem = new() { ChecklistId = checklistId };

                if (parentId == 0)
                    return View(await viewModelService.CreateViewModelCreateOrUpdateTodoItem(todoItem));

                todoItem.ParentId = parentId;
                return View(await viewModelService.CreateViewModelCreateOrUpdateTodoItem(todoItem));
            }

            var todoItemModel = await apiCallsService.GetItemAsync<TodoItemModel>("TodoItems/" + todoItemId);
            todoItemModel.ChecklistId = checklistId;

            return todoItemModel is not null
                ? View(await viewModelService.CreateViewModelCreateOrUpdateTodoItem(todoItemModel))
                : NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdateAsync(CreateViewModel createViewModel)
        {
            if (!ModelState.IsValid)
            {
                Json(new { isValid = false, html = RazorViewToStringConverter.RenderRazorViewToString(this, "CreateOrUpdate", createViewModel.TodoItemModel) });
            }

            if (createViewModel.TodoItemModel.Id == 0)
                await AddTodoItem(createViewModel.TodoItemModel);

            if (createViewModel.TodoItemModel.Id != 0)
                await apiCallsService.PutItemAsync("TodoItems", createViewModel.TodoItemModel);

            IndexViewModel viewModel = await viewModelService.CreateIndexViewModel();
            return Json(new { isValid = true, html = RazorViewToStringConverter.RenderRazorViewToString(this, "_ViewAll", viewModel) });
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await apiCallsService.DeleteItemAsync("TodoItems/", id);
            var viewModel = await viewModelService.CreateIndexViewModel();

            return Json(new { html = RazorViewToStringConverter.RenderRazorViewToString(this, "_ViewAll", viewModel) });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        public IActionResult Privacy() => View();

        private async Task AddTodoItem(TodoItemModel todoItemModel)
        {
            if (todoItemModel.Image is not null)
                await imageAddingService.AddImageInTodoItem(todoItemModel);

            await apiCallsService.PostItemAsync("TodoItems", todoItemModel);
        }
    }
}
