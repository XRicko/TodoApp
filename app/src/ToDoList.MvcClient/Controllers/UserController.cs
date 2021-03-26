using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.Services.Api;

namespace ToDoList.MvcClient.Controllers
{
    public class UserController : Controller
    {
        private readonly IApiCallsService apiCallsService;

        public UserController(IApiCallsService apiService)
        {
            apiCallsService = apiService ?? throw new System.ArgumentNullException(nameof(apiService));
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsync(UserModel userModel)
        {
            _ = userModel ?? throw new System.ArgumentNullException(nameof(userModel));

            await apiCallsService.AuthenticateUserAsync("User/Register", userModel);
            return RedirectToAction("Index", "Todo");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(UserModel userModel)
        {
            _ = userModel ?? throw new System.ArgumentNullException(nameof(userModel));

            await apiCallsService.AuthenticateUserAsync("User/Login", userModel);
            return RedirectToAction("Index", "Todo");
        }

        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("Token");
            return RedirectToAction("Index", "Home");
        }
    }
}
