using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.Services.Api;

namespace ToDoList.MvcClient.Controllers
{
    public class UserController : Controller
    {
        private readonly IApiInvoker apiInvoker;
        private readonly IStringLocalizer<UserController> localizer;

        public UserController(IApiInvoker invoker, IStringLocalizer<UserController> stringLocalizer)
        {
            apiInvoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
            localizer = stringLocalizer;
        }

        public IActionResult Login()
        {
            return View("Login");
        }

        public IActionResult Register()
        {
            return View("Register");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsync(UserModel userModel)
        {
            _ = userModel ?? throw new ArgumentNullException(nameof(userModel));

            try
            {
                await apiInvoker.AuthenticateUserAsync("User/Register", userModel);
            }
            catch (Exception e)
            {
                if (e.Message == "Unauthorized")
                {
                    ModelState.AddModelError(string.Empty, localizer["UserExists"]);
                    return View("Register");
                }
            }

            return RedirectToAction("Index", "Todo");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(UserModel userModel)
        {
            _ = userModel ?? throw new ArgumentNullException(nameof(userModel));

            try
            {
                await apiInvoker.AuthenticateUserAsync("User/Login", userModel);
            }
            catch (Exception e)
            {
                if (e.Message == "Unauthorized")
                {
                    if (ModelState.ContainsKey("ConfirmPassword"))
                        ModelState["ConfirmPassword"].Errors.Clear();

                    ModelState.AddModelError(string.Empty, localizer["InvalidCredentials"]);
                    return View("Login");
                }
            }

            return RedirectToAction("Index", "Todo");
        }

        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("Token");
            return RedirectToAction("Index", "Home");
        }
    }
}
