using System;
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
            apiCallsService = apiService ?? throw new ArgumentNullException(nameof(apiService));
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
                await apiCallsService.AuthenticateUserAsync("User/Register", userModel);
            }
            catch (Exception e)
            {
                if (e.Message == "Unauthorized")
                {
                    ModelState.AddModelError(string.Empty, "User exists");
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
                await apiCallsService.AuthenticateUserAsync("User/Login", userModel);
            }
            catch (Exception e)
            {
                if (e.Message == "Unauthorized")
                {
                    if (ModelState.ContainsKey("ConfirmPassword"))
                        ModelState["ConfirmPassword"].Errors.Clear();

                    ModelState.AddModelError(string.Empty, "Username or password is incorrect");
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
