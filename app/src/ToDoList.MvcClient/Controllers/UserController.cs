using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.MvcClient.Controllers
{
    public class UserController : Controller
    {
        private readonly IApiInvoker apiInvoker;
        private readonly IStringLocalizer<UserController> localizer;

        public UserController(IApiInvoker invoker, IStringLocalizer<UserController> stringLocalizer)
        {
            apiInvoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
            localizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
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
                _ = await apiInvoker.AuthenticateUserAsync("Authentication/Register", userModel);
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
                _ = await apiInvoker.AuthenticateUserAsync("Authentication/Login", userModel);
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

        public async Task<IActionResult> LogoutAsync()
        {
            await apiInvoker.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
