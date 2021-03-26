using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.Services;
using ToDoList.MvcClient.Services.Api;

namespace ToDoList.MvcClient.Controllers
{
    public class ChecklistController : Controller
    {
        private readonly ICreateViewModelService viewModelService;
        private readonly IApiCallsService apiCallsService;

        public ChecklistController(IApiCallsService apiService, ICreateViewModelService modelService)
        {
            apiCallsService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            viewModelService = modelService ?? throw new ArgumentNullException(nameof(modelService));
        }

        // GET: ChecklistController/CreateOrUpdate
        public async Task<ActionResult> CreateOrUpdateAsync(int id = 0, int userId = 0)
        {
            if (id == 0)
                return View(new ChecklistModel { UserId = userId });

            var checklistModel = await apiCallsService.GetItemAsync<ChecklistModel>("Checklists/" + id);
            return checklistModel is not null
                ? View(checklistModel)
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
                await apiCallsService.PostItemAsync("Checklists", checklistModel);

            if (checklistModel.Id != 0)
                await apiCallsService.PutItemAsync("Checklists", checklistModel);

            var viewModel = await viewModelService.CreateIndexViewModel();
            return PartialView("_ViewAll", viewModel);
        }

        // POST: ChecklistController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await apiCallsService.DeleteItemAsync("Checklists/", id);
            var viewModel = await viewModelService.CreateIndexViewModel();

            return PartialView("_ViewAll", viewModel);
        }
    }
}
