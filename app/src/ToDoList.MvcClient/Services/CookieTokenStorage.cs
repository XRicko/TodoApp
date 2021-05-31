using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.MvcClient.Services
{
    public class CookieTokenStorage : ITokenStorage
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ITokenParser tokenParser;

        public CookieTokenStorage(IHttpContextAccessor contextAccessor, ITokenParser parser)
        {
            httpContextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            tokenParser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        public Task<string> GetTokenAsync(string key) =>
            Task.FromResult(httpContextAccessor.HttpContext.Request.Cookies[key]);

        public async Task SetTokenAsync(string key, string token)
        {
            await Task.Run(() => httpContextAccessor.HttpContext.Response.Cookies.Append(key, token,
                new CookieOptions { Expires = tokenParser.GetExpiryDate(token) }));
        }

        public async Task RemoveTokenAsync(string key) =>
            await Task.Run(() => httpContextAccessor.HttpContext.Response.Cookies.Delete(key));
    }
}

// TODO: cookie expiry time, refreshToken deserialization, continue refactroring
