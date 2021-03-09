using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using ToDoList.MvcClient.API;
using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.ViewModels;
using ToDoList.SharedKernel;

namespace ToDoList.MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<ActionResult> IndexAsync()
        {
            IEnumerable<TodoItemModel> todoItems = await GetItems<TodoItemModel>("TodoItems");
            IEnumerable<ChecklistModel> checklistModels = await GetItems<ChecklistModel>("Checklists");

            IndexViewModel viewModel = new() { ChecklistModels = checklistModels, TodoItemModels = todoItems };

            return View(viewModel);
        }

        private static async Task<IEnumerable<T>> GetItems<T>(string route) where T : BaseEntity
        {
            using HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(route);

            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            IEnumerable<T> items = await response.Content.ReadAsAsync<IEnumerable<T>>();
            return items;
        }

        public ActionResult Create()
        {
            return View();
        }

        //[ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> CreateAsync(TodoItemModel todoItem)
        {
            using (HttpResponseMessage response = await ApiHelper.ApiClient.PostAsJsonAsync("TodoItems", todoItem))
            {
                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");

                throw new Exception(response.ReasonPhrase);
            }
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        public ActionResult Delete(int id)
        {

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
