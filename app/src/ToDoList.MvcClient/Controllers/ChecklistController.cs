using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using ToDoList.MvcClient.Services;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.MvcClient.Controllers
{
    public class ChecklistController : Controller
    {
        private readonly IViewModelService viewModelService;
        private readonly IApiInvoker apiInvoker;

        public ChecklistController(IApiInvoker invoker, IViewModelService modelService)
        {
            apiInvoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
            viewModelService = modelService ?? throw new ArgumentNullException(nameof(modelService));
        }

        // GET: ChecklistController/CreateOrUpdate
        public async Task<ActionResult> CreateOrUpdateAsync(int id = 0, int userId = 0)
        {
            if (id == 0)
                return View("CreateOrUpdate", new ChecklistModel { UserId = userId });

            var checklistModel = await apiInvoker.GetItemAsync<ChecklistModel>("Checklists/" + id);
            return checklistModel is not null
                ? View("CreateOrUpdate", checklistModel)
                : NotFound();
        }

        // POST: ChecklistController/CreateOrUpdate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdateAsync(ChecklistModel checklistModel)
        {
            _ = checklistModel ?? throw new ArgumentNullException(nameof(checklistModel));

            if (!ModelState.IsValid)
                return PartialView("CreateOrUpdate", checklistModel);

            if (checklistModel.Id == 0)
                await apiInvoker.PostItemAsync("Checklists", checklistModel);

            if (checklistModel.Id != 0)
                await apiInvoker.PutItemAsync("Checklists", checklistModel);

            var viewModel = await viewModelService.CreateIndexViewModelAsync();
            return PartialView("_ViewAll", viewModel);
        }

        // POST: ChecklistController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await apiInvoker.DeleteItemAsync("Checklists/", id);
            var viewModel = await viewModelService.CreateIndexViewModelAsync();

            return PartialView("_ViewAll", viewModel);
        }
    }
}
