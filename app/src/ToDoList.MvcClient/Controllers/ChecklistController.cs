using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.Services;

namespace ToDoList.MvcClient.Controllers
{
    public class ChecklistController : Controller
    {
        private readonly ICreateViewModelService viewModelService;
        private readonly IApiCallsService apiCallsService;

        public ChecklistController(IApiCallsService apiService, ICreateViewModelService modelService)
        {
            apiCallsService = apiService;
            viewModelService = modelService;
        }

        // GET: ChecklistController/Create
        public async Task<ActionResult> CreateOrUpdateAsync(int id = 0)
        {
            if (id == 0)
                return View(new ChecklistModel());

            var checklistModel = await apiCallsService.GetItemAsync<ChecklistModel>("Checklists/" + id);
            return checklistModel is not null
                ? View(checklistModel)
                : NotFound();
        }

        // POST: ChecklistController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdateAsync(ChecklistModel checklistModel)
        {
            if (!ModelState.IsValid)
                Json(new { isValid = false, html = RazorViewToStringConverter.RenderRazorViewToString(this, "CreateOrUpdate", checklistModel) });

            if (checklistModel.Id == 0)
                await apiCallsService.PostItemAsync("Checklists", checklistModel);

            if (checklistModel.Id != 0)
                await apiCallsService.PutItemAsync("Checklists", checklistModel);

            var viewModel = await viewModelService.CreateIndexViewModel();
            return Json(new { isValid = true, html = RazorViewToStringConverter.RenderRazorViewToString(this, "_ViewAll", viewModel) });
        }

        // POST: ChecklistController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
