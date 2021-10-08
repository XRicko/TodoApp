using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using ToDoList.MvcClient.Models;
using ToDoList.SharedClientLibrary.Services;
using ToDoList.SharedKernel;

namespace ToDoList.MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITokenStorage tokenStorage;

        public HomeController(ITokenStorage storage)
        {
            tokenStorage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public async Task<ActionResult> IndexAsync()
        {
            string refreshToken = await tokenStorage.GetTokenAsync(Constants.RefreshToken);

            return string.IsNullOrWhiteSpace(refreshToken)
                ? View("Index")
                : RedirectToAction("Index", "Todo");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public ActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
