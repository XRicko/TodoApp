using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using ToDoList.SharedClientLibrary;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Resources;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.MvcClient.Controllers
{
    public class UserController : Controller
    {
        private readonly IApiInvoker apiInvoker;
        private readonly IStringLocalizer<General> localizer;

        public UserController(IApiInvoker invoker, IStringLocalizer<General> stringLocalizer)
        {
            apiInvoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
            localizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
        }

        public ActionResult Login()
        {
            return View("Login");
        }

        public ActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterAsync(UserModel userModel)
        {
            _ = userModel ?? throw new ArgumentNullException(nameof(userModel));

            try
            {
                _ = await apiInvoker.AuthenticateUserAsync(ApiEndpoints.Register, userModel);
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ModelState.AddModelError(string.Empty, localizer["Registration Failed"]);
                return View("Register");
            }

            return RedirectToAction("Index", "Todo");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginAsync(UserModel userModel)
        {
            _ = userModel ?? throw new ArgumentNullException(nameof(userModel));

            try
            {
                _ = await apiInvoker.AuthenticateUserAsync(ApiEndpoints.Login, userModel);
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (ModelState.ContainsKey("ConfirmPassword"))
                    ModelState["ConfirmPassword"].Errors.Clear();

                ModelState.AddModelError(string.Empty, localizer["Login Failed"]);
                return View("Login");
            }

            return RedirectToAction("Index", "Todo");
        }

        public async Task<ActionResult> LogoutAsync()
        {
            await apiInvoker.LogOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
