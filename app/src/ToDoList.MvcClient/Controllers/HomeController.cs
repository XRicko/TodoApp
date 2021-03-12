using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using ToDoList.MvcClient.API;
using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.ViewModels;

namespace ToDoList.MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public HomeController(IWebHostEnvironment hostEnvironment)
        {
            webHostEnvironment = hostEnvironment;
        }

        public async Task<ActionResult> IndexAsync()
        {
            var todoItems = await GetItems<TodoItemModel>("TodoItems");
            var checklistModels = await GetItems<ChecklistModel>("Checklists");

            IndexViewModel viewModel = new() { ChecklistModels = checklistModels, TodoItemModels = todoItems };

            return View(viewModel);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(TodoItemModel todoItem)
        {
            var todoItems = await GetItems<TodoItemModel>("TodoItems");
            var fileName = GetUniqueFileNameAsync(todoItem);

            using HttpResponseMessage response = await WebApiHelper.WebApiClient.PostAsJsonAsync("TodoItems", todoItem);

            return response.IsSuccessStatusCode
                ? Json(new { isValid = true, html = RazorViewToStringConvertor.RenderRazorViewToString(this, "_ViewAll", todoItems) })
                : throw new Exception(response.ReasonPhrase);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            using HttpResponseMessage response = await WebApiHelper.WebApiClient.DeleteAsync("TodoItems/" + id);

            return response.IsSuccessStatusCode
                ? RedirectToAction("Index")
                : throw new Exception(response.ReasonPhrase);
        }

        public IActionResult Privacy() 
            => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => 
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        private static async Task<IEnumerable<T>> GetItems<T>(string route) where T : BaseModel
        {
            using HttpResponseMessage response = await WebApiHelper.WebApiClient.GetAsync(route);

            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsAsync<IEnumerable<T>>()
                : throw new Exception(response.ReasonPhrase);
        }

        private async Task<string> GetUniqueFileNameAsync(TodoItemModel model)
        {
            string uniqueFileName = null;

            if (model.Image is not null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
