using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ToDoList.MvcClient.API;
using ToDoList.MvcClient.Models;

namespace ToDoList.MvcClient.Controllers
{
    public class TodoItemController : Controller
    {
        // GET: TodoItemController
        public async Task<ActionResult> IndexAsync()
        {
            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync("TodoItems"))
            {
                if (response.IsSuccessStatusCode)
                {
                    IEnumerable<TodoItemModel> todoItems = await response.Content.ReadAsAsync<IEnumerable<TodoItemModel>>();
                    return View(todoItems);
                }

                throw new Exception(response.ReasonPhrase);
            }
        }

        // GET: TodoItemController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }
    }
}
